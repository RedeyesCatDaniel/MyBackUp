using LGUVirtualOffice;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avPlayerMaker : MonoBehaviourPunCallbacks, IPunPrefabPool
    {
        public PhotonView[] views;
        public avAnimatorController animatorController;
        public GameObject mainController;
        //public bool MakeOnAwake;
        public UnityEvent DOnJoinRoom;
        public void Awake()
        {
   /*         if (MakeOnAwake) {
                Make();
            }
            //*/
            
        }

        public void Destroy(GameObject gameObject)
        {
          //  players.Remove(gameObject);
            MonoBehaviour.Destroy(gameObject);
        }

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {

          //  bool wasActive = view.gameObject.activeSelf;
            //if (wasActive) view.gameObject.SetActive(false);

            //GameObject instance = GameObject.Instantiate(view.gameObject, position, rotation) as GameObject;

            //
            // return instance;

            GameObject obj = Instantiate(views[0]).gameObject;
            obj.SetActive(false);
            return obj;
        }
        public override void OnJoinedRoom()
        {
            Make();
            DOnJoinRoom.Invoke();
        }


        public void Make() {
           
            PhotonNetwork.PrefabPool = this;
            GameObject obj = PhotonNetwork.Instantiate("", Vector3.zero, Quaternion.identity);
            if (obj.TryGetComponent<PhotonView>(out PhotonView view))
            {
                if (view.IsMine)
                {
                    animatorController.SetAnim(obj.GetComponent<Animator>());
                    mainController.gameObject.SetActive(true);

                }
                else{
                    Debug.Log("How could my body is not mine?");

                }
            }


        }

       
    }
}