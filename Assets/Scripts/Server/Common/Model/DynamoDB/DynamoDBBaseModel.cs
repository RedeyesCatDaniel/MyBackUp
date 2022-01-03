using UnityEngine;

namespace LGUVirtualOffice {
	public class DynamoDBBaseModel
	{
		public string TableName { get; set; }
		public DynamoDBKeyModel PartitionKey { get; set; }
		public DynamoDBKeyModel SortKey { get; set; }

		public bool UnAuthSupport { get; set; }
	}
}