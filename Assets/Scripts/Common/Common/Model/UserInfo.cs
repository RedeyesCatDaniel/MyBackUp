using System;
using System.Collections.Generic;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    public class UserInfo : AbstractModel
    {
        private DynamoDBUpdateModel updateModel;
        private static UserInfo _instance;
        [Obsolete("Deprecated,please use GetModel<UserInfo>() to achieve local user's UserInfo instance")]
        public static UserInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserInfo();
                }
                return _instance;
            }
            set { _instance = value; }
        }
        private IDBUtility dBUtility;
        private Dictionary<string, UserInfo> teamMatePool;
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        //0:male,1:female 
        public int Gender { get; set; }
        //personalized signature
        public BindableProperty<string> Signature { get; } = new BindableProperty<string>();
        public BindableProperty<int> UserStatus { get; } = new BindableProperty<int>();
        //The name of the workspace which user are stay in;
        public BindableProperty<string> WorkSpacenNowIn { get; } = new BindableProperty<string>();

        //The area of the workspace where user are staying
        public BindableProperty<int> AreaStaying { get; } = new BindableProperty<int>();

        public string VocationStartTime { get; set; }

        public string VocationEndTime { get; set; }

        public string FavoriteWorkSpaceList { get; set; }
        //team infomation of which user belongs to
        public TeamModel TeamInfo { get; set; }

        //user's avatar infomation
        /*public AvatarModel AvatarInfo { get; set; }*/

        public void ClearCache()
        {
            UserInfo newUser = new UserInfo();
            newUser.OnInit();
            LGUVirtualOffice.SubscribeRegisterpatch((architecture) => {
                architecture.RegisterModel(newUser);
            }, true);
        }
        
        public void GetTeamMateInfo(string userId, Action<UserInfo> onCompleted)
        {
            if (string.IsNullOrEmpty(userId))
            {
                onCompleted?.Invoke(null);
            }
            else if (!string.IsNullOrEmpty(UserId) && userId.Equals(UserId))
            {
                onCompleted?.Invoke(this);
            }
            else
            {
                //get from database
                if (teamMatePool == null)
                {
                    teamMatePool = new Dictionary<string, UserInfo>();
                }
                if (teamMatePool.Count > 0)
                {
                    if (teamMatePool.TryGetValue(userId, out UserInfo userInfo))
                    {
                        onCompleted?.Invoke(userInfo);
                    }
                    else
                    {
                        onCompleted?.Invoke(null);
                    }
                }
                else
                {
                    GetTeamMateList((teamMateList) => {
                        GetTeamMateInfo(userId, (userInfo) => { onCompleted?.Invoke(userInfo); });
                    },true);
                }
            }
        }

        public void GetTeamMateList(Action<List<UserInfo>> onCompleted, bool IsInit = false)
        {
            if (teamMatePool == null)
            {
                teamMatePool = new Dictionary<string, UserInfo>();
            }
            if (teamMatePool.Count > 0)
            {
                onCompleted?.Invoke(new List<UserInfo>(teamMatePool.Values));
            }
            else
            {
                if (!IsInit)
                {
                    GetTeamMateList(TeamInfo.TeamCode, () => { onCompleted?.Invoke(new List<UserInfo>(teamMatePool.Values)); });
                }
                else
                {
                    GetTeamMateList(TeamInfo.TeamCode, () => { onCompleted?.Invoke(null); });
                }
            }
        }
        protected override void OnInit()
        {
            Instance = this;
            dBUtility = this.GetUtility<IDBUtility>();
            UserStatus.Subscribe(UpdateUserStatus);
            WorkSpacenNowIn.Subscribe(UpdateWorkSpacenNowIn);
            AreaStaying.Subscribe(UpdateAreaStaying);
        }
        private void GetTeamMateList(string teamCode, Action onCompleted, string pagenationTolen = null)
        {
            DynamoDBConditionModel queryParam = new DynamoDBConditionModel()
            {
                TableName = DynamoDBTableConst.TABLE_USER_MEMBER_INFO,
                PartitionKey = new DynamoDBKeyModel {
                    Name = DynamoDBTableConst.GetTablePartitionKeyName(DynamoDBTableConst.TABLE_USER_MEMBER_INFO), 
                    Value = teamCode 
                },
                PaginationToken = pagenationTolen
            };
            DBInvokeHandler<DynamoDBQueryResultModel<object>> handler = dBUtility.GetItemsByPrimaryKeyPaginationWithinDictionary<object>(queryParam);
            handler.OnCompleted((queryResult) => {
                if (queryResult != null) 
                {
                    foreach (var item in queryResult.DictionaryResultList)
                    {
                        UserInfo userInfo = BuildUserInfo(item);
                        teamMatePool.Add(userInfo.UserId, userInfo);
                    }
                    if (queryResult.HaveMoreItems)
                    {
                        GetTeamMateList(teamCode, () => { onCompleted?.Invoke(); }, queryResult.PaginationToken);
                    }
                    else
                    {
                        onCompleted?.Invoke();
                    }
                }
            });
        }
        public UserInfo BuildUserInfo(Dictionary<string, object> queryResult, UserInfo userInfo = null)
        {
            if (userInfo == null)
            {
                userInfo = new UserInfo();
            }
            if (queryResult.TryGetValue("UserName", out object userName))
            {
                userInfo.UserName = userName as string;
            }
            if (queryResult.TryGetValue("UserId", out object userId))
            {
                userInfo.UserId = userId as string;
            }
            if (queryResult.TryGetValue("Gender", out object gender))
            {
                userInfo.Gender = Convert.ToInt32(gender);
            }
            if (queryResult.TryGetValue("Signature", out object signature))
            {
                userInfo.Signature.Value = signature as string;
            }
            if (queryResult.TryGetValue("UserStatus", out object userStatus))
            {
                userInfo.UserStatus.Value = Convert.ToInt32(userStatus);
            }
            if (queryResult.TryGetValue("WorkSpacenNowIn", out object workSpacenNowIn))
            {
                userInfo.WorkSpacenNowIn.Value = workSpacenNowIn as string;
            }
            if (queryResult.TryGetValue("AreaStaying", out object areaStaying))
            {
                userInfo.AreaStaying.Value = Convert.ToInt32(areaStaying);
            }
            if (queryResult.TryGetValue("VocationStartTime", out object vocationStartTime))
            {
                userInfo.VocationStartTime = vocationStartTime as string;
            }
            if (queryResult.TryGetValue("VocationEndTime", out object vocationEndTime))
            {
                userInfo.VocationEndTime = vocationEndTime as string;
            }
            if (queryResult.TryGetValue("FavoriteWorkSpaceList", out object favoriteWorkSpaceList))
            {
                userInfo.FavoriteWorkSpaceList = favoriteWorkSpaceList as string;
            }
            userInfo.TeamInfo = new TeamModel();
            if (queryResult.TryGetValue("TeamCode", out object teamCode))
            {
                userInfo.TeamInfo.TeamCode = teamCode as string;
            }
            if (queryResult.TryGetValue("TeamName", out object teamName))
            {
                userInfo.TeamInfo.TeamName = teamName as string;
            }
            /*userInfo.AvatarInfo = new AvatarModel();
            if (queryResult.TryGetValue("AvatarColorData", out object avatarColorData))
            {
                userInfo.AvatarInfo.AvatarColorData = avatarColorData as string;
            }
            if (queryResult.TryGetValue("AvatarData", out object avatarData))
            {
                userInfo.AvatarInfo.AvatarData = avatarData as string;
            }*/
            return userInfo;
        }

        private void UpdateUserStatus(int newState)
        {
            InitUpdateModel();
            updateModel.items = new Dictionary<string, object> { { "UserStatus", newState} };
            dBUtility.UpdateItemByPrimarykey(updateModel);
        }

        private void UpdateWorkSpacenNowIn(string newWorkSpace)
        {
            InitUpdateModel();
            updateModel.items = new Dictionary<string, object> { { "WorkSpacenNowIn", newWorkSpace } };
            dBUtility.UpdateItemByPrimarykey(updateModel);
        }

        private void UpdateAreaStaying(int newArea)
        {
            InitUpdateModel();
            updateModel.items = new Dictionary<string, object> { { "AreaStaying", newArea} };
            dBUtility.UpdateItemByPrimarykey(updateModel);
        }
        private void InitUpdateModel() 
        {
            if (updateModel != null) 
            {
                return;
            }
            updateModel = new DynamoDBUpdateModel()
            {
                TableName = DynamoDBTableConst.TABLE_USER_MEMBER_INFO,
                PartitionKey = new DynamoDBKeyModel
                {
                    Name = DynamoDBTableConst.GetTablePartitionKeyName(DynamoDBTableConst.TABLE_USER_MEMBER_INFO),
                    Value = TeamInfo.TeamCode
                },
                SortKey = new DynamoDBKeyModel
                {
                    Name = DynamoDBTableConst.GetTableSortKeyName(DynamoDBTableConst.TABLE_USER_MEMBER_INFO),
                    Value = UserId
                }
            };
        }
    }
}

