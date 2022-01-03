using agora_gaming_rtc;
using LGUVirtualOffice.Framework;
using System.Collections.Generic;

namespace LGUVirtualOffice
{
	public interface IDeviceManager : IService
	{


		public void Initailize(IRtcEngine rtcEngine);

		// 摄像
		public List<string> GetCameraDeviceList();
		/// <summary>
		/// 设置目标摄像头设备
		/// </summary>
		/// <param name="DeviceName">设备名称</param>
		/// <returns>是否成功</returns>
		public bool SetCameraDevice(string DeviceName);

		// 录音
		/// <summary>
		/// 获取录音设备名称列表，当前设备为第一位，如果没有设备返还null；
		/// </summary>
		/// <returns>录音设备名称列表，如果没有设备返还null</returns>
		public List<string> GetRecordingDeviceList();
		/// <summary>
		/// 设置目标录音设备
		/// </summary>
		/// <param name="DeviceName">设备名称</param>
		/// <returns>是否成功</returns>
		public bool SetRecordingDevice(string DeviceName);

		// 播放
		/// <summary>
		/// 获取播放设备名称列表，当前设备为第一位，如果没有设备返还null；
		/// </summary>
		/// <returns>播放设备名称列表，如果没有设备返还null</returns>
		public List<string> GetAudioPlayingDeviceList();
		/// <summary>
		/// 设置目标播放设备
		/// </summary>
		/// <param name="DeviceName">设备名称</param>
		/// <returns>是否成功</returns>
		public bool SetAudioPlayingDevice(string DeviceName);


		public void QuickSwitchCamDevice();
	}
}
