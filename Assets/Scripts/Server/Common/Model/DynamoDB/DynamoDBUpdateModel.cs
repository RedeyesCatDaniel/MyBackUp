using System.Collections.Generic;

namespace LGUVirtualOffice {
	public class DynamoDBUpdateModel : DynamoDBBaseModel
	{
		//key:attribute name,value:new value of the attribute
		public Dictionary<string, object> items;
	}
}