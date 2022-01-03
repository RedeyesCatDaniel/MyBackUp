using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ExitGames.Client.Photon;
using LGUVirtualOffice.Framework;
using Photon.Pun;
using Photon.Realtime;
using HashTable = ExitGames.Client.Photon.Hashtable;

namespace LGUVirtualOffice {
    public class PhotonPUNService : AbstractService, INetworkSyncService
    {
        public Dictionary<string, WorkSpaceModel> WorkSpaceList { get; private set; }
        //key:userId,the cache of one user will be removed when the user disconnect from the server
        private Dictionary<string,UserInfo> currentWorkSpaceGuestPool;
        //key:userId,value:teamCode
        private Dictionary<string,string> currentWorkSpaceGuestList;

        private Dictionary<string,Timer> offlineUserCache = new Dictionary<string, Timer>();
        private bool isConnecting;
        private bool isInRoom;
        private bool leaveRoom = false;
        private byte maxPlayerPerRoom = 25;
        private string JoiningWorkSpaceName;
        private string TEAM_CODE_PROPERTY = "TeamCode";
        private string TEAM_NAME_PROPERTY = "TeamName";
        private string USER_ID_PROPERTY = "UserId";
        private string GUEST_LIST = "GuestList";
        private string Auth_MD5_Salt_Key = "LGU_VERTUAL_OFFICE_PHOTON_SERVER";
        private IQueueMessageService queueMessageService;
        private IDBService dBService;
        //local user
        UserInfo userInfo;
        protected override void OnInit()
        {
            this.SubscribeEvent<ConnectToServerSuccessEvent>(OnConnectToPhotonServerSuccess);
            this.SubscribeEvent<JoinPhotonRoomSuccessEvent>(OnJoinedPhotonRoom);
            this.SubscribeEvent<UserJoinWorkSpaceEvent>(OnUserJoinWorkSpace);
            this.SubscribeEvent<RoomPropertyUpdateEvent>(OnRoomPropertyUpdate);
            this.SubscribeEvent<UserDisconnectFromServerEvent>(OnUserDisconnectFromServer);
            //signature
            //userstate
            queueMessageService = this.GetService<IQueueMessageService>();
            dBService = this.GetService<IDBService>();
            userInfo = this.GetModel<UserInfo>();
            WorkSpaceList = new Dictionary<string, WorkSpaceModel>();
            currentWorkSpaceGuestList = new Dictionary<string, string>();
            currentWorkSpaceGuestPool = new Dictionary<string, UserInfo>();
        }


        #region public method region
        public void ConnectToServer(UserInfo userInfo)
        {
            if (!CheckLoginState(userInfo.UserId))
            {
                this.TriggerEvent<ConnectToServerFailedEvent>();
                return;
            }
            leaveRoom = false;
            if (!isConnecting)
            {
                InitWorkSpaceList();
                if (!PhotonNetwork.IsConnected)
                {
                    ConnectToServer(userInfo.UserId);
                }
            }
        }

        public bool IsChangeWorkspace() 
        {
            return leaveRoom;
        }

