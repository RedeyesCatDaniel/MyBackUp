using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Extensions.CognitoAuthentication;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LGUVirtualOffice
{
	public class logDBManager : Singleton<logDBManager>
	{
		private logDBManager() { }
		private List<TeamModel> teamList;
		public ServerReturnModel<List<TeamModel>> GetTeamList()
		{
			ServerReturnModel<List<TeamModel>> result = new ServerReturnModel<List<TeamModel>>() { };
			if (teamList != null && teamList.Count > 0)
			{
				result.TriggerOnSuccess(teamList);
				return result;
			}
			DynamoDBConditionModel condition = new DynamoDBConditionModel()
			{
				TableName = DynamoDBTableConst.TABLE_ORGANIZATION,
				UnAuthSupport = true
			};
			DBInvokeHandler<DynamoDBQueryResultModel<TeamModel>> handler = DynamoDBUtil.Instance.GetAllItemsPaginationWithinCustomType<TeamModel>(condition);
			handler.OnCompleted((queryResult) => { result.TriggerOnSuccess(queryResult.CustomTypeResultList); });
			handler.OnFailed(() => { result.TriggerOnFailed(ReturnMessageConst.sys_System_Error); });
			return result;
		}

		public ServerReturnModel<UserInfo> GetUserInfo(UserInfo userInfo)
		{
			ServerReturnModel<UserInfo> result = new ServerReturnModel<UserInfo>();
			DynamoDBQueryModel queryParam = new DynamoDBQueryModel()
			{
				TableName = DynamoDBTableConst.TABLE_USER_MEMBER_INFO,
				PartitionKey = new DynamoDBKeyModel { Value = userInfo.TeamInfo.TeamCode },
				SortKey = new DynamoDBKeyModel { Value = userInfo.UserId }
			};
			DBInvokeHandler<Dictionary<string, object>> handler = DynamoDBUtil.Instance.GetItemByPrimaryKeyWithinDictionary<object>(queryParam);
			handler.OnCompleted((queryResult) => {
				FillUserInfo(queryResult, userInfo);
				result.TriggerOnSuccess(userInfo);
			});
			handler.OnFailed(() => { LogUtil.LogDebug("GetUserInfo OnFailed"); result.TriggerOnFailed(ReturnMessageConst.sys_System_Error); });
			return result;
		}
		private void FillUserInfo(Dictionary<string, object> queryResult, UserInfo userInfo)
		{
			userInfo.BuildUserInfo(queryResult,userInfo);
		}

		/*private async void UpdateTeamList() 
		{
			DynamoDBConditionModel condition = new DynamoDBConditionModel()
			{
				TableName = DynamoDBTableConst.TABLE_ORGANIZATION,
				UnAuthSupport = true
			};
			DynamoDBQueryResultModel<TeamModel> result = await DynamoDBUtil.Instance.GetAllItemsPaginationWithinCustomType<TeamModel>(condition);
			if (result.TotalCount > 0&&result.TotalCount!=teamList.Count) 
			{
				teamList = result.CustomTypeResultList;
			}
		}*/
	}
}