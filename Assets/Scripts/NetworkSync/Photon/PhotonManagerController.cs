using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using LGUVirtualOffice.Framework;
using ExitGames.Client.Photon;

namespace LGUVirtualOffice {
    public class PhotonManagerController: AbstractPhotonController
    {
        public static PhotonManagerController Instance;
        private string GUEST_LIST = "GuestList";
        private INetworkSyncService photonPUNService;
        #region MonoBehaviour Call Backs
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else 
            {
                Destroy(gameObject);
            }
            photonPUNService = this.GetService<INetworkSyncService>();
        }
        private void Awake()
        {
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client
            // and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        #endregion

          #region Photon callbacks
        public override void OnConnectedToMaster()
        {
            LogUtil.LogDebug("OnConnectedToMaster");
            this.SendCommand<ConnectToServerSuccessCommand>();
        }
        public override void OnCustomAuthenticationFailed(string message) 
        {
            LogUtil.LogDebug("CustomAuthenticationFailed:"+message);
            this.SendCommand<ConnectToServerFailedCommand>();
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            //PhotonNetwork.LeaveRoom();
            LogUtil.LogDebug("User " + PhotonNetwork.NickName + " Disconnected,Casue:" + cause.ToString());
            //clear cache
            AWSUtil.Instance.ClearCachedUserInfo();
            this.SendCommand<UserDisconnectFromServerCommand>();
        }
        public override void OnJoinedRoom()
        {
            LogUtil.LogDebug("OnJoinedRoom");
            this.SendCommand<JoinPhotonRoomSuccessCommand>();
            //add current user to guest list if needed
        }
        /*public override void OnLeftRoom()
        {
            LogUtil.LogDebug("OnLeftRoom");
            //remove current user from guest list if needed
        }*/
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            //update accordingly member in the members list to online mode,and if the newPlayer is a guest
            photonPUNService.UpdateRoomGuestList(newPlayer,true);
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            //when a user left room,we need to check if the user was a guest,if true,then we need to
            //check if this user was removed from the guest list.
            LogUtil.LogDebug("OnPlayerLeftRoom:" + otherPlayer.NickName);
            photonPUNService.CheckAfterOtherPlayerLeftWorkSpace(otherPlayer);
            //otherPlayer.
        }
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            //update the room state of each Team's workspace
            photonPUNService.UpdateWorkSpaceList(roomList);
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) 
        {
            photonPUNService.UpdatePlayerProperties(targetPlayer,changedProps);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(GUEST_LIST)) 
            {
                //if guest list changed,inform other users in the same room
                this.SendCommand<RoomPropertyUpdateCommand>();
            }
        }
        #endregion
    }
}
