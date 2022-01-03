using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avJsonToolkit
    {
        public const string TableName = "LGU_JSON";
        public const string VALUE = "JSON_VALUE";
        public const string Key = "JSON_KEY";

        public static void Read(string id, System.Action<string> onFinishReading)
        {
            DynamoDBQueryModel model = new DynamoDBQueryModel()
            {
                TableName = avJsonToolkit.TableName,
                PartitionKey = new DynamoDBKeyModel()
                {
                    Name = Key,
                    Value = id
                },
                UnAuthSupport = true,
                AttributesToGet = new List<string>() {
                    avJsonToolkit.VALUE
                }
            };
            var handler = DynamoDBUtil.Instance.GetItemByPrimaryKeyWithinDictionary<string>(model);
            handler.OnCompleted((x) => {
                onFinishReading(x[avJsonToolkit.VALUE]);
            });
        }

        public static void Write(string id, string json, System.Action<bool> onFinishWriting)
        {
            DynamoDBUpdateModel model = new DynamoDBUpdateModel()
            {
                TableName = avJsonToolkit.TableName,
                PartitionKey = new DynamoDBKeyModel()
                {
                    Name = "JSON_KEY",
                    Value = id
                },
                items = new Dictionary<string, object>() {
                    { avJsonToolkit.VALUE, json}
                }

            };

            var awaiter = DynamoDBUtil.Instance.UpdateItemByPrimarykey(model);
            awaiter.OnCompleted((x)=> { onFinishWriting(x); });
        }
    }
}