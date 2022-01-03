using System.Collections.Generic;
namespace LGUVirtualOffice {
	public class DynamoDBConditionModel:DynamoDBQueryModel
	{
		/*for writing condition expressions,stores primary key attribute operator
		key:attribute name,value:DBComparisonOperator*/
		public Dictionary<string,DBComparisonOperator> KeyOperator { get; set; }

		/*for writing condition expressions,stores other attribute operator
		key:attribute name,value:DBComparisonOperator*/
		public Dictionary<string, DBComparisonOperator> NonKeyOperator { get; set; }
		/*for writing condition expressions
		key:attribute name,value:operand,if not only one,than use a List,
		fro example,you need multiple operands with operator "in" or "between..and.."
		*/
		public Dictionary<string, object> NonKeyOperand { get; set; }

		/*for writing condition expressions
		key:attribute name,value:operand,if not only one,than use a List,
		fro example,you need multiple operands with operator "in" or "between..and.."
		*/
		public Dictionary<string, object> KeyOperand { get; set; }

		/*Pagination token corresponding to the item where the last Query operation stopped,
		inclusive of the previous result set.
		Set this value to resume Query operation from the next item.
		This token should be retrieved from a Search object.*/
		public string PaginationToken { get; set; }

		//determins how many items should be return in one query,zero means no limitation 
		public int PageLimitCount { get; set; }
	}
}