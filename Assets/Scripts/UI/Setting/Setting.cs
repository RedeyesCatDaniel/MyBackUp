using UnityEngine.UI;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice
{
    public class Setting : AbstractController

    {
        IDeviceManager deviceManager;


        public enum DataType
        {
            loudspeaker, microphone, camera, updata_videosize, show_videosize, avatarsetings
        }
        //public List<GetSetData> getSetDatas;
        public Button button;
        private SettingData settingData;
        public SettingData SettingData { get 
            {
                if (settingData==null)
                {
                    settingData = new SettingData();
                }
                return settingData;
            } }
        // Start is called before the first frame update
        void Awake()
        {
            deviceManager = this.GetService<IDeviceManager>();
            button.onClick.AddListener(Save);
        }
       
        void Save()
        {
            gameObject.SetActive(false);
        }
    }
}