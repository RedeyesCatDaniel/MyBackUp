using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice {
	public class DynamoDBQueryModel : DynamoDBBaseModel
	{
		//specific attributes to query,if null(by default),get all the attributes declared in this table, example:attributesToGet = new List<string> { "Id", "Title"}
		public List<string> AttributesToGet { get; set; }
	}
}