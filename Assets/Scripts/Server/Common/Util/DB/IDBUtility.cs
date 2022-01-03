using LGUVirtualOffice.Framework;
using System.Collections.Generic;

namespace LGUVirtualOffice {
	public interface IDBUtility : IUtility
	{
		//TABLENAME,ITEMS,KEY,UnAuthSupport
		public DBInvokeHandler<bool> AddItem(DynamoDBUpdateModel addParam);
		public DBInvokeHandler<bool> UpdateItemByPrimarykey(DynamoDBUpdateModel updateParam);
		public DBInvokeHandler<Dictionary<string, T>> GetItemByPrimaryKeyWithinDictionary<T>(DynamoDBQueryModel queryParam);
		public DBInvokeHandler<T> GetItemByPrimaryKeyWithinCustomType<T>(DynamoDBQueryModel queryParam) where T : class, new();
		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetItemsByPrimaryKeyPaginationWithinDictionary<T>(DynamoDBConditionModel condition);
		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetItemsByPrimaryKeyPaginationWithinCustomType<T>(DynamoDBConditionModel condition) where T : class, new();
		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetAllItemsPaginationWithinDictionary<T>(DynamoDBConditionModel condition);
		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetAllItemsPaginationWithinCustomType<T>(DynamoDBConditionModel condition) where T : class, new();
		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetBatchItemWithinDictionary<T>(DynamoDBBatchQueryModel batchQueryModel);
	}
}