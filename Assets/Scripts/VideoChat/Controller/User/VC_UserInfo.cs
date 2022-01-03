using LGUVirtualOffice.Framework;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace LGUVirtualOffice
{
	public class VC_UserInfo : AbstractController
	{
		// 服务器获取
		public avAvatarMemberInfoManager memberInfo { get; private set; }
		public UserInfo userInfo { get; private set; }
		public new Collider collider { get; private set; }
		string userID;
		string userName;

		// Photon获取
		WaitForSeconds InfoRefreshSpacing = new WaitForSeconds(0.5f);
		public PhotonView photonView;
		public bool isLocalPlayer { get; private set; }
		string channelName;
		uint uid;

		// Local获取
		public WorkspaceAreaEnum Area { get; private set; }

		// 数据更新
		public bool isVideoMuted { get; private set; }
		public bool isAudioMuted { get; private set; }


		public IVCUserService UserService { get; private set; }
		public IAreaManager areaManager { get; private set; }
		public INetworkSyncService NetWorkService { get; private set; }

		private void Start()
		{
			UserService = this.GetService<IVCUserService>();
			areaManager = this.GetService<IAreaManager>();
			NetWorkService = this.GetService<INetworkSyncService>();
			isLocalPlayer = photonView.AmOwner;
			memberInfo = this.transform.parent.GetComponentInChildren<avAvatarMemberInfoManager>();
			userInfo = this.GetModel<UserInfo>();

			Initiailize();

			// 非本地用户进行刷新注册
			if (!isLocalPlayer)
			{
				StartCoroutine(IERefreshInfo());
				this.SubscribeEvent<UserMuteVideoEvent>((e) =>
				{
					if (e.uid == uid)
						isVideoMuted = e.isMuted;
				});
			}
			// 本地用户信息变更时，更新Info同步
			else
			{
				this.SubscribeEvent<LocalPushStateEvent>((e) =>
			   {
				   if (e.isVideo)
					   isVideoMuted = e.isMuted;
				   else
					   isAudioMuted = e.isMuted;
			   });
				this.SubscribeEvent<AreaChangeEvent>((e) => SetInfo(e.Area));
				this.SubscribeEvent<JoinChannelEvent>((e) => SetInfo(e.uid, e.channelName));
			}
		}
		/// <summary>
		/// 获取AWS相关数据
		/// </summary>
		void Initiailize()
		{
			Area = WorkspaceAreaEnum.Default;
			collider = this.GetComponent<Collider>();
			isVideoMuted = isAudioMuted = false;
			userName = userID = "";
			memberInfo.FetchData<string>("Name", (x) =>
			{
				if (x.Trim() == "")
					x = "Name";
				userName = x;
			});
		}
		IEnumerator IERefreshInfo()
		{
			yield return new WaitForSeconds(0.5f);
			for (; ; )
			{
				if (!this)
				{
					this.SendCommand<PlayerQuitCMD>(new PlayerQuitCMD { info = this });
					StopAllCoroutines();
				}

				RefreshInfo();
				UserService.LoadInfo(this);
				yield return InfoRefreshSpacing;
			}
		}

		#region 本地处理

		#endregion

		#region Member相关
		public string GetUserID()
		{
			userID = memberInfo.id;
			return userID;
		}

		public string GetName()
		{
			if (userName == "")
			{
				memberInfo.FetchData<string>("Name", (x) =>
				{
					if (x.Trim() == "")
						x = "Name";
					userName = x;
				});
			}
			return userName;
		}

		#endregion

		#region Photon相关信息
		// 赋值更新
		// VC信息变更
		public void SetInfo(uint uid, string channelName)
		{
			this.uid = uid;
			this.channelName = channelName;
			string VCInfo = uid.ToString() + '/' + channelName + '/' + ((int)Area).ToString();

			if (photonView.IsMine)
			{
				NetWorkService.SetPlayerVCInfo(VCInfo);
			}
		}
		// 区域信息变更
		public void SetInfo(WorkspaceAreaEnum Area)
		{
			this.Area = Area;
			string VCInfo = uid.ToString() + '/' + channelName + '/' + ((int)Area).ToString();

			if (photonView.IsMine)
			{
				NetWorkService.SetPlayerVCInfo(VCInfo);
			}
		}
		// 刷新远端用户信息
		void RefreshInfo()
		{
			if (photonView == null)
				return;

			string info = NetWorkService.GetPlayerVCInfo(photonView);
			if (info == null)
				info = "404/---/0";
			string oldInfo = GetVCInfo();
			if (oldInfo == info)
				return;

			string[] strs = info.Split('/');

			// UID
			if (isLocalPlayer)
				uid = 0;
			else
			{
				uid = uint.Parse(strs[0]);
			}
			// ChannelName
			channelName = strs[1];
			// Area
			WorkspaceAreaEnum NewArea = (WorkspaceAreaEnum)(int.Parse(strs[2]));
			if (NewArea != Area)
			{
				areaManager.RemoteSwitchArea(this, NewArea);
				Area = NewArea;
			}

			AgoraInfoChangeCMD cmd = new AgoraInfoChangeCMD
			{
				agoraInfo = oldInfo,
				info = this
			};
			this.SendCommand<AgoraInfoChangeCMD>(cmd);
			this.SendCommand<VCUserInfoUpdateCMD>(new VCUserInfoUpdateCMD { info = this });
		}
		// 获取
		public uint GetUID()
		{
			if (isLocalPlayer)
				uid = 0;
			return uid;
		}
		public WorkspaceAreaEnum GetArea()
		{
			return Area;
		}
		public string GetChannel()
		{
			return channelName;
		}
		public string GetVCInfo()
		{
			return uid.ToString() + '/' + channelName;
			#endregion
		}
	}
}