        public void SetPlayerVCInfo(string VCInfo)
        {
            Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            if (playerProperties.ContainsKey("VCInfo"))
            {
                playerProperties["VCInfo"] = VCInfo;
            }
            else 
            {
                playerProperties.Add("VCInfo", VCInfo);
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        public string GetPlayerVCInfo(PhotonView playerPhotonView)
        {
            if (playerPhotonView.Owner.CustomProperties.TryGetValue("VCInfo", out object value))
            {
                return value.ToString();
            }
            return "";
        }

        public void DisConnectFromPhontonServer()
        {
            isConnecting = false;
            if (isInRoom) 
            {
                isInRoom = false;
                PhotonNetwork.LeaveRoom();
            } 
            PhotonNetwork.Disconnect();
            queueMessageService.PushEventMessage(new UserDisconnectFromServerEvent { 
                UserId=userInfo.UserId
            },null);
        }

        public void JoinWorkSpace(string workSpaceName = null)
        {
            if (!isConnecting || !CheckLoginState())
            {
                this.TriggerEvent<JoinWorkSpaceFailedEvent>();
                return;
            }
            //if the same room,do nothing
            if (IsSameRoom(workSpaceName))
            {
                return;
            }
            if (!CheckIfRoomExist(workSpaceName))
            {
                this.TriggerEvent<JoinWorkSpaceFailedEvent>();
                return;
            }
            if (CheckIfRoomFull(workSpaceName))
            {
                this.TriggerEvent<JoinWorkSpaceFailedEvent>();
                return;
            }
            JoiningWorkSpaceName = workSpaceName;
            if (isInRoom)
            {
                LeaveRoom();
            }
            else 
            {
                JoinRoom(JoiningWorkSpaceName);
            }
            return;
        }

        public void UpdateWorkSpaceList(List<RoomInfo> roomList) 
        {
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
        public void CheckAfterOtherPlayerLeftWorkSpace(Player otherPlayer) 
        {
            //remove the player from current room guest list if necessary
            DealWithWorkSpaceGuestList(otherPlayer,false);
            //check if need to change player's login state
            DealWithUserLoginState(otherPlayer);
        }
        public void UpdatePlayerProperties(Player targetPlayer,Hashtable playerProperties) 
        {
            if (!targetPlayer.IsLocal)
            {
                targetPlayer.CustomProperties = playerProperties;
            }
        }

        public UserInfo GetGuestInfo(string userId) 
        {
            UserInfo guest = null;
            currentWorkSpaceGuestPool.TryGetValue(userId, out guest);
            return guest;
        }

        //Get the infomation of  users who not belong to current workspace
        public void GetCurrentWorkSpaceGuestList() 
        {
            bool haveNewGuest = false;
            List<UserInfo> gustList = new List<UserInfo>(currentWorkSpaceGuestList.Count);
            Dictionary<string,string> newGuestList = new Dictionary<string, string>();
            foreach (var item in currentWorkSpaceGuestList)
            {
                if (currentWorkSpaceGuestPool.TryGetValue(item.Key,out UserInfo guest))
                {
                    if (!haveNewGuest) 
                    {
                        gustList.Add(guest);
                    }
                    continue;
                }
                haveNewGuest = true;
                gustList = null;
                newGuestList.Add(item.Key, item.Value);
            }
            if (haveNewGuest)
            {
                if (newGuestList.Count > 1)
                {
                    //first time load the guest list
                    this.SubscribeEvent<DBGetBatchUserInfoSuccessEvent>(OnDBGetBatchUserInfoSuccess);
                    dBService.GetBatchUserInfo(newGuestList);
                }
                else 
                {
                    this.SubscribeEvent<DBGetUserInfoSuccessEvent>(OnDBGetUserInfoSuccess);
                    string userId = newGuestList.First().Key;
                    dBService.GetUserInfo(currentWorkSpaceGuestList[userId],userId);
                }
            }
            else 
            {
                this.TriggerEvent(new GetCurrentWorkSpaceGuestListSuccessEvent { 
                    GuestList= gustList
                });
            }
        }
        //user who enter or leave the room mantain the GuestList property for it,other users in the room will
        //sync this infomation by the photon OnRoomPropertiesUpdate call back
        public void UpdateRoomGuestList(Player player,bool enterRoom) 
        {
            DealWithWorkSpaceGuestList(player, enterRoom);
        }
        #endregion

        #region Custom Event Call Backs
        private void OnConnectToPhotonServerSuccess(ConnectToServerSuccessEvent e) 
        {
            isInRoom = false;
            if (!isConnecting)
            {
                isConnecting = true;
                SetLocalPlayerCustomProperties();
            }
            else 
            {
                JoinRoom(JoiningWorkSpaceName);
            }
        }
        private void OnUserDisconnectFromServer(UserDisconnectFromServerEvent e) 
        {
            DisableUserTimer(e.UserId);
            //clear the cache
            currentWorkSpaceGuestPool.Remove(e.UserId);
        }
        private void OnUserJoinWorkSpace(UserJoinWorkSpaceEvent e) 
        {
            DisableUserTimer(e.UserId);
            ChangeUserInfo(e.UserId, (guest) => { 
                guest.WorkSpacenNowIn.Value = e.WorkSpaceNowIn;
            });
        }
        private void OnJoinedPhotonRoom(JoinPhotonRoomSuccessEvent e) 
        {
            isInRoom = true;
            string WorkSpacenNowIn = PhotonNetwork.CurrentRoom.Name;
            JoiningWorkSpaceName = null;
            userInfo.UserStatus.Value = (int)(UserStateEnum.Online);
            userInfo.WorkSpacenNowIn.Value = WorkSpacenNowIn;
            //sync the change to other users
            queueMessageService.PushEventMessage(new UserJoinWorkSpaceEvent {
                UserId = GetPlayerProperty(PhotonNetwork.LocalPlayer, USER_ID_PROPERTY),
                WorkSpaceNowIn = WorkSpacenNowIn
            }, null);
            //update the record in db
            //UpdateDBRecorde(new Dictionary<string, object>() { { "WorkSpacenNowIn", WorkSpacenNowIn } });
        }

        private void OnRoomPropertyUpdate(RoomPropertyUpdateEvent e) 
        {
            HashTable CustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            string[] oldGuestList = null;
            if (CustomProperties.TryGetValue(GUEST_LIST, out object guestList)) 
            {
                oldGuestList = guestList as string[];
                //if guest list have been changed,we need to update the chache and inform other users.
                if (oldGuestList.Length != currentWorkSpaceGuestList.Count)
                {
                    
                    UpdateCachedGuestList(oldGuestList);
                    this.TriggerEvent<WorkSpaceGuestListUpdateEvent>();
                }
            }
        }
        private void OnDBGetUserInfoSuccess(DBGetUserInfoSuccessEvent e) 
        {
            this.UnSubscribeEvent<DBGetUserInfoSuccessEvent>(OnDBGetUserInfoSuccess);
            currentWorkSpaceGuestPool.Add(e.UserInfo.UserId, e.UserInfo);
            this.TriggerEvent(new GetCurrentWorkSpaceGuestListSuccessEvent
            {
                GuestList = new List<UserInfo>(currentWorkSpaceGuestPool.Values)
            });
        }

        private void OnDBGetBatchUserInfoSuccess(DBGetBatchUserInfoSuccessEvent e) 
        {
            this.UnSubscribeEvent<DBGetBatchUserInfoSuccessEvent>(OnDBGetBatchUserInfoSuccess);
            foreach (var item in e.UserList)
            {
                currentWorkSpaceGuestPool.Add(item.UserId,item);
            }
            this.TriggerEvent(new GetCurrentWorkSpaceGuestListSuccessEvent
            {
                GuestList = new List<UserInfo>(currentWorkSpaceGuestPool.Values)
            });
        }
        #endregion

        #region private method region

        private void ConnectToServer(string userId) 
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string secretKey=Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(userId+Auth_MD5_Salt_Key)));
            AuthenticationValues authValues = new AuthenticationValues();
            authValues.AuthType = CustomAuthenticationType.Custom;
            authValues.SetAuthPostData(new Dictionary<string, object>() {
                {"userId",userId },
                { "secretKey",secretKey}
            });
            PhotonNetwork.AuthValues = authValues;
            PhotonNetwork.ConnectUsingSettings();
        }

