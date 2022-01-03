using agora_gaming_rtc;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public interface IVCService : IService
	{
		public IRtcEngine GetEngine();
		public void InitailizeVCSystem();
		public void PasueVCSystem();
		public void ResumeVCSystem();
		public void DestroyVCSystem();
		//
		public void LeaveChannel();
		public void SwitchChannel(string chennelName);
		public void MuteLocalVideoStream(bool isMute);
		public void MuteLocalAudioStream(bool isMute);
		public void SetUserVolume(uint uid, int volume);
		public void EnableReciveVideo(uint uid, bool isRecive);
		public void EnableReciveAudio(uint uid, bool isRecive);

		public void Log(string str);

	}
}
