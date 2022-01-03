using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	/// <summary>
	/// used for batch get operation,should use the  PrimaryKeyList property
	/// properties PartitionKey and SortKey is not capable for this operation,they will not be used
	/// </summary>
	public class DynamoDBBatchQueryModel:DynamoDBBaseModel
	{
		public Dictionary<DynamoDBKeyModel, DynamoDBKeyModel> PrimaryKeyList { get; set; }
	}
}
