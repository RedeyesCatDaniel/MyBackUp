using UnityEngine;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public class LGUVirtualOffice : AbstractArchitecture<LGUVirtualOffice>
	{
        //this is for make use of service outside of framework
        private static LGUVirtualOffice backDoor;

        public static bool TryGetBackDoor(out LGUVirtualOffice rs) {
            rs = backDoor;
            if (rs == null) {
                return false;
            }
            else {
                return true;
            }
        }   

        protected override void OnInit()
        {
            UtilityRegister();
            ModelRegister();
            ServiceRegister();
            
            //I added this line to open the back door;
            backDoor = this;
        }
        private void UtilityRegister() 
        {
            RegisterUtility<IMessageQueueUtility>(new AWSSQSUtil());
            RegisterUtility<IDBUtility>(new DynamoDBUtility());
            RegisterUtility<IVCUtility>(new VCUtility());
        }
        private void ModelRegister() 
        {
            RegisterModel(new UserInfo());
        }
        private void ServiceRegister() 
        {
            RegisterService<IMemberService>(new memService());
            RegisterService<ILoginService>(new LoginService());
            RegisterService<IQueueMessageService>(new QueueMessageService());
            RegisterService<INetworkSyncService>(new PhotonPUNService());
            RegisterService<IDBService>(new DynamoDBService());
            RegisterService(new Click_Login_Controller());
            RegisterService(new JumpPosition());
            RegisterService(new My_GuestList());
            RegisterService(new GetOtherListMember());
            RegisterService(new TeamMateStatusSyncService());
            RegisterService(new MessageSendService());
            RegisterService<IVCService>(new VCService());
            RegisterService<IDeviceManager>(new DeviceManager());
            RegisterService<IVCUserService>(new VCUserService());
            RegisterService<IAreaManager>(new AreaManager());
            RegisterService<IUIService>(new UIService());
        }
    }
}