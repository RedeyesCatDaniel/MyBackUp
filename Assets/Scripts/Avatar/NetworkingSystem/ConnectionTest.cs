using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bear
{
    public class ConnectionTest : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            PhotonNetwork.GameVersion = "0.0.1";
            PhotonNetwork.ConnectUsingSettings();
        }
        public override void OnConnected()
        {
            print("I am connected");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            print("I am disconnected");
        }
    }
}