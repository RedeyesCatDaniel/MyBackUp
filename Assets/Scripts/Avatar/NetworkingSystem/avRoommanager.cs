using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using LGUVirtualOffice;

namespace LGUVirtualOffice
{
    
    public class avRoommanager:MonoBehaviourPunCallbacks
    {
        public static string roomName = "BearTown";
        public static bool hasRoom;



        public UnityEvent DOnJoinRoom;



        public void JoinRoom() {
			RoomOptions roomOptions = new RoomOptions();
			roomOptions.MaxPlayers = 25;
			PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
			// PhotonManager.Instance.JoinWorkSpace();
		}

        public override void OnJoinedRoom()
        {
            //  base.OnJoinedRoom();
            DOnJoinRoom.Invoke();
            //Debug.Log(PhotonNetwork.CurrentRoom.Name);
        
        }

        









    }
}