using agora_gaming_rtc;
using LGUVirtualOffice.Framework;
using UnityEngine;

namespace LGUVirtualOffice
{
	public class VCService : AbstractService, IVCService
	{
		string targetChannelName;

		IVCUtility VCUtility;
		IDeviceManager DeviceManager;

		protected override void OnInit()
		{
			VCUtility = this.GetUtility<IVCUtility>();
			DeviceManager = this.GetService<IDeviceManager>();
		}

		public void InitailizeVCSystem()
		{
			targetChannelName = "";
			VCUtility.VCEngineInitialize();
			DeviceManager.Initailize(VCUtility.GetEngine());

			IRtcEngine rtcEngine = VCUtility.GetEngine();
			rtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
			rtcEngine.OnLeaveChannel = onLeaveChannelSuccess;
			rtcEngine.OnUserJoined = onUserJoined;
			rtcEngine.OnUserOffline = onUserOffline;
			rtcEngine.OnUserMuteVideo = onUserMuteVideo;
			//rtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStatsChanged;

		}
#region 回调事件
		public void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
		{
			Debug.Log("成功加入频道:" + channelName + "; uid = " + uid);
			targetChannelName = "";

			JoinChannelEvent e = new JoinChannelEvent
			{
				channelName = channelName,
				uid = uid
			};
			this.TriggerEvent(e);
		}
		/// <summary>
		/// 成功离开频道
		/// </summary>
		/// <param name="stats"></param>
		private void onLeaveChannelSuccess(RtcStats stats)
		{

			this.TriggerEvent<LeaveChannelEvent>(new LeaveChannelEvent());
			if (targetChannelName != "")
				SwitchChannel(targetChannelName);

			Debug.Log("成功退出频道\n(可在此处调用Log上传，如：通话时间为" + stats.duration + ")");
		}
		// 远端用户加入
		private void onUserJoined(uint uid, int elapsed)
		{
			UserJoinedEvent e = new UserJoinedEvent
			{
				uid = uid
			};
			this.TriggerEvent<UserJoinedEvent>(e);

			Debug.Log("远端用户加入: uid => " + uid);
		}
		// 远端用户离线
		private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
		{
			UserOfflineEvent e = new UserOfflineEvent
			{
				uid = uid
			};

			this.TriggerEvent<UserOfflineEvent>(e);

			Debug.Log("远端用户离线: uid = " + uid + "； reason = " + reason);

			// 试图关闭正在工作的UID窗口

		}
		// 远端用户关闭视频推流
		private void onUserMuteVideo(uint uid, bool muted)
		{
			UserMuteVideoEvent e = new UserMuteVideoEvent
			{
				uid = uid,
				isMuted = muted
			};

			this.TriggerEvent<UserMuteVideoEvent>(e);
		}
#endregion

		public void Log(string str)
		{
			Debug.Log(str);
		}

		public void PasueVCSystem()
		{
		}

		public void ResumeVCSystem()
		{
		}

		public void DestroyVCSystem()
		{
			VCUtility.VCEngineDestroy();
		}

		public IRtcEngine GetEngine()
		{
			return VCUtility.GetEngine();
		}

		public void LeaveChannel()
		{
			VCUtility.LeaveChannel();
		}

		public void SwitchChannel(string channelName)
		{
			if (VCUtility.GetConnectionState() == 1)
			{
				VCUtility.JoinChannel(channelName);
			}
			else
			{
				targetChannelName = channelName;
				VCUtility.LeaveChannel();
			}
		}

		public void MuteLocalAudioStream(bool isMute)
		{
			this.TriggerEvent<LocalPushStateEvent>(new LocalPushStateEvent { isVideo = false, isMuted = isMute });
			VCUtility.MuteLocalAudioStream(isMute);
		}

		public void SetUserVolume(uint uid, int volume)
		{
			if (uid == 0)
				return;
			VCUtility.SetUserVolume(uid, volume);
		}

		public void MuteLocalVideoStream(bool isMute)
		{
			this.TriggerEvent<LocalPushStateEvent>(new LocalPushStateEvent { isVideo = true, isMuted = isMute });
			VCUtility.MuteLocalVideoStream(isMute);
		}

		public void EnableReciveVideo(uint uid, bool isRecive)
		{
			VCUtility.EnableReciveVedio(uid, isRecive);
		}

		public void EnableReciveAudio(uint uid, bool isRecive)
		{
			VCUtility.EnableReciveAudio(uid, isRecive);
		}
	}
}
