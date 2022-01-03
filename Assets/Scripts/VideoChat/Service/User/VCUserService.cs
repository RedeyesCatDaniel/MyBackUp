using LGUVirtualOffice.Framework;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 管理一个频道的用户数据
/// </summary>
namespace LGUVirtualOffice
{
	public class VCUserService : AbstractService, IVCUserService
	{
		public VC_UI_Controller UI_Controller;
		public IVCService VCService;
		public IVCUtility VCUtility;

		// 用户表
		VC_UserInfo localUserInfo;
		public List<VC_UserInfo> TempUser { get; private set; }
		public List<VC_UserInfo> WorkUser { get; private set; }
		public List<VC_UserInfo> AllUser { get; private set; }
		public Dictionary<VC_UserInfo, float> Dic_UserAlpha { get; private set; }

		// 初始化
		protected override void OnInit()
		{
			VCService = this.GetService<IVCService>();
			VCUtility = this.GetUtility<IVCUtility>();

			AllUser = new List<VC_UserInfo>();

			ResetState();

			// 订阅事件
			this.SubscribeEvent<PlayerCreatSuccessedEvent>((e) => { this.SetLocalInfo(e.localInfo); });

			this.SubscribeEvent<JoinChannelEvent>((e) =>
			{
				VCService.MuteLocalVideoStream(!GetLocalPushState(true));
				VCService.MuteLocalAudioStream(!GetLocalPushState(false));
			});
			this.SubscribeEvent<LeaveChannelEvent>((e) => ResetState());
			this.SubscribeEvent<UserJoinedEvent>((e) =>
			{
				if (AreaManager.CurRule == VCRuleEnum.AutoConnect)
				{
					VC_UserInfo info = GetUserInfo(e.uid);
					if (info == null) return;
					PutInWork(info);
					SetUserAlpha(info, 1);
				}
			});
			this.SubscribeEvent<UserOfflineEvent>((e) => CheckStream());
			//this.SubscribeEvent<UserMuteVideoEvent>((e) => OnUserMute(e.uid, e.isMuted));

			this.SubscribeEvent<StreamStateEvent>((e) => StreamChanged(e));
			// 视频信息发生改变时
			this.SubscribeEvent<AgoraInfoChangeEvent>((e) => VCInfoChange(e));
			// 加载/卸载 VC_Info
			this.SubscribeEvent<VCUserInfoUpdateEvent>((e) => LoadInfo(e.info));
			this.SubscribeEvent<PlayerQuitEvent>((e) => UnLoadInfo(e.info));
		}


		#region Private Function
		// 处理玩家视频信息变更信息
		void VCInfoChange(AgoraInfoChangeEvent e)
		{
			LoadInfo(e.info);
			CheckStream();
		}
		// 判断用户是否需要分配窗口
		bool IsInfoActive(VC_UserInfo info)
		{
			if (info == null) return false;
			if (info.gameObject == null) return false;
			if (info.GetArea() != localUserInfo.GetArea()) return false;
			if (info.GetChannel() != localUserInfo.GetChannel()) return false;
			return true;
		}

		// 处理玩家距离消息
		void StreamChanged(StreamStateEvent e)
		{
			if (localUserInfo.GetArea() == WorkspaceAreaEnum.Default)
				return;
			VC_UserInfo info = e.info;
			switch (e.state)
			{
				case 0:
					LeaveAround(info);
					break;
				case 1:
					PutInTemp(info);
					break;
				case 2:
					PutInWork(info);
					break;
			}
		}

		#endregion


		public VC_UserInfo GetUserInfo(uint uid)
		{
			foreach (VC_UserInfo info in AllUser)
			{
				if (info.GetUID() == uid)
					return info;
			}
			return null;
		}

		// 重置状态
		public void ResetState()
		{
			TempUser = new List<VC_UserInfo>();
			WorkUser = new List<VC_UserInfo>();
			Dic_UserAlpha = new Dictionary<VC_UserInfo, float>();
			if (UI_Controller != null) UI_Controller.ResetPanel();
		}

