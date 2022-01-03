using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice
{
    public class GetSettingData : AbstractController
    {
        public Setting.DataType dataType;
        public TMP_Dropdown dropdowns;
        public SettingData settingData;
        IDeviceManager deviceManager;
        // Start is called before the first frame update
        void Start()
        {
            
            dropdowns.onValueChanged.AddListener(OnDatachange);

        }
        private void OnEnable()
        {
            settingData = GetComponentInParent<Setting>().SettingData;
            deviceManager = this.GetService<IDeviceManager>();
            try
            {
                Init(settingData);
            }
            catch (System.Exception)
            {
                ShowData(null);
            }
            
        }
        void Init(SettingData settingData)
        {
            List<string> data;
            switch (dataType)
            {
                case Setting.DataType.loudspeaker:
                    settingData.loudspeaker = deviceManager.GetAudioPlayingDeviceList();
                    data = settingData.loudspeaker;
                    break;
                case Setting.DataType.microphone:
                    settingData.microphone = deviceManager.GetRecordingDeviceList();
                    data = settingData.microphone;
                    break;
                case Setting.DataType.camera:
                    settingData.camera = deviceManager.GetCameraDeviceList();
                    data = settingData.camera;
                    break;
                case Setting.DataType.updata_videosize:
                    data = settingData.updata_videosize;
                    break;
                case Setting.DataType.show_videosize:
                    data = settingData.show_videosize;
                    break;
                case Setting.DataType.avatarsetings:
                    data = settingData.avatarsetings;
                    break;
                default:
                    data = null;
                    break;
            }SettingData.defaultdata = settingData;
            ShowData(data);
        }

        private void ShowData( List<string> data)
        {
            
            List<TMP_Dropdown.OptionData> s = dropdowns.options;
            dropdowns.value = 0;
            s.Clear();
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    s.Add(new TMP_Dropdown.OptionData());
                    s[i].text = data[i];
                }
            }
        }

        void OnDatachange(int index) 
        {
            
            switch (dataType)
            {
                case Setting.DataType.loudspeaker:
                    Setloudspeaker(index);
                    break;
                case Setting.DataType.microphone:
                    Setmicrophone(index);
                    break;
                case Setting.DataType.camera:
                    Setcamera(index);
                    break;
                case Setting.DataType.updata_videosize:
                    Setupdata_videosize(index);
                    break;
                case Setting.DataType.show_videosize:
                    Setshow_videosize(index);
                    break;
                case Setting.DataType.avatarsetings:
                    LogUtil.LogDebug(settingData.avatarsetings[index]);
                    break;
                default:
                    break;
            }
        }
        void Setmicrophone(int index)
        {
            if (deviceManager.SetRecordingDevice(settingData.microphone[index])) 
            {
                LogUtil.LogInfo("Set microphone Success");
            }

           
        }
        void Setloudspeaker(int index)
        {
            if (deviceManager.SetAudioPlayingDevice(settingData.loudspeaker[index]))
            {
                LogUtil.LogInfo("Set Loadspeaker Success");
            }
        }
        void Setcamera(int index)
        {
            if (deviceManager.SetCameraDevice(settingData.camera[index]))
            {
                LogUtil.LogInfo("Set Camera Success");
            }
        }
        void Setupdata_videosize(int index)
        {
            LogUtil.LogInfo(settingData.updata_videosize[index]);
        }
        void Setshow_videosize(int index)
        {
            LogUtil.LogInfo(settingData.show_videosize[index]);
        }

    }
}