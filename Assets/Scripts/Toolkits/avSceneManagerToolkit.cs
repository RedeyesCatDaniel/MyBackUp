using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "Toolkit/avSceneManagerToolkit")]
    public class avSceneManagerToolkit : ScriptableObject
    {
        public static void LoadScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        public static void JoinRoom() {
            // PhotonManager.Instance.JoinWorkSpace();
            if (LGUVirtualOffice.TryGetBackDoor(out LGUVirtualOffice instance))
            {
                var connector = instance.GetService<INetworkSyncService>();
                connector.JoinWorkSpace();
            }
            else {
                Debug.Log("Office is not created");
            }
           
        }
    }
}
