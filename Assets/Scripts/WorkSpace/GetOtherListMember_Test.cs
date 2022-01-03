using LGUVirtualOffice.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class GetOtherListMember : AbstractService
    {
        private IDBUtility dBUtility;
        private Dictionary<string, UserInfo> teamMatePool;
        protected override void OnInit()
        {
            //localUser = this.GetModel<UserInfo>();
            dBUtility = this.GetUtility<IDBUtility>();
        }

        public void GetList(string teamCode, Action<List<UserInfo>> onCompleted)
        {
            //userinfo.BuildUserInfo(userinfo);
            GetTeamMateList(teamCode, () => {
                //Test();
                onCompleted?.Invoke(new List<UserInfo>(teamMatePool.Values));
            });

            //List<UserInfo> a = new List<UserInfo>();
            //if (onCompleted != null)
            //    onCompleted( a);
        }

        private void GetTeamMateList(string teamCode, Action onCompleted, string pagenationTolen = null)
        {
            if(teamMatePool==null)
                 teamMatePool = new Dictionary<string, UserInfo>();
            DynamoDBConditionModel queryParam = new DynamoDBConditionModel()
            {
                TableName = DynamoDBTableConst.TABLE_USER_MEMBER_INFO,
                PartitionKey = new DynamoDBKeyModel { Name = "TeamCode", Value = teamCode },
                PaginationToken = pagenationTolen
            };
            DBInvokeHandler<DynamoDBQueryResultModel<object>> handler = dBUtility.GetItemsByPrimaryKeyPaginationWithinDictionary<object>(queryParam);
            handler.OnCompleted((queryResult) => {
                foreach (var item in queryResult.DictionaryResultList)
                {
                    UserInfo userInfo = new UserInfo();
                    userInfo.BuildUserInfo(item, userInfo);
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
            });
        }

    }
}