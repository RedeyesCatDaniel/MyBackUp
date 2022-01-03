using System.Collections.Generic;

namespace LGUVirtualOffice {
	public static class DynamoDBTableConst
	{
		public static string TABLE_ORGANIZATION = "LGU_Team_Info";
		public static string TABLE_USER = "LGU_User_Info";
		public static string TABLE_MEMBER_DATA = "LGU_MEMBER_DATA";
		public static string TABLE_USER_MEMBER_INFO = "LGU_User_Member_Info";
		private static Dictionary<string, string> partitionKeyPool = new Dictionary<string, string>
		{
			{ TABLE_ORGANIZATION,"TeamCode"},
			{ TABLE_USER_MEMBER_INFO,"TeamCode"}
		};
		private static Dictionary<string, string> sortKeyPool = new Dictionary<string, string>
		{
			{ TABLE_USER_MEMBER_INFO,"UserId"}
		};

		public static string GetTablePartitionKeyName(string tableName) 
		{
			string keyName = null;
			partitionKeyPool.TryGetValue(tableName, out keyName);
			return keyName;
		}

		public static string GetTableSortKeyName(string tableName) 
		{
			string keyName=null;
			sortKeyPool.TryGetValue(tableName, out keyName);
			return keyName;
		}
		public static bool IsTableHaveSortkey(string tableName) 
		{
			return sortKeyPool.ContainsKey(tableName);
		}
	}
}