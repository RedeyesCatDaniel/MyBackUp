using System.Collections.Generic;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
    public class DynamoDBService : AbstractService, IDBService
    {
		private List<TeamModel> teamList;
		private IDBUtility dBUtility;
        protected override void OnInit()
        {
			teamList = new List<TeamModel>();
			dBUtility = this.GetUtility<IDBUtility>();
        }
		public void GetTeamList(string paginationToken=null)
		{
			if (teamList != null && teamList.Count > 0)
			{
				this.TriggerEvent(new DBGetTeamListSuccessEvent(teamList));
			}
			DynamoDBConditionModel condition = new DynamoDBConditionModel()
			{
				TableName = DynamoDBTableConst.TABLE_ORGANIZATION,
				UnAuthSupport = true
			};
			if (!string.IsNullOrEmpty(paginationToken)) 
			{
				condition.PaginationToken = paginationToken;
			}
			DBInvokeHandler<DynamoDBQueryResultModel<TeamModel>> handler = dBUtility.GetAllItemsPaginationWithinCustomType<TeamModel>(condition);
			handler.OnCompleted((queryResult) => {
				teamList.AddRange(queryResult.CustomTypeResultList);
				if (queryResult.HaveMoreItems)
				{
					GetTeamList(queryResult.PaginationToken);
				}
				else 
				{
					this.TriggerEvent(new DBGetTeamListSuccessEvent(teamList));
				}
			});
			handler.OnFailed(() => {
				this.TriggerEvent<DBGetTeamListFailedEvent>();
			});
		}

		public void GetUserInfo(UserInfo userInfo)
		{
			DynamoDBQueryModel queryParam = new DynamoDBQueryModel()
			{
				TableName = DynamoDBTableConst.TABLE_USER_MEMBER_INFO,
				PartitionKey = GetPartitionKeyModel(DynamoDBTableConst.TABLE_USER_MEMBER_INFO, userInfo.TeamInfo.TeamCode),
				SortKey = GetSortKeyModel(DynamoDBTableConst.TABLE_USER_MEMBER_INFO, userInfo.UserId)
			};
			DBInvokeHandler<Dictionary<string, object>> handler = dBUtility.GetItemByPrimaryKeyWithinDictionary<object>(queryParam);
			handler.OnCompleted((queryResult) => {
				FillUserInfo(queryResult, userInfo);
				this.TriggerEvent(new DBGetUserInfoSuccessEvent() { UserInfo = userInfo});
			});
			handler.OnFailed(() => {
				this.TriggerEvent<DBGetUserInfoFailedEvent>();
			});
		}

		public void GetUserInfo(string teamCode,string userId)
		{
			UserInfo userInfo = new UserInfo()
			{
				UserId=userId,
				TeamInfo=new TeamModel { TeamCode=teamCode}
			};
			GetUserInfo(userInfo);
		}
		public void GetBatchUserInfo(Dictionary<string, string> userList) 
		{
			Dictionary<DynamoDBKeyModel, DynamoDBKeyModel> primaryKeyList = new Dictionary<DynamoDBKeyModel, DynamoDBKeyModel>();
			foreach (var item in userList)
            {
				DynamoDBKeyModel partitionKey = GetPartitionKeyModel(DynamoDBTableConst.TABLE_USER_MEMBER_INFO, item.Value);
				DynamoDBKeyModel sortKey = GetSortKeyModel(DynamoDBTableConst.TABLE_USER_MEMBER_INFO, item.Key);
				primaryKeyList.Add(partitionKey, sortKey);
			}
			DynamoDBBatchQueryModel batchQueryModel = new DynamoDBBatchQueryModel()
			{
				TableName = DynamoDBTableConst.TABLE_USER_MEMBER_INFO,
				PrimaryKeyList= primaryKeyList
			};
			var handler=dBUtility.GetBatchItemWithinDictionary<object>(batchQueryModel);
			handler.OnCompleted((queryResult)=> {
				List<UserInfo> userList = new List<UserInfo>();
				if (queryResult != null&& queryResult.DictionaryResultList!=null) 
				{
                    foreach (var item in queryResult.DictionaryResultList)
                    {
						UserInfo user = new UserInfo();
						user.BuildUserInfo(item,user);
						userList.Add(user);
                    }
				}
				this.TriggerEvent(new DBGetBatchUserInfoSuccessEvent{UserList= userList });
			});
			handler.OnFailed(()=> {
				this.TriggerEvent<DBGetBatchUserInfoFailedEvent>();
			});
		}
		/// <summary>
		/// update date user's infomation which specified in the updateItem
		/// </summary>
		/// <param name="teamCode">the teamCode of the team the user who you want to update belongs to </param>
		/// <param name="userId">the userId of the user you want to update</param>
		/// <param name="updateItem">key is the name of the attribute which you want to update</param>
		public void UpdateUserInfo(string teamCode,string userId,Dictionary<string,object> updateItem) 
		{
			DynamoDBUpdateModel updateModel = new DynamoDBUpdateModel()
			{
				TableName = DynamoDBTableConst.TABLE_USER_MEMBER_INFO,
				PartitionKey = GetPartitionKeyModel(DynamoDBTableConst.TABLE_USER_MEMBER_INFO, teamCode),
				SortKey = GetSortKeyModel(DynamoDBTableConst.TABLE_USER_MEMBER_INFO, userId),
				items = updateItem
			};
			dBUtility.UpdateItemByPrimarykey(updateModel);
		}

		#region private method region
        private void FillUserInfo(Dictionary<string, object> queryResult, UserInfo userInfo)
		{
			userInfo.BuildUserInfo(queryResult, userInfo);
		}

		private DynamoDBKeyModel GetPartitionKeyModel(string tableName,string partitionKeyValue,bool isNumeric= false) 
		{
			DynamoDBKeyModel partitionKey = new DynamoDBKeyModel
			{
				Name = DynamoDBTableConst.GetTablePartitionKeyName(tableName),
				Value = partitionKeyValue,
				IsNumeric= isNumeric
			};
			return partitionKey;
		}
		private DynamoDBKeyModel GetSortKeyModel(string tableName,string sortKeyValue,bool isNumeric=false) 
		{
			if (string.IsNullOrEmpty(sortKeyValue)) {
				return new DynamoDBKeyModel();
			}
			DynamoDBKeyModel sortKey = new DynamoDBKeyModel
			{
				Name = DynamoDBTableConst.GetTablePartitionKeyName(tableName),
				Value = sortKeyValue,
				IsNumeric=isNumeric
			};
			return sortKey;
		}
		#endregion
	}
}