		public void LoadInfo(VC_UserInfo info)
		{
			if (info.isLocalPlayer)
				localUserInfo = info;
			else
			{
				if (AllUser.Contains(info))
				{
					return;
				}
			}
		}
		public void UnLoadInfo(VC_UserInfo info)
		{
			Debug.LogError(info.GetName() + "退出房间");
			if (AllUser.Contains(info))
			{
				AllUser.Remove(info);
			}
		}
		/// <summary>
		/// 添加一个用户至缓冲区
		/// </summary>
		/// <param name="uid"></param>
		public void PutInTemp(VC_UserInfo info)
		{
			if (TempUser.Contains(info)) return;
			if (WorkUser.Contains(info)) WorkUser.Remove(info);
			if (!TempUser.Contains(info))
			{
				// 进入暂存区
				TempUser.Add(info);
				UI_Controller.CheckQueue();
				CheckStream();
			}
		}
		/// <summary>
		/// 添加一个用户至工作区
		/// </summary>
		/// <param name="uid"></param>
		public void PutInWork(VC_UserInfo info)
		{
			if (WorkUser.Contains(info)) return;
			if (TempUser.Contains(info)) TempUser.Remove(info);
			if (!WorkUser.Contains(info))
			{
				// 进入工作区
				WorkUser.Add(info);
				UI_Controller.CheckQueue();
				CheckStream();
			}
		}
		/// <summary>
		/// 离开周围区域
		/// </summary>
		/// <param name="uid"></param>
		public void LeaveAround(VC_UserInfo info)
		{
			if (WorkUser.Contains(info)) WorkUser.Remove(info);
			if (TempUser.Contains(info)) TempUser.Remove(info);
			CheckStream();
			UI_Controller.UserLeaveAround(info);
		}
		/// <summary>
		/// 切换工作队列顺序
		/// </summary>
		public void ScrollQueue(bool isScrollRight)
		{
			// 对此面板进行数据调整
			VC_UserInfo tempUser;
			if (isScrollRight)
			{
				tempUser = WorkUser[0];
				WorkUser.Remove(tempUser);
				WorkUser.Add(tempUser);
			}
			else
			{
				tempUser = WorkUser[WorkUser.Count - 1];
				WorkUser.Remove(tempUser);
				WorkUser.Insert(0, tempUser);
			}
		}


		/// <summary>
		/// 查询一个用户的音视频接收状态
		/// </summary>
		/// <returns></returns>
		public bool GetLocalPushState(bool isVideo)
		{
			if (isVideo)
				return !localUserInfo.isVideoMuted;
			else
				return !localUserInfo.isAudioMuted;
		}

		/// <summary>
		/// 调整一个用户的渐隐系数
		/// </summary>
		public void SetUserAlpha(VC_UserInfo info, float alpha)
		{
			if (!Dic_UserAlpha.ContainsKey(info))
				Dic_UserAlpha.Add(info, alpha);
			else
				Dic_UserAlpha[info] = alpha;
		}
		/// <summary>
		/// 获取一个用户的渐隐系数
		/// </summary>
		public float GetUserAlpha(VC_UserInfo info)
		{
			if (!Dic_UserAlpha.ContainsKey(info))
				Dic_UserAlpha.Add(info, 1);
			return Dic_UserAlpha[info];
		}

		/// <summary>
		/// 检测推流接收情况
		/// </summary>
		public void CheckStream()
		{
			// 剔除频道不对的UID
			for (int i = 0; i < WorkUser.Count; i++)
			{
				if (!IsInfoActive(WorkUser[i]))
				{
					UI_Controller.SetWindowFree(WorkUser[i]);
					WorkUser.RemoveAt(i--);
				}
			}
			for (int i = 0; i < TempUser.Count; i++)
			{
				if (!IsInfoActive(TempUser[i]))
				{
					UI_Controller.SetWindowFree(TempUser[i]);
					WorkUser.RemoveAt(i--);
				}
			}

			UI_Controller.CheckQueue();
		}

		public List<VC_UserInfo> GetWorkUsers()
		{
			return WorkUser;
		}

		public List<VC_UserInfo> GetTempUsers()
		{
			return TempUser;
		}

		public void SetLocalInfo(VC_UserInfo info)
		{
			localUserInfo = info;
		}

		public VC_UserInfo GetLocalInfo()
		{
			return localUserInfo;
		}

		public void SetUIController(VC_UI_Controller ctr)
		{
			this.UI_Controller = ctr;
		}
	}
}
