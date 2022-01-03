using System.Collections;
using System.Collections.Generic;
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace LGUVirtualOffice {
    /// <summary>
    /// first ConnectToServer,if the result is true,then you can JoinWorkSpace
    /// </summary>
    [Obsolete("Deprecated,Please Use PhotonPUNService")]
    public class PhotonManager:MonoBehaviourPunCallbacks
    {
        public static PhotonManager Instance;
        public static PhotonView myPhotonView;
        //worksapce list
        public Dictionary<string,WorkSpaceModel> WorkSpaceList { get; private set; }
        //favorite workspace list
        //Team member list
        //add and delete favorite workspace

        private bool isConnecting;
        private bool isInRoom;
        private bool leaveRoom;
        private bool connectingToMatsterServer;
        private byte maxPlayerPerRoom=25;
        private string TEAM_CODE_PROPERTY = "TeamCode";
        private string TEAM_NAME_PROPERTY = "TeamName";
        private string USER_ID_PROPERTY = "UserId";
        private DateTime operationTime;
        //seconds to wait for build internet connection
        private int waitForConnection = 10;


        #region MonoBehaviour Call Backs
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            //TestEvent.Register(()=> {
            //    print("TestEvent triggered");
            //    JoinWorkSpace();
            // });
        }
        private void Awake()
        {
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client
            // and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            WorkSpaceList = new Dictionary<string, WorkSpaceModel>();
        }
        #endregion

        #region public method region

        public ServerReturnModel<bool> ConnectToServer(UserInfo userInfo)
        {
            ServerReturnModel<bool> result = new ServerReturnModel<bool>();
            if (!CheckLoginState(userInfo.UserId))
            {
                result.TriggerOnFailed(ReturnMessageConst.log_Status_Wrong);
                return result;
            }
            leaveRoom = false;
            if (!isConnecting)
            {
                //init the workspace list
                InitWorkSpaceList();
                if (!PhotonNetwork.IsConnected && !PhotonNetwork.ConnectUsingSettings())
                {
                    result.TriggerOnFailed(ReturnMessageConst.photon_Connect_Fail);
                    return result;
                }
                isConnecting = true;
            }
            PhotonNetwork.LocalPlayer.NickName = userInfo.UserName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
                { "TeamCode",userInfo.TeamInfo.TeamCode},
                { "TeamName",userInfo.TeamInfo.TeamName},
                { "UserId",userInfo.UserId}
            });
            StartCoroutine(CheckState(result));
            return result;
        }
        IEnumerator CheckState(ServerReturnModel<bool> result) 
        {
            while (!connectingToMatsterServer) 
            {
                yield return null;
            }
            result.TriggerOnSuccess(true);
        }
        public void DisConnectFromPhontonServer() {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        public void SetPlayerAgoraInfo(string agoraInfo) 
        {
            Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            if (playerProperties.ContainsKey("AgoraInfo"))
            {
                playerProperties["AgoraInfo"] = agoraInfo;
            }
            else 
            {
                playerProperties.Add("AgoraInfo", agoraInfo);
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        public string GetPlayerAgoraInfo(PhotonView playerPhotonView) 
        {
            if (playerPhotonView.Owner.CustomProperties.TryGetValue("AgoraInfo", out object value))
			{
                return value.ToString();
            }
            return null;
        }

        public ServerReturnModel<bool> JoinWorkSpace(string workSpaceName = null)
        {
            ServerReturnModel<bool> result = new ServerReturnModel<bool>();
            //if the same room,do nothing
            /*if (!isConnecting || !CheckLoginState())
            {
                result.TriggerOnFailed(ReturnMessageConst.log_Status_Wrong);
                return result;
            }*/
            if (!CheckLoginState())
            {
                result.TriggerOnFailed(ReturnMessageConst.log_Status_Wrong);
                return result;
            }
            if (IsSameRoom(workSpaceName))
            {
                result.TriggerOnFailed(ReturnMessageConst.photon_Same_Room);
                return result;
            }
            if (!CheckIfRoomExist(workSpaceName))
            {
                result.TriggerOnFailed(ReturnMessageConst.photon_Room_Not_Exist);
                return result;
            }
            /*if (CheckIfRoomFull(workSpaceName))
            {
                result.TriggerOnFailed(ReturnMessageConst.photon_Room_Full);
                return result;
            }*/
            StartCoroutine(SwitchWorkSpace(workSpaceName,result));
            return result;
        }
        #endregion

        #region coroutines
        //After connected to the master server,then we can join or create a room
        IEnumerator SwitchWorkSpace(string workSpaceName, ServerReturnModel<bool> result) 
        {
            bool switchResult = false;
            //if you are in a room,you neen to first leave the current room
            if (isInRoom) 
            {
                LeaveRoom();
            }
            operationTime = DateTime.Now.AddSeconds(waitForConnection);
            //only when you are connected to the Master server, you can join a room,otherwise you need to wait
            while (!connectingToMatsterServer&&operationTime.CompareTo(DateTime.Now)>0) 
            {
                yield return new WaitForSeconds(0.5f);//wait for a frame
            }
            //if join room failed,do nothing
            if (!isInRoom&&connectingToMatsterServer) 
            {
                if (JoinRoom(workSpaceName)) 
                {
                    operationTime = DateTime.Now.AddSeconds(waitForConnection);
                    while (!isInRoom&&operationTime.CompareTo(DateTime.Now)>0)
                    {
                        yield return new WaitForSeconds(0.5f);//wait for a frame
                    }
                    if (isInRoom)
                    {
                        // LoadWorkSpaceScene();
                        switchResult = true;
                    }
                }
            }
            if (switchResult)
            {
                result.TriggerOnSuccess(true);
            }
            else 
            {
                result.TriggerOnFailed(ReturnMessageConst.photon_Connect_Fail);
            }
        }
        #endregion
        #region Photon callbacks
        // this case where isConnecting is false is typically when you lost or quit the game,
        // when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        public override void OnConnectedToMaster()
        {
            print("PhotonManager OnConnectedToMaster");
            connectingToMatsterServer = true;
            isInRoom = false;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            connectingToMatsterServer = false;
            isConnecting = false;
            isInRoom = false;
            if (isInRoom) 
            {
                PhotonNetwork.LeaveRoom();
            }
            StopAllCoroutines();
            LogUtil.LogInfo("User " + PhotonNetwork.NickName + " Disconnected,Casue:" + cause.ToString());
            //LoadLoginScene();
        }
        public override void OnJoinedRoom()
        {
            LogUtil.LogDebug("PhotonManager OnJoinedRoom!");
            isInRoom = true;
            connectingToMatsterServer = false;
        }
        public override void OnLeftRoom()
        {
            isInRoom = false;
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            //update accordingly member in the members list to online mode,and if the newPlayer is a guest
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            LogUtil.LogDebug("OnPlayerLeftRoom:"+otherPlayer.NickName);
            //otherPlayer.
        }
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            //update the room state of each Team's workspace
            foreach (var item in WorkSpaceList)
            {
                item.Value.IsOnline = false;
            }
            if (roomList == null) 
            {
                return;
            }
            WorkSpaceModel workSpace;
            foreach (var item in roomList)
            {
                if (WorkSpaceList.TryGetValue(item.Name, out workSpace)) 
                {
                    workSpace.IsOnline = true;
                }
            }
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!targetPlayer.IsLocal) 
            {
                targetPlayer.CustomProperties=changedProps;
            }
        }
        #endregion

        #region private method region

        private bool CheckLoginState(string userId=null) 
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetPlayerProperty(PhotonNetwork.LocalPlayer, USER_ID_PROPERTY);
            }
            bool result = !string.IsNullOrEmpty(AWSUtil.Instance.GetIdToken(userId));
            return result;
        }
        private bool JoinRoom(string workSpaceName=null) 
        {
            LogUtil.LogDebug("PhotonManager JoinRoom");
            //TODO Members List and guest
            string teamName = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            //only team members can create workspace room.
            if (string.IsNullOrEmpty(workSpaceName)||workSpaceName.Equals(teamName))
            {
                return JoinOrCreateRoom();
            }
            else 
            {//guests can only join the room
                return PhotonNetwork.JoinRoom(workSpaceName);
            }
        }
        private bool JoinOrCreateRoom() 
        {
            string teamName = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            string teamCode = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_CODE_PROPERTY);
            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = maxPlayerPerRoom,
                PlayerTtl = 0,
                EmptyRoomTtl = 0,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
                {
                    { TEAM_CODE_PROPERTY,teamCode}
                }
            };
            bool joinResult = PhotonNetwork.JoinOrCreateRoom(teamName, roomOptions, TypedLobby.Default);
            return joinResult;
        }
        private void LoadWorkSpaceScene()
        {
            if (isConnecting && isInRoom)
            {
                print("LoadWorkSpaceScene");
                //LOAD WORKSPACE SCENE
                //We only load if we are the first player,
                //else we rely on PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    PhotonNetwork.LoadLevel("CharacterCustomization");
                }
                //PhotonNetwork.Instantiate("Cube",Vector3.zero,Quaternion.identity);
            }
        }
        private void LoadLoginScene() 
        {
            //LGUSceneManager.Instance.LoadLoginScene();
        }
        private bool LeaveRoom() 
        {
            return PhotonNetwork.LeaveRoom();
        }
        private string GetPlayerProperty(Player player, string propertyName)
        {
            return player.CustomProperties[propertyName] as string;
        }
        private bool IsSameRoom(string roomName) 
        {
            if (!isInRoom) 
            {
                return false;
            }
            if (string.IsNullOrEmpty(roomName)) 
            {
                roomName=GetPlayerProperty(PhotonNetwork.LocalPlayer,TEAM_NAME_PROPERTY);
            }
            return PhotonNetwork.CurrentRoom.Name.Equals(roomName);
        }
        private bool CheckIfRoomExist(string workSpaceName) 
        {
            string teamName = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            if (string.IsNullOrEmpty(workSpaceName)||workSpaceName.Equals(teamName))
            {//no need to check  existence of own team workspace
                return true;
            }
            WorkSpaceModel workSpace;
            if (!WorkSpaceList.TryGetValue(workSpaceName, out workSpace)) 
            {
                return false;
            }
            if (!workSpace.IsOnline) 
            {
                return false;
            }
            return true;
        }
        private bool CheckIfRoomFull(string workSpaceName) 
        {
            foreach (var item in WorkSpaceList)
            {
                print(item.Key+","+item.Value);
            }
            if (string.IsNullOrEmpty(workSpaceName)) 
            {
                workSpaceName=GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            }
            print("workSpaceName=="+ workSpaceName);
            return WorkSpaceList[workSpaceName].IsFull;
        }
        private void InitWorkSpaceList() 
        {
            //achieve the team list
            var result = logDBManager.Instance.GetTeamList();
            result.OnFailed((s)=> { print("InitWorkSpaceList failed"); });
            result.OnSuccess((teamList)=> {
                print("InitWorkSpaceList success"+teamList.Count);
                foreach (var item in teamList)
                {
                    WorkSpaceList.Add(item.TeamName, new WorkSpaceModel()
                    {
                        IsOnline = false,
                        Name = item.TeamName,
                        TeamCode = item.TeamCode
                    });
                }
            }) ;
        }
        #endregion
    }
}