        private void SetLocalPlayerCustomProperties() 
        {
            PhotonNetwork.LocalPlayer.NickName = userInfo.UserName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
                    { "TeamCode",userInfo.TeamInfo.TeamCode},
                    { "TeamName",userInfo.TeamInfo.TeamName},
                    { "UserId",userInfo.UserId}
                });
        }
        private void DisableUserTimer(string userId) 
        {
            if (offlineUserCache.TryGetValue(userId, out Timer timer))
            {
                timer.Close();
                offlineUserCache.Remove(userId);
            }
        }
        private void DealWithWorkSpaceGuestList(Player player,bool add) 
        {
            string teamName = GetPlayerProperty(player, TEAM_NAME_PROPERTY);
            if (PhotonNetwork.CurrentRoom.Name.Equals(teamName))
            {
                return;
            }
            HashTable CustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            object guestListObj;
            if (!CustomProperties.TryGetValue(GUEST_LIST, out guestListObj))
            {
                return;
            }
            string userId = GetPlayerProperty(player, USER_ID_PROPERTY);
            string teamCode = GetPlayerProperty(player, TEAM_CODE_PROPERTY);
            string[] cachedGuestInfoList = guestListObj as string[];
            string guestInfoStr = teamCode + "," + userId;
            string[] newGuestInfoList = AddOrRemoveItemFromArray(cachedGuestInfoList, guestInfoStr, add);
            if (newGuestInfoList.Length == cachedGuestInfoList.Length)
            {
                return;
            }
            HashTable newGuestListProperty = new HashTable
            {
                { GUEST_LIST, newGuestInfoList}
            };
            HashTable oldGuestListProperty = new HashTable
            {
                { GUEST_LIST, cachedGuestInfoList}
            };
            //only when the value of the custom property on the server equals to oldGuestListProperty,
            //then it will be change to newGuestListProperty,otherwise,nothing will happen
            PhotonNetwork.CurrentRoom.SetCustomProperties(newGuestListProperty, oldGuestListProperty);
        }

        //first,we need to check if the user was disconnected,if false,we need do nothing
        //if true,then need to check if the user disconnected by the regular way,if true,we need do nothing,
        //otherwise, we need to update the login state of the player in db to offline,and inform other player
        //to know that this player was go offline
        private void DealWithUserLoginState(Player player) 
        {
            string teamCode = GetPlayerProperty(player,TEAM_CODE_PROPERTY);
            string userId = GetPlayerProperty(player, USER_ID_PROPERTY);
            Timer timer = new Timer(new Random().Next(5,10)*1000);
            timer.Enabled = true;
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) =>
            {
                ((Timer)sender).Close();
                //if can remove,then this user should be disconnected by a non-regular way,then need to
                //update the db recorde,and inform other users.
                if (offlineUserCache.Remove(userId)) 
                {
                    dBService.UpdateUserInfo(teamCode, userId, new Dictionary<string, object>() {
                        { "UserStatus",0}
                    });
                    queueMessageService.PushEventMessage(new UserDisconnectFromServerEvent { UserId=userId},null);  
                }
            };
            offlineUserCache.Add(userId, new Timer());
        }
        private string[] AddOrRemoveItemFromArray(string[] oldArray,string item,bool add=true) 
        {
            int length = 0;
            bool exist = false;
            if (oldArray != null) 
            {
                length = oldArray.Length;
                exist = Array.Exists(oldArray, str => str.Equals(item));
            }
            if (add)
            {
                if (exist) 
                {
                    return oldArray;
                }
                string[] newArray = new string[++length];
                if (length>1)
                {
                    //only when the lenth greater than 1,we need to copy the old value to the new array  
                    Array.Copy(oldArray, newArray, length - 1);
                }
                //make the last item to be the new user
                newArray[length - 1] = item;
                return newArray;
            }
            else 
            {
                if (!exist)
                {
                    return oldArray;
                }
                //delete the user from the array
                int index = Array.IndexOf(oldArray, item);
                string[] newArray = new string[--length];
                if (length > 0) 
                {
                    oldArray[index] = oldArray[length];
                    Array.Copy(oldArray, newArray, length);
                }
                return newArray;
            }
        }

        private void UpdateCachedGuestList(string[] nowGuestArray) 
        {
            //just create a new dictionary,then we don't need to check which item changed
            currentWorkSpaceGuestList = new Dictionary<string, string>();
            foreach (var item in nowGuestArray)
            {
                string[] guestInfo = item.Split(',');
                currentWorkSpaceGuestList.Add(guestInfo[1], guestInfo[0]);
            }
        }
        private void ChangeUserInfo(string userId, Action<UserInfo> changeAction)
        {
            if (NeedToChange(userId))
            {
                if (currentWorkSpaceGuestPool.TryGetValue(userId, out UserInfo guest))
                {
                    changeAction?.Invoke(guest);
                }
            }
        }
        private bool NeedToChange(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            if (userId.Equals(userInfo.UserId))
            {
                return false;
            }
            return true;
        }
        private bool CheckLoginState(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetPlayerProperty(PhotonNetwork.LocalPlayer, USER_ID_PROPERTY);
            }
            bool result = !string.IsNullOrEmpty(AWSUtil.Instance.GetIdToken(userId));
            return result;
        }
        private void JoinRoom(string workSpaceName=null)
        {
            string teamName = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            //only team members can create workspace room.
            if (string.IsNullOrEmpty(workSpaceName) || workSpaceName.Equals(teamName))
            {
                 JoinOrCreateRoom();
            }
            else
            {
                //guests can only join the room
                PhotonNetwork.JoinRoom(workSpaceName);
            }
        }
        private void JoinOrCreateRoom()
        {
            string teamName = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            string teamCode = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_CODE_PROPERTY);
            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = maxPlayerPerRoom,
                PlayerTtl = 0,
                EmptyRoomTtl = 0,
                CustomRoomProperties = new Hashtable()
                {
                    { TEAM_CODE_PROPERTY,teamCode},
                    {TEAM_NAME_PROPERTY,teamName },
                    { GUEST_LIST, new string[0] }
            }
            };
            PhotonNetwork.JoinOrCreateRoom(teamName, roomOptions, TypedLobby.Default);
        }
        private void LoadWorkSpaceScene()
        {
            LogUtil.LogDebug("LoadWorkSpaceScene");
            //LOAD WORKSPACE SCENE
            //We only load if we are the first player,
            //else we rely on PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel("TestScene");
            }
        }
        private void LoadLoginScene()
        {
            //LGUSceneManager.Instance.LoadLoginScene();
        }
        private void LeaveRoom()
        {
            leaveRoom = true;
            PhotonNetwork.LeaveRoom();
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
                roomName = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            }
            return PhotonNetwork.CurrentRoom.Name.Equals(roomName);
        }
        private bool CheckIfRoomExist(string workSpaceName)
        {
            string teamName = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            if (string.IsNullOrEmpty(workSpaceName) || workSpaceName.Equals(teamName))
            {//no need to check  existence of own team workspace
                return true;
            }
            WorkSpaceModel workSpace;
            if (!WorkSpaceList.TryGetValue(workSpaceName, out workSpace))
            {
                return false;
            }
            /*if (!workSpace.IsOnline)
            {
                return false;
            }*/
            return true;
        }
        private bool CheckIfRoomFull(string workSpaceName)
        {
            foreach (var item in WorkSpaceList)
            {
                LogUtil.LogDebug(item.Key + "," + item.Value);
            }
            if (string.IsNullOrEmpty(workSpaceName))
            {
                workSpaceName = GetPlayerProperty(PhotonNetwork.LocalPlayer, TEAM_NAME_PROPERTY);
            }
            LogUtil.LogDebug("workSpaceName==" + workSpaceName);
            return WorkSpaceList[workSpaceName].IsFull;
        }
        private void InitWorkSpaceList()
        {
            //achieve the team list
            var result = logDBManager.Instance.GetTeamList();
            result.OnFailed((s) => { LogUtil.LogDebug("InitWorkSpaceList failed"); });
            result.OnSuccess((teamList) => {
                LogUtil.LogDebug("InitWorkSpaceList success" + teamList.Count);
                foreach (var item in teamList)
                {
                    WorkSpaceList.Add(item.TeamName, new WorkSpaceModel()
                    {
                        IsOnline = false,
                        Name = item.TeamName,
                        TeamCode = item.TeamCode
                    });
                }
            });
        }
        #endregion
    }
}