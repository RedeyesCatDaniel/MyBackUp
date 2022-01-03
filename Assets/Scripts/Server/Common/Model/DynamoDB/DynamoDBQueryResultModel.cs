using System.Collections.Generic;

namespace LGUVirtualOffice {
	public class DynamoDBQueryResultModel<T>
	{
		public DynamoDBQueryResultModel(bool isCustomType=false) 
		{
			IsCustomType = isCustomType;
			if (isCustomType)
			{
				CustomTypeResultList = new List<T>();
			}
			else 
			{
				DictionaryResultList = new List<Dictionary<string, T>>();
			}
		}

		public bool IsCustomType { get; set; }
		public List<Dictionary<string,T>> DictionaryResultList { get; set; }

		public List<T> CustomTypeResultList { get; set; }

		public string PaginationToken { get; set; }

		public bool HaveMoreItems { get; set; }

		public int TotalCount { get; set; }
	}
}