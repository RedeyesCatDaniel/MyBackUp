using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avConnector : MonoBehaviourPunCallbacks
    {
        public UnityEvent OnConnectedToMain;

        public void Connect()
        {
            // PhotonNetwork.AutomaticallySyncScene = true;
            // PhotonNetwork.ConnectUsingSettings();
            Debug.Log(UserInfo.Instance.UserId);
            PhotonManager.Instance.ConnectToServer(UserInfo.Instance);
           // var v =PhotonManager.Instance;
        }

        public override void OnConnectedToMaster()
        {
            OnConnectedToMain.Invoke();
        }

        

    }
}