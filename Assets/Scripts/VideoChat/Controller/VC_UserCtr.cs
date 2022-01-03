using LGUVirtualOffice.Framework;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
	public class VC_UserCtr : AbstractController
	{
		IVCService VCService;
		IVCUserService UserService;
		IAreaManager areaManager;

		UserInfo userInfo;
		VC_UserInfo localPlayer;
		public new SphereCollider collider;
		Dictionary<Collider, VC_UserInfo> Dic_ColInfo;
		Dictionary<Collider, Coroutine> Dic_ColUidCheck;

		WorkspaceAreaEnum targetArea;
		Coroutine AreaSwitchIE;
		Coroutine TryToEnterAreaIE;


		public bool isLocal { get; private set; }

		enum remoteTargetState
		{
			offline = 0,
			temp = 1,
			work = 2
		}

		private void Awake()
		{

		}

		/// <summary>
		/// 对控制器进行初始化,初始化后即可作用
		/// </summary>
		void Start()
		{
			userInfo = this.GetModel<UserInfo>();
			VCService = this.GetService<IVCService>();
			UserService = this.GetService<IVCUserService>();
			areaManager = this.GetService<IAreaManager>();


			targetArea = WorkspaceAreaEnum.Default;
			Dic_ColInfo = new Dictionary<Collider, VC_UserInfo>();
			Dic_ColUidCheck = new Dictionary<Collider, Coroutine>();
			isLocal = this.transform.parent.GetComponent<PhotonView>().IsMine;
			if (!isLocal)
			{
				collider.radius = 0.25f;
				collider.enabled = true;
				Destroy(transform.GetChild(0).gameObject);
				Destroy(this);
				return;
			}

			localPlayer = this.gameObject.GetComponent<VC_UserInfo>();
			collider.enabled = false;
			// 检测当前所处区域

			/////////////////////

			this.SubscribeEvent<UserOfflineEvent>((e) => UserLeaveChannel(UserService.GetUserInfo(e.uid)));
			this.SubscribeEvent<PlayerEnterAreaEvent>((e) => PlayerEnterNewArea(e.Area));
			this.SubscribeEvent<VCUserInfoUpdateEvent>((e) => AddVCUserInfo(e.info));

			UserService.ResetState();
			this.SendCommand<UserCreatSuccessedCommand>(new UserCreatSuccessedCommand { localInfo = localPlayer });
		}

		/// <summary>
		/// 进入缓冲区
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerEnter(Collider other)
		{


			if (AreaManager.CurRule == VCRuleEnum.Default)
				return;
			if (AreaManager.CurRule == VCRuleEnum.AutoConnect)
				return;
			if (other.tag != "VC_Player")
				return;
			VC_UserInfo info = TryGetVCInfo(other);

			if (info == null) return;

			// 结合房间规则进行判断
			if (info.GetArea() != localPlayer.GetArea())
				UserLeaveBufferArea(info);
			else if (info.GetChannel() != localPlayer.GetChannel())
				UserLeaveBufferArea(info);
			else
				UserStayBufferArea(info);
		}
		/// <summary>
		/// 退出缓冲区
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerExit(Collider other)
		{
			if (AreaManager.CurRule == VCRuleEnum.Default)
				return;
			if (AreaManager.CurRule == VCRuleEnum.AutoConnect)
				return;
			if (other.tag != "VC_Player")
				return;
			VC_UserInfo info = TryGetVCInfo(other);

			if (info == null) return;

			UserLeaveBufferArea(info);
		}
		/// <summary>
		/// 在缓冲区内
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerStay(Collider other)
		{
			if (AreaManager.CurRule == VCRuleEnum.Default)
				return;
			if (AreaManager.CurRule == VCRuleEnum.AutoConnect)
				return;
			if (other.tag != "VC_Player")
				return;
			VC_UserInfo info = TryGetVCInfo(other);

			if (info == null) return;

			if (info.GetArea() != localPlayer.GetArea())
			{
				UserLeaveBufferArea(info);
				return;
			}
			if (info.GetChannel() != localPlayer.GetChannel())
			{
				UserLeaveBufferArea(info);
				return;
			}

			// 距离判断
			float DisSqrRate = VC_Tool.GetDistanceSqrRate(other.transform, localPlayer.transform);
			if (AreaManager.CurRule == VCRuleEnum.Hallway)
			{
				// 大厅规则
				// 3倍距离以上不显示
				if (DisSqrRate > 9)
					UserStayBufferArea(info);
				else
					UserStayWorkArea(info);
			}
			else if (AreaManager.CurRule == VCRuleEnum.Fade)
			{
				// 渐隐规则
				if (DisSqrRate > 9)
					UserStayBufferArea(info);
				else if (DisSqrRate > 2.25f)
				{
					UserStayFadeArea(info, VC_Tool.GetAlpha(DisSqrRate, 9, 1.5f * 1.5f));
					this.SendCommand<RefreshViewCommand>(new RefreshViewCommand());
				}
				else
					UserStayWorkArea(info);
			}
		}

		void AddVCUserInfo(VC_UserInfo info)
		{
			if (Dic_ColInfo == null) Dic_ColInfo = new Dictionary<Collider, VC_UserInfo>();
			if (Dic_ColInfo.ContainsKey(info.collider))
			{
				if (Dic_ColInfo[info.collider] != info)
				{
					Dic_ColInfo.Remove(info.collider);
					Dic_ColInfo.Add(info.collider, info);
				}
			}
			else
			{
				Dic_ColInfo.Add(info.collider, info);
			}
		}

		VC_UserInfo TryGetVCInfo(Collider collider)
		{
			if (Dic_ColInfo == null) return null;
			if (Dic_ColInfo.ContainsKey(collider))
			{
				return Dic_ColInfo[collider];
			}
			else
				return null;
		}


		/// <summary>
		/// 玩家进入一个新区域
		/// </summary>
		public void PlayerEnterNewArea(WorkspaceAreaEnum area)
		{
			if (area == targetArea) return;

			targetArea = area;

			if (AreaSwitchIE == null)
				AreaSwitchIE = StartCoroutine(SwitchArea());
		}

		public void UserLeaveChannel(VC_UserInfo info)
		{
			if (info == null) return;

			UserLeaveBufferArea(info);
		}

		IEnumerator SwitchArea()
		{
			//退出旧区域，旧频道，关闭碰撞检测，停止旧尝试加入区域协程，数据重置
			VCService.LeaveChannel();
			collider.enabled = false;
			if (TryToEnterAreaIE != null)
			{
				StopCoroutine(TryToEnterAreaIE);
				TryToEnterAreaIE = null;
			}
			UserService.ResetState();

			// 试图加入新区域
			if (AreaManager.CurArea != targetArea)
			{
				TryToEnterAreaIE = StartCoroutine(TryToEnterArea());
			}
			yield return new WaitForSeconds(0.5f);
			for (; ; )
			{
				if (TryToEnterAreaIE == null)
					break;
			}
			// 加入区域成功时，试图加入频道
			string workSpace = userInfo.WorkSpacenNowIn.Value;
			string channelName = workSpace.GetHashCode() + "_" + targetArea.ToString();

			// 加入频道成功时，开启碰撞体
			this.SubscribeEvent<JoinChannelEvent>(EnableColliderAction);

			VCService.SwitchChannel(channelName);

			AreaSwitchIE = null;

		}

		IEnumerator TryToEnterArea()
		{
			WaitForSeconds spaceTime = new WaitForSeconds(0.34f);
			for (; ; )
			{
				if (areaManager.LocalTryToSwitchArea(localPlayer, targetArea))
					break;
				yield return spaceTime;
			}
			AreaChangeCommand cmd = new AreaChangeCommand
			{
				userID = localPlayer.GetUserID(),
				workSpace = userInfo.WorkSpacenNowIn.Value,
				area = targetArea
			};
			this.SendCommand<AreaChangeCommand>(cmd);
			userInfo.AreaStaying.Value = (int)targetArea;
			TryToEnterAreaIE = null;
		}

		void EnableColliderAction(JoinChannelEvent e) { EnableCollider(); }
		void EnableCollider()
		{
			collider.enabled = true;
			this.UnSubscribeEvent<JoinChannelEvent>(EnableColliderAction);
		}

		/// <summary>
		/// 用户进入缓冲区域
		/// </summary>
		/// <param name="uid"></param>
		void UserStayBufferArea(VC_UserInfo info)
		{
			RemoteStateCommand cmd = new RemoteStateCommand { info = info, targetState = (int)remoteTargetState.temp };
			this.SendCommand<RemoteStateCommand>(cmd);
			UserService.SetUserAlpha(info, 0);
		}
		/// <summary>
		/// 用户退出缓冲区域
		/// </summary>
		/// <param name="uid"></param>
		void UserLeaveBufferArea(VC_UserInfo info)
		{
			RemoteStateCommand cmd = new RemoteStateCommand { info = info, targetState = (int)remoteTargetState.offline };
			this.SendCommand<RemoteStateCommand>(cmd);
		}
		/// <summary>
		/// 用户进入缓冲区域
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="fadeRate"></param>
		void UserStayFadeArea(VC_UserInfo info, float fadeRate)
		{
			RemoteStateCommand cmd = new RemoteStateCommand { info = info, targetState = (int)remoteTargetState.work };
			this.SendCommand<RemoteStateCommand>(cmd);
			// 修改对应面板Alpha值
			UserService.SetUserAlpha(info, fadeRate);
		}
		/// <summary>
		/// 用户进入工作区域
		/// </summary>
		/// <param name="uid"></param>
		void UserStayWorkArea(VC_UserInfo info)
		{
			UserService.SetUserAlpha(info, 1);
			RemoteStateCommand cmd = new RemoteStateCommand { info = info, targetState = (int)remoteTargetState.work };
			this.SendCommand<RemoteStateCommand>(cmd);
		}
	}
}
