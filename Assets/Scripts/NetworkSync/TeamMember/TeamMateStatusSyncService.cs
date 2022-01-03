using LGUVirtualOffice.Framework;
using System;

namespace LGUVirtualOffice {
	public class TeamMateStatusSyncService : AbstractService
	{
        private UserInfo localUser;
        private INetworkSyncService networkSyncService;
        protected override void OnInit()
        {
            localUser = this.GetModel<UserInfo>();
            networkSyncService = this.GetService<INetworkSyncService>();
            //Subscribe event
            this.SubscribeEvent<UserJoinWorkSpaceEvent>(OnUserJoinPhotonRoomSuccess);
            this.SubscribeEvent<UserDisconnectFromServerEvent>(OnUserDisconnectFromServer);
            this.SubscribeEvent<AreaChangeNetEvent>(OnUserAreaStayingChanged);
            this.SubscribeEvent<UserSignatureModifiedEvent>(OnUserSignatureChanged);
            this.SubscribeEvent<UserStatusModifiedEvent>(OnUserStateChanged);
            //signature
            //userState
        }
        #region call back region
        private void OnUserJoinPhotonRoomSuccess(UserJoinWorkSpaceEvent e) 
        {
            ChangeTeamMateInfo(e.UserId, (teamMate) => {
                teamMate.UserStatus.Value = (int)(UserStateEnum.Online);
                teamMate.WorkSpacenNowIn.Value = e.WorkSpaceNowIn;
            });
        }
        private void OnUserDisconnectFromServer(UserDisconnectFromServerEvent e) 
        {
            ChangeTeamMateInfo(e.UserId, (teamMate) => {
                teamMate.UserStatus.Value = (int)UserStateEnum.Offline;
            });
        }
        private void OnUserAreaStayingChanged(AreaChangeNetEvent e) 
        {
            ChangeTeamMateInfo(e.userID, (teamMate) => {
                teamMate.AreaStaying.Value = (int)(e.Area);
            });
            ChangeGuestInfo(e.userID, (guest) => {
                guest.AreaStaying.Value = (int)(e.Area);
            });
        }
        private void OnUserSignatureChanged(UserSignatureModifiedEvent e) 
        {
            ChangeTeamMateInfo(e.UserId, (teamMate) => {
                teamMate.Signature.Value = e.NewSignature;
            });
            ChangeGuestInfo(e.UserId, (guest) => {
                guest.Signature.Value = e.NewSignature;
            });
        }
        private void OnUserStateChanged(UserStatusModifiedEvent e)
        {
            ChangeTeamMateInfo(e.UserId, (teamMate) => {
                teamMate.UserStatus.Value = e.NewState;
            });
            ChangeGuestInfo(e.UserId, (guest) => {
                guest.UserStatus.Value = e.NewState;
            });
        }
        
        #endregion


        #region private method region
        private void ChangeTeamMateInfo(string userId,Action<UserInfo> changeAction) 
        {
            if (NeedToChange(userId)) 
            {
                localUser.GetTeamMateInfo(userId, (teamMate) => {
                    if (teamMate != null)
                    {
                        changeAction?.Invoke(teamMate);
                    }
                });
            }
        }

        private void ChangeGuestInfo(string userId, Action<UserInfo> changeAction)
        {
            if (NeedToChange(userId))
            {
                UserInfo guest = networkSyncService.GetGuestInfo(userId);
                if (guest != null) 
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
            if (userId.Equals(localUser.UserId))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}