using UnityEngine;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice 
{
    public abstract class AbstractController : MonoBehaviour, IController
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
        IArchitecture ICanGetArchitecture.GetArchitecture()
        {
            return architecture;
        }
    }
}
