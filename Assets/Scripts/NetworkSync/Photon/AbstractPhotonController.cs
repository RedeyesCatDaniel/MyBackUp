using UnityEngine;
using LGUVirtualOffice.Framework;
using Photon.Pun;
namespace LGUVirtualOffice {
	public class AbstractPhotonController : MonoBehaviourPunCallbacks,IController
	{
        private static IArchitecture architecture;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitArchitecture()
        {
            LGUVirtualOffice.SubscribeRegisterpatch((instance) =>
            {
                architecture = instance;
            }, true);
        }

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            throw new System.NotImplementedException();
        }

        IArchitecture ICanGetArchitecture.GetArchitecture()
        {
            return architecture;
        }
    }
}