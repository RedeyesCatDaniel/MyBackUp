using agora_gaming_rtc;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 视频聊天引擎管理器
/// </summary>
namespace LGUVirtualOffice
{
	public class VCUtility : IVCUtility
	{
		// 属性
		public static IRtcEngine rtcEngine { get; private set; }
		public string AppID;
		public VC_Config config { get; private set; }
		public VC_UserInfo localPlayerInfo { get; private set; }
		public VC_UserCtr userCtr { get; private set; }


#if UNITY_EDITOR
		// 编辑器变更时，用于即时销毁引擎
		private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
		{
			switch (obj)
			{
				case PlayModeStateChange.EnteredEditMode://停止播放事件监听后被监听
					break;
				case PlayModeStateChange.ExitingEditMode://编辑转播放时监听(播放之前)
					break;
				case PlayModeStateChange.EnteredPlayMode://播放时立即监听
					break;
				case PlayModeStateChange.ExitingPlayMode://停止播放立即监听
					VCEngineDestroy();
					break;
			}

		}
#endif

		// 引擎相关
		/// <summary>
		/// 视频通话引擎初始化
		/// </summary>
		public void VCEngineInitialize()
		{
#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif
			if (rtcEngine != null)
			{
				rtcEngine = null;
				IRtcEngine.Destroy();
			}
			// 获取配置
			config = new VC_Config();
			//进行AppID获取
			AppID = config.AppID;


			rtcEngine = IRtcEngine.GetEngine(AppID);

			// 回调注册

			// 进行配置
			// 进行视频捕获设置(EnableVedio() 前)
			rtcEngine.SetCameraCapturerConfiguration(config.myCapturerConfig);
			// 进行声音传输设置
			rtcEngine.SetAudioProfile((AUDIO_PROFILE_TYPE)config.AUDIO_PROFILE_TYPE_int, (AUDIO_SCENARIO_TYPE)config.AUDIO_SCENARIO_TYPE_int);
			// 进行视频传输设置
			rtcEngine.SetVideoEncoderConfiguration(config.myEncoderConfig);
			// 默认拒绝接收音视频流
			rtcEngine.SetDefaultMuteAllRemoteVideoStreams(true);
			rtcEngine.SetDefaultMuteAllRemoteAudioStreams(true);

			// 启用音频回调
			//rtcEngine.EnableAudioVolumeIndication(500,3,false);

			// 启用视频功能
			rtcEngine.EnableVideo();
			// 开启视频观测器,直接将视频图片发送给 App，而无需经过传统的视图渲染器。
			rtcEngine.EnableVideoObserver();
			// 开启本地视频预览
			rtcEngine.StartPreview();
		}

		public IRtcEngine GetEngine()
		{
			if (rtcEngine != null)
				return rtcEngine;
			Debug.Log("引擎尚未初始化完成！");
			return null;
		}
		public void VCEngineDestroy()
		{
			rtcEngine.LeaveChannel();
			rtcEngine.DisableVideo();
			rtcEngine = null;
			IRtcEngine.Destroy();
		}

		#region 状态变化回调
		//     private void onVolumeIndication(AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume)
		//     {
		//         string str = "说话者数量为:" + speakerNumber + "; totalVolume = " + totalVolume + "；";
		//         foreach (AudioVolumeInfo speaker in speakers)
		//{
		//             str += "\n说话者音量为 => " + speaker.volume+"；";
		//}

		//         //Debug.Log(str);
		//     }




		/// <summary>
		/// 远端用户视频状态变更时
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="state"></param>
		/// <param name="reason"></param>
		/// <param name="elapsed"></param>
		private void onRemoteVideoStateChanged(uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
		{
			//string log = "视频流发生变化：";
			switch (state)
			{
				case REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STOPPED:
					//log += "！！！视频流停止！！！"; Debug.Log(log);
					break;
				case REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_STARTING:
					//log += "！！！视频流播放！！！"; Debug.Log(log);
					break;
				case REMOTE_VIDEO_STATE.REMOTE_VIDEO_STATE_FAILED:
					//log += "！！！视频流失败！！！"; Debug.Log(log);
					break;
			}

		}

		#endregion


		// 用户行为

		/// <summary>
		/// 获取连接状态，如果为1，则在空闲中
		/// </summary>
		/// <returns></returns>
		public int GetConnectionState()
		{
			return (int)rtcEngine.GetConnectionState();
		}
		public void JoinChannel(string channelName)
		{
			rtcEngine.JoinChannel(channelName);
		}
		public void LeaveChannel()
		{
			rtcEngine.LeaveChannel();
		}

		/// <summary>
		/// 设置某个用户音量大小，0-100
		/// </summary>
		public void SetUserVolume(uint uid, int volume)
		{
			rtcEngine.AdjustUserPlaybackSignalVolume(uid, volume);
		}

		/// <summary>
		/// 打开或关闭某个音频流的接收
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="isRecive"></param>
		public void EnableReciveAudio(uint uid, bool isRecive)
		{
			rtcEngine.MuteRemoteAudioStream(uid, !isRecive);
		}
		/// <summary>
		/// 打开或关闭某个视频流的接收
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="isRecive"></param>
		public void EnableReciveVedio(uint uid, bool isRecive)
		{
			rtcEngine.MuteRemoteVideoStream(uid, !isRecive);
		}
		/// <summary>
		/// 关闭本地视频推流
		/// </summary>
		public void MuteLocalVideoStream(bool isMute)
		{
			rtcEngine.MuteLocalVideoStream(isMute);
		}
		/// <summary>
		/// 关闭本地视频推流
		/// </summary>
		public void MuteLocalAudioStream(bool isMute)
		{
			rtcEngine.MuteLocalAudioStream(isMute);
		}

	}
}
