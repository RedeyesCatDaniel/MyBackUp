using LGUVirtualOffice.Framework;
using System.Collections.Generic;

namespace LGUVirtualOffice {
	public interface IDBService : IService
	{
		//supplies two event: DBGetTeamListSuccessEvent，DBGetTeamListFailedEvent
		public void GetTeamList(string paginationToken = null);
		//supplies two event:DBGetUserInfoSuccessEvent,DBGetUserInfoFailedEvent
		public void GetUserInfo(UserInfo userInfo);
		public void GetUserInfo(string teamCode,string userId);
		/// <summary>
		/// supplies two event: DBGetBatchUserInfoSuccessEvent,DBGetBatchUserInfoFailedEvent
		/// </summary>
		/// <param name="userList">key:userId,value:teamCode</param>
		public void GetBatchUserInfo(Dictionary<string, string> userList);
		public void UpdateUserInfo(string teamCode, string userId, Dictionary<string, object> updateItem);
	}
}