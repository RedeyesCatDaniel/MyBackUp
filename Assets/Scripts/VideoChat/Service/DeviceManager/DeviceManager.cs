using agora_gaming_rtc;
using LGUVirtualOffice.Framework;
using System.Collections.Generic;

/// <summary>
/// 视频聊天引擎管理器
/// </summary>
namespace LGUVirtualOffice
{
	public class DeviceManager : AbstractService, IDeviceManager
	{
		// 视频
		IVideoDeviceManager CamMana;
		Dictionary<string, string> Dic_CamNameID;
		// 录音
		IAudioRecordingDeviceManager RecMana;
		Dictionary<string, string> Dic_RecNameID;
		// 播放
		IAudioPlaybackDeviceManager APMana;
		Dictionary<string, string> Dic_APNameID;

		protected override void OnInit()
		{
		}

		public void Initailize(IRtcEngine rtcEngine)
		{
			CamMana = rtcEngine.GetVideoDeviceManager();
			RecMana = rtcEngine.GetAudioRecordingDeviceManager();
			APMana = rtcEngine.GetAudioPlaybackDeviceManager();
		}

		// 摄像
		/// <summary>
		/// 获取摄像头设备名称列表，当前设备为第一位，如果没有设备返还null；
		/// </summary>
		/// <returns>摄像头设备名称列表，如果没有设备返还null</returns>
		public List<string> GetCameraDeviceList()
		{
			CamMana.CreateAVideoDeviceManager();

			List<string> DeviceNameList = new List<string>();
			Dic_CamNameID = new Dictionary<string, string>();

			string curID = "";
			CamMana.GetCurrentVideoDevice(ref curID);

			for (int i = 0; i < CamMana.GetVideoDeviceCount(); i++)
			{
				string ID = "";
				string Name = "";
				CamMana.GetVideoDevice(i, ref Name, ref ID);
				if (ID == curID)
					DeviceNameList.Insert(0, Name);
				else
					DeviceNameList.Add(Name);
				Dic_CamNameID.Add(Name, ID);
			}

			if (DeviceNameList.Count == 0)
				return null;
			return DeviceNameList;
		}
		/// <summary>
		/// 设置目标摄像头设备
		/// </summary>
		/// <param name="DeviceName">设备名称</param>
		/// <returns>是否成功</returns>
		public bool SetCameraDevice(string DeviceName)
		{
			bool isSuccessed = false;
			if (Dic_CamNameID.ContainsKey(DeviceName))
			{
				string ID = Dic_CamNameID[DeviceName];
				if (CamMana.SetVideoDevice(ID) == 0)
					isSuccessed = true;
			}
			return isSuccessed;
		}

		// 录音
		/// <summary>
		/// 获取录音设备名称列表，当前设备为第一位，如果没有设备返还null；
		/// </summary>
		/// <returns>录音设备名称列表，如果没有设备返还null</returns>
		public List<string> GetRecordingDeviceList()
		{
			RecMana.CreateAAudioRecordingDeviceManager();

			List<string> DeviceNameList = new List<string>();
			Dic_RecNameID = new Dictionary<string, string>();

			string curID = "";
			RecMana.GetCurrentRecordingDevice(ref curID);

			for (int i = 0; i < RecMana.GetAudioRecordingDeviceCount(); i++)
			{
				string ID = "";
				string Name = "";
				RecMana.GetAudioRecordingDevice(i, ref Name, ref ID);
				if (ID == curID)
					DeviceNameList.Insert(0, Name);
				else
					DeviceNameList.Add(Name);
				Dic_RecNameID.Add(Name, ID);
			}

			if (DeviceNameList.Count == 0)
				return null;
			return DeviceNameList;
		}
		/// <summary>
		/// 设置目标录音设备
		/// </summary>
		/// <param name="DeviceName">设备名称</param>
		/// <returns>是否成功</returns>
		public bool SetRecordingDevice(string DeviceName)
		{
			bool isSuccessed = false;
			if (Dic_RecNameID.ContainsKey(DeviceName))
			{
				string ID = Dic_RecNameID[DeviceName];
				if (RecMana.SetAudioRecordingDevice(ID) == 0)
					isSuccessed = true;
			}
			return isSuccessed;
		}

		// 播放
		/// <summary>
		/// 获取播放设备名称列表，当前设备为第一位，如果没有设备返还null；
		/// </summary>
		/// <returns>播放设备名称列表，如果没有设备返还null</returns>
		public List<string> GetAudioPlayingDeviceList()
		{
			APMana.CreateAAudioPlaybackDeviceManager();

			List<string> DeviceNameList = new List<string>();
			Dic_APNameID = new Dictionary<string, string>();

			string curID = "";
			APMana.GetCurrentPlaybackDevice(ref curID);

			for (int i = 0; i < APMana.GetAudioPlaybackDeviceCount(); i++)
			{
				string ID = "";
				string Name = "";
				APMana.GetAudioPlaybackDevice(i, ref Name, ref ID);
				if (ID == curID)
					DeviceNameList.Insert(0, Name);
				else
					DeviceNameList.Add(Name);
				Dic_APNameID.Add(Name, ID);
			}

			if (DeviceNameList.Count == 0)
				return null;
			return DeviceNameList;
		}
		/// <summary>
		/// 设置目标播放设备
		/// </summary>
		/// <param name="DeviceName">设备名称</param>
		/// <returns>是否成功</returns>
		public bool SetAudioPlayingDevice(string DeviceName)
		{
			bool isSuccessed = false;
			if (Dic_APNameID.ContainsKey(DeviceName))
			{
				string ID = Dic_CamNameID[DeviceName];
				if (APMana.SetAudioPlaybackDevice(ID) == 0)
					isSuccessed = true;
			}
			return isSuccessed;
		}

		/// <summary>
		/// 测试用，一键快速切换摄像设备
		/// </summary>
		public void QuickSwitchCamDevice()
		{
			List<string> camDevicesList = GetCameraDeviceList();
			if (camDevicesList.Count > 1)
			{
				SetCameraDevice(camDevicesList[1]);
			}
		}
	}
}
