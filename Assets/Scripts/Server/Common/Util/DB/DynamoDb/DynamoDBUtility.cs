using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class DynamoDBUtility : IDBUtility
	{
		private AmazonDynamoDBClient _client;
		private Table _table;
		private Dictionary<DBComparisonOperator, QueryOperator> queryOperatorMap;
		public DynamoDBUtility()
		{
			queryOperatorMap = new Dictionary<DBComparisonOperator, QueryOperator>()
			{
				{ DBComparisonOperator.Equal,QueryOperator.Equal},
				{ DBComparisonOperator.GreaterThan,QueryOperator.GreaterThan},
				{ DBComparisonOperator.LessThan,QueryOperator.LessThan},
				{ DBComparisonOperator.GreaterThanOrEqual,QueryOperator.GreaterThanOrEqual},
				{ DBComparisonOperator.LessThanOrEqual,QueryOperator.LessThanOrEqual},
				{ DBComparisonOperator.BetweenAnd,QueryOperator.Between},
				{ DBComparisonOperator.BeginsWith,QueryOperator.BeginsWith},
			};
		}

		#region public Method region


		public async Task<bool> CheckTableExist(string tableName)
		{
			try
			{
				ListTablesResponse res = await _client.ListTablesAsync().ConfigureAwait(false);
				return res.TableNames.Contains(tableName);
			}
			catch (Exception) { throw; }
		}

		public DBInvokeHandler<bool> AddItem(DynamoDBUpdateModel addParam)
		{
			DBInvokeHandler<bool> handler = new DBInvokeHandler<bool>();
			var task = LoadTable(addParam);
			Task<Document> documentTask = task.ContinueWith<Document>((t) => {
				try
				{
					Document document = BuildAddDocument(addParam);
					return _table.PutItemAsync(document).Result;
				}
				catch (Exception e)
				{
					LogUtil.LogError("AddItem failed,", e);
					throw e;
				}
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
			HandlerWithFinalTask(documentTask, handler, (d) => { return true; });
			return handler;
		}

		/// <summary>
		/// update item by primarykey and sortkey(optional)
		/// </summary>
		/// <param name="updateParam">property ParttionKeyValue must be assigned</param>
		/// <returns></returns>
		public DBInvokeHandler<bool> UpdateItemByPrimarykey(DynamoDBUpdateModel updateParam)
		{
			DBInvokeHandler<bool> handler = new DBInvokeHandler<bool>();
			var loadTableTask = LoadTable(updateParam);

			Task<Document> documentTask = loadTableTask.ContinueWith((t) => {
				try
				{
					UpdateItemOperationConfig config = BuildBaseUpdateItemOperationConfig();
					Primitive[] primaryKeys = GetPrimaryKeys(updateParam);
					return UpdateItem(BuildUpdateDocument(updateParam), primaryKeys, config).Result;
				}
				catch (Exception e)
				{
					LogUtil.LogError("UpdateItemByPrimarykey failed,", e);
					throw e;
				}

			}, TaskContinuationOptions.OnlyOnRanToCompletion);
			HandlerWithFinalTask(documentTask, handler, (d) => { return true; });
			return handler;
		}

		/// <summary>
		/// update item 
		/// </summary>
		/// <param name="updateParam"></param>
		/// <param name="conditionalExpression"></param>
		/// <returns></returns>
		/*public async Task UpdateItemByCondition(DynamoDBUpdateModel updateParam, Expression conditionalExpression) 
		{
            try
            {
                LoadTable(updateParam);
                UpdateItemOperationConfig config = BuildBaseUpdateItemOperationConfig();
                config.ConditionalExpression = conditionalExpression;
                await UpdateItem(BuildUpdateDocument(updateParam), null, config).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
		}*/

		/// <summary>
		/// Get Item By partitionkey and sortkey(optional)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryParam">the property partitionkeyValue must be assigned</param>
		/// <returns></returns>
		public DBInvokeHandler<Dictionary<string, T>> GetItemByPrimaryKeyWithinDictionary<T>(DynamoDBQueryModel queryParam)
		{
			DBInvokeHandler<Dictionary<string, T>> handler = new DBInvokeHandler<Dictionary<string, T>>();
			Task<Document> task = GetItemByPrimaryKey(queryParam, handler);
			HandlerWithFinalTask(task, handler, (d) => { return DynamoDBDataConverter.Instance.ConvertToDictionary<T>(d); });
			return handler;
		}

		public DBInvokeHandler<T> GetItemByPrimaryKeyWithinCustomType<T>(DynamoDBQueryModel queryParam) where T : class, new()
		{
			DBInvokeHandler<T> handler = new DBInvokeHandler<T>();
			Task<Document> task = GetItemByPrimaryKey(queryParam, handler);
			HandlerWithFinalTask(task, handler, (d) => { return DynamoDBDataConverter.Instance.ConvertToCustomType<T>(d); });
			return handler;
		}

		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetBatchItemWithinDictionary<T>(DynamoDBBatchQueryModel batchQueryModel) 
		{
			DBInvokeHandler<DynamoDBQueryResultModel<T>> handler = new DBInvokeHandler<DynamoDBQueryResultModel<T>>();
			var loadTableTask = LoadTable(batchQueryModel);
			Task<List<Document>> batchTask = loadTableTask.ContinueWith((task)=> {
				DocumentBatchGet batchOpe=_table.CreateBatchGet();
				DynamoDBBaseModel baseModel = new DynamoDBBaseModel();
                foreach (var item in batchQueryModel.PrimaryKeyList)
                {
					baseModel.PartitionKey = item.Key;
					baseModel.SortKey = item.Value;
					Primitive[] keys=this.GetPrimaryKeys(baseModel);
					if (keys[1] != null)
					{
						batchOpe.AddKey(keys[0], keys[1]);
					}
					else 
					{
						batchOpe.AddKey(keys[0]);
					}
				}
				batchOpe.ExecuteAsync().Wait();
				return batchOpe.Results;
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
			HandlerWithFinalTask(batchTask,handler,(documentList)=> {
				DynamoDBQueryResultModel<T> result = new DynamoDBQueryResultModel<T>();
				if (documentList != null) 
				{
					foreach (var item in documentList)
					{
						result.DictionaryResultList.Add(DynamoDBDataConverter.Instance.ConvertToDictionary<T>(item));
					}
				}
				return result;
			});
			return handler;
		}
		/// <summary>
		/// //pagination query,
		/// tablename,Partition key must be asigned(both name and value,and the IsNumeric property)
		/// operator and operand are related,if you assigned one of them,do not forget to assign another one accordingly
		/// </summary>
		/// <param name="condition"></param>
		/// <returns></returns>
		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetItemsByPrimaryKeyPaginationWithinDictionary<T>(DynamoDBConditionModel condition)
		{
			DBInvokeHandler<DynamoDBQueryResultModel<T>> handler = new DBInvokeHandler<DynamoDBQueryResultModel<T>>();
			var loadTableTask = LoadTable(condition);
			Search search = null;
			Task<List<Document>> documentTask = loadTableTask.ContinueWith((t) => {
				search = _table.Query(BuildQueryOperationConfig(condition));
				List<Document> documentList = search.GetNextSetAsync().Result;
				return documentList;
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
			HandlerWithFinalTask(documentTask, handler, (d) => {
				DynamoDBQueryResultModel<T> result = GetDictionaryResultFromSearch<T>(d, search);
				return result;
			});
			return handler;
		}
		/// <summary>
		/// //pagination query,
		/// tablename,Partition key must be asigned(both name and value,and the IsNumeric property)
		/// operator and operand are related,if you assigned one of them,do not forget to assign another one accordingly
		/// </summary>
		/// <param name="condition"></param>
		/// <returns></returns>
		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetItemsByPrimaryKeyPaginationWithinCustomType<T>(DynamoDBConditionModel condition) where T : class, new()
		{
			DBInvokeHandler<DynamoDBQueryResultModel<T>> handler = new DBInvokeHandler<DynamoDBQueryResultModel<T>>();
			var loadTableTask = LoadTable(condition);
			Search search = null;
			Task<List<Document>> documentTask = loadTableTask.ContinueWith((t) => {
				search = _table.Query(BuildQueryOperationConfig(condition));
				List<Document> documentList = search.GetNextSetAsync().Result;
				return documentList;
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
			HandlerWithFinalTask(documentTask, handler, (d) => {
				DynamoDBQueryResultModel<T> result = GetCustomTypeResultFromSearch<T>(d, search);
				return result;
			});
			return handler;
		}

		/// <summary>
		/// Get all the items that match your criteria(if exist) form the specific table,
		/// operator and operand are related,if you assigned one of them,
		/// do not forget to assign another one accordingly,
		/// should only use these three properties TableName and NonKeyOperator and NonKeyOperand ,
		/// because primary keys are not efficient in this operation
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="condition">every property is optional,except the tableName</param>
		/// <returns></returns>
		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetAllItemsPaginationWithinDictionary<T>(DynamoDBConditionModel condition)
		{
			DBInvokeHandler<DynamoDBQueryResultModel<T>> handler = new DBInvokeHandler<DynamoDBQueryResultModel<T>>();
			var loadTableTask = LoadTable(condition);
			Search search = null;
			Task<List<Document>> documentTask = loadTableTask.ContinueWith((t) => {
				search = _table.Scan(BuildScanOperationConfig(condition));
				return search.GetNextSetAsync().Result;
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
			HandlerWithFinalTask(documentTask, handler, (d) => {
				DynamoDBQueryResultModel<T> result = GetDictionaryResultFromSearch<T>(d, search);
				return result;
			});
			return handler;
		}

		public DBInvokeHandler<DynamoDBQueryResultModel<T>> GetAllItemsPaginationWithinCustomType<T>(DynamoDBConditionModel condition) where T : class, new()
		{
			DBInvokeHandler<DynamoDBQueryResultModel<T>> handler = new DBInvokeHandler<DynamoDBQueryResultModel<T>>();
			var loadTableTask = LoadTable(condition);
			Search search = null;
			Task<List<Document>> documentTask = loadTableTask.ContinueWith((t) => {
				search = _table.Scan(BuildScanOperationConfig(condition));
				var result = search.GetNextSetAsync().Result;
				return result;
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
			HandlerWithFinalTask(documentTask, handler, (d) => {
				DynamoDBQueryResultModel<T> result = GetCustomTypeResultFromSearch<T>(d, search);
				return result;
			});
			return handler;
		}
		#endregion

		#region private method region
		private void HandlerWithFinalTask<T, K>(Task<K> documentTask, DBInvokeHandler<T> handler, Func<K, T> getResultAction) where K : class
		{
			var awaiter = documentTask.GetAwaiter();
			awaiter.OnCompleted(() => {
				if (documentTask.IsFaulted)
				{
					LogUtil.LogError("HandlerWithFinalTask error," + documentTask.Exception.StackTrace, documentTask.Exception.GetBaseException());
					handler.TriggerOnFailed();
				}
				else
				{
					try
					{
						T result = getResultAction.Invoke(awaiter.GetResult());
						handler.TriggerOnCompleted(result);
					}
					catch (Exception e)
					{
						LogUtil.LogError("HandlerWithFinalTask error," + e.StackTrace, e);
						handler.TriggerOnFailed();
					}
				}
			});
		}
		/// <summary>
		/// initiate connection to DynamoDB
		/// </summary>
		/// <param name="userName">username of current logined user</param>
		private void InitDynamoDBClient(string userId, bool unAuthSupport = false)
		{
			CognitoAWSCredentials credentials = AWSUtil.Instance.GetCognitoAWSCredentials(userId, unAuthSupport);
			//check user login status
			if (credentials == null)
			{
				LogUtil.LogInfo("InitDynamoDBClient Failed,User Not Authenticated: " + userId);
				_client = null;
				return;
			}
			if (_client != null)
			{
				return;
			}
			AmazonDynamoDBConfig dbConfig = new AmazonDynamoDBConfig()
			{
				RegionEndpoint = AWSRegionMapper.regionMapper[AWSUtil.Instance.GetAWSSetting().region]
			};
			_client = new AmazonDynamoDBClient(credentials, dbConfig);
		}
		private Task LoadTable(DynamoDBBaseModel loadParam)
		{
			if (loadParam.UnAuthSupport)
			{
				//can not use Resources.load in a task thread,so when the first time try to get items from db without login,
				//AWSUtill will not be initiated,need to init it now to avoid calling "load" in below task.
				AWSUtil.Instance.GetAWSSetting();
			}
			var task = Task.Run(() =>
			{
				InitDynamoDBClient(AWSUtil.Instance._user?.UserID, loadParam.UnAuthSupport);
				if (_table != null && loadParam.TableName.Equals(_table.TableName))
				{
					return;
				}
				_table = Table.LoadTable(_client, loadParam.TableName);
			});
			task.ContinueWith((t) => {
				LogUtil.LogError("LoadTable failed,", t.Exception.GetBaseException());
			}, TaskContinuationOptions.NotOnRanToCompletion);
			return task;
		}
		private Document BuildAddDocument(DynamoDBUpdateModel addParam)
		{
			Document document = BuildUpdateDocument(addParam);
			Primitive[] primaryKeys = GetPrimaryKeys(addParam);
			document[addParam.PartitionKey.Name] = primaryKeys[0];
			if (primaryKeys[1] != null)
			{
				document[addParam.SortKey.Name] = primaryKeys[1];
			}
			return document;
		}
		private Document BuildUpdateDocument(DynamoDBUpdateModel updateParam)
		{
			Document document = new Document();
			foreach (var item in updateParam.items)
			{
				document[item.Key] = DynamoDBDataConverter.Instance.BuildDynamoDBEntry(item.Value);
			}
			return document;
		}

		private UpdateItemOperationConfig BuildBaseUpdateItemOperationConfig()
		{
			UpdateItemOperationConfig config = new UpdateItemOperationConfig()
			{
				ReturnValues = ReturnValues.None
			};
			return config;
		}
		private Task<Document> UpdateItem(Document document, Primitive[] primaryKeys, UpdateItemOperationConfig config)
		{

			if (primaryKeys == null)
			{
				return _table.UpdateItemAsync(document, config);
			}
			else if (primaryKeys[1] == null)
			{
				return _table.UpdateItemAsync(document, primaryKeys[0], config);
			}
			else
			{
				return _table.UpdateItemAsync(document, primaryKeys[0], primaryKeys[1], config);
			}

		}
		private GetItemOperationConfig BuildGetItemOperationConfig(DynamoDBQueryModel queryParam)
		{
			GetItemOperationConfig config = new GetItemOperationConfig
			{
				ConsistentRead = true
			};
			if (queryParam.AttributesToGet != null && queryParam.AttributesToGet.Count > 0)
			{
				config.AttributesToGet = queryParam.AttributesToGet;
			}
			return config;
		}

		private Task<Document> GetItemByPrimaryKey<T>(DynamoDBQueryModel queryParam, DBInvokeHandler<T> handler)
		{
			GetItemOperationConfig config = BuildGetItemOperationConfig(queryParam);
			Primitive[] primaryKeys = GetPrimaryKeys(queryParam);
			var loadTableTask = LoadTable(queryParam);
			var documentTask = loadTableTask.ContinueWith((t) =>
			{
				if (primaryKeys[1] == null)
				{
					return _table.GetItemAsync(primaryKeys[0], config).Result;
				}
				else
				{
					return _table.GetItemAsync(primaryKeys[0], primaryKeys[1], config).Result;
				}

			}, TaskContinuationOptions.OnlyOnRanToCompletion);
			return documentTask;
		}
		private Primitive[] GetPrimaryKeys(DynamoDBBaseModel param)
		{
			Primitive[] primaryKeys = new Primitive[2];
			if (param.PartitionKey!=null&&!string.IsNullOrEmpty(param.PartitionKey.Value))
			{
				primaryKeys[0] = new Primitive(param.PartitionKey.Value, param.PartitionKey.IsNumeric);
			}
			if (param.SortKey!=null&&!string.IsNullOrEmpty(param.SortKey.Value))
			{
				primaryKeys[1] = new Primitive(param.SortKey.Value, param.SortKey.IsNumeric);
			}
			return primaryKeys;
		}
		private QueryOperationConfig BuildQueryOperationConfig(DynamoDBConditionModel condition)
		{
			QueryOperationConfig config = new QueryOperationConfig()
			{
				ConsistentRead = true,
				Select = SelectValues.AllAttributes,
				CollectResults = false,
				PaginationToken = condition.PaginationToken
			};
			if (condition.PageLimitCount != 0)
			{
				config.Limit = condition.PageLimitCount;
			}
			if (condition.AttributesToGet != null && condition.AttributesToGet.Count > 0)
			{
				config.AttributesToGet = condition.AttributesToGet;
				config.Select = SelectValues.SpecificAttributes;
			}
			config.Filter = BuildQueryFilter(condition);
			return config;
		}
		private ScanOperationConfig BuildScanOperationConfig(DynamoDBConditionModel condition)
		{
			ScanOperationConfig config = new ScanOperationConfig()
			{
				ConsistentRead = false,
				Select = SelectValues.AllAttributes,
				CollectResults = false,
				PaginationToken = condition.PaginationToken
			};
			if (condition.PageLimitCount != 0)
			{
				config.Limit = condition.PageLimitCount;
			}
			if (condition.AttributesToGet != null && condition.AttributesToGet.Count > 0)
			{
				config.AttributesToGet = condition.AttributesToGet;
				config.Select = SelectValues.SpecificAttributes;
			}
			config.Filter = BuildScanFilter(condition);
			return config;
		}
		private QueryFilter BuildQueryFilter(DynamoDBConditionModel condition)
		{
			QueryFilter filter = new QueryFilter();
			Primitive[] primaryKeys = GetPrimaryKeys(condition);
			filter.AddCondition(condition.PartitionKey.Name, QueryOperator.Equal, primaryKeys[0]);
			if (condition.KeyOperator != null && condition.KeyOperator.Count > 0)
			{
				if (primaryKeys[1] != null && condition.KeyOperator.ContainsKey(condition.SortKey.Name))
				{
					filter.AddCondition(condition.SortKey.Name, GetQueryOperator(condition.KeyOperator[condition.SortKey.Name]), primaryKeys[1]);
				}
			}
			if (condition.NonKeyOperator != null && condition.NonKeyOperator.Count > 0)
			{
				foreach (var item in condition.NonKeyOperator)
				{
					filter.AddCondition(item.Key, GetQueryOperator(item.Value),
						DynamoDBDataConverter.Instance.BuildDynamoDBEntry(condition.NonKeyOperand[item.Key]));
				}
			}
			return filter;
		}
		private ScanFilter BuildScanFilter(DynamoDBConditionModel condition)
		{
			ScanFilter filter = new ScanFilter();
			if (condition.NonKeyOperator != null && condition.NonKeyOperator.Count > 0)
			{
				foreach (var item in condition.NonKeyOperator)
				{
					filter.AddCondition(item.Key, GetScanOperator(item.Value),
						DynamoDBDataConverter.Instance.BuildDynamoDBEntry(condition.NonKeyOperand[item.Key]));
				}
			}
			return filter;
		}
		private QueryOperator GetQueryOperator(DBComparisonOperator dbOperator)
		{
			QueryOperator queryOperator;
			if (!queryOperatorMap.TryGetValue(dbOperator, out queryOperator))
			{
				queryOperator = QueryOperator.Equal;
			}
			return queryOperator;
		}
		private ScanOperator GetScanOperator(DBComparisonOperator dbOperator)
		{
			return (ScanOperator)dbOperator;
		}
		private DynamoDBQueryResultModel<T> GetDictionaryResultFromSearch<T>(List<Document> pageResult, Search search)
		{
			DynamoDBQueryResultModel<T> result = new DynamoDBQueryResultModel<T>();
			if (pageResult == null || pageResult.Count <= 0)
			{
				return result;
			}
			foreach (var item in pageResult)
			{
				result.DictionaryResultList.Add(DynamoDBDataConverter.Instance.ConvertToDictionary<T>(item));
			}
			AddPaginationResult<T>(search, result);
			return result;
		}

		private DynamoDBQueryResultModel<T> GetCustomTypeResultFromSearch<T>(List<Document> pageResult, Search search) where T : class, new()
		{
			LogUtil.LogDebug("GetCustomTypeResultFromSearch");
			DynamoDBQueryResultModel<T> result = new DynamoDBQueryResultModel<T>(true);
			if (pageResult == null || pageResult.Count <= 0)
			{
				return result;
			}
			foreach (var item in pageResult)
			{
				T value = DynamoDBDataConverter.Instance.ConvertToCustomType<T>(item);
				result.CustomTypeResultList.Add(value);
			}
			AddPaginationResult<T>(search, result);
			return result;
		}

		private void AddPaginationResult<T>(Search search, DynamoDBQueryResultModel<T> result)
		{
			result.PaginationToken = search.PaginationToken;
			result.TotalCount = search.Count;
			result.HaveMoreItems = !search.IsDone;
		}
		#endregion
	}
}