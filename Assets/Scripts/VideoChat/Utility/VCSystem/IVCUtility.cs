using agora_gaming_rtc;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public interface IVCUtility : IUtility
	{
		public IRtcEngine GetEngine();
		public int GetConnectionState();
		public void JoinChannel(string channelName);
		public void LeaveChannel();
		public void VCEngineInitialize();
		public void VCEngineDestroy();
		public void SetUserVolume(uint uid, int volume);
		public void EnableReciveAudio(uint uid, bool isRecive);
		public void EnableReciveVedio(uint uid, bool isRecive);
		public void MuteLocalVideoStream(bool isMute);
		public void MuteLocalAudioStream(bool isMute);
	}
}
