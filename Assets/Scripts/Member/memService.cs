using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LGUVirtualOffice
{
    public interface IMemberService:IService {
        public IMember MyMember { get;  }

    }
    public class memService : AbstractService, IMemberService
    {
        public const string TableName = "LGU_MEMBER_DATA";
        public Member myMember;
        public IMember MyMember { get => myMember; }

        //private static memService backdoor;
        //public static memService Backdoor{
        //    get {
        //        if (backdoor == null) {
        //            backdoor = new memService();
        //            LGUVirtualOffice.SubscribeRegisterpatch((instance) =>
        //            {
        //                IService service = backdoor;
        //                service.SetArchitecture(instance);
        //            }, false);
        //        }

        //        return backdoor;
        //    }
        //}


        protected override void OnInit(){}

        //this function will push data into server
        //the onpushdone will receive bool value indicating the pushing is success or not
        
        public static void PushData<T>(string userid,string Key,T value, System.Action<bool> OnPushDone) {

            PushData<T>(userid,Key,value).OnCompleted((x) => { OnPushDone(x); }); 
        }

        public static DBInvokeHandler<bool> PushData<T>(string userid, string Key, T value)
        {
            DynamoDBUpdateModel model = new DynamoDBUpdateModel()
            {
                TableName = memService.TableName,
                PartitionKey = new DynamoDBKeyModel()
                {
                    Name = Key,
                    Value = userid
                },
                items = new Dictionary<string, object>() {
                    { Key, value}
                }

            };

            var awaiter = DynamoDBUtil.Instance.UpdateItemByPrimarykey(model);
            return awaiter;
        }


        public static void PullData<T>(string userid, string Key, System.Action<T> OnPullComplete) {
            //DynamoDBQueryModel model = new DynamoDBQueryModel()
            //{
            //    TableName = memService.TableName,
            //    PartitionKey = new DynamoDBKeyModel()
            //    {
            //        Name = Key,
            //        Value = userid
            //    },
            //    AttributesToGet = new List<string>() {
            //        Key
            //    }
            //};
            //var handler = DynamoDBUtil.Instance.GetItemByPrimaryKeyWithinDictionary<T>(model);
            //handler.OnCompleted((x) => {
            //    OnPullComplete(x[Key]);
            //});

            PullData<T>(userid,Key).OnCompleted((x) => {
                OnPullComplete(x[Key]);
            });
        }

        public static DBInvokeHandler<Dictionary<string,T>> PullData<T>(string userid, string Key)
        {
            DynamoDBQueryModel model = new DynamoDBQueryModel()
            {
                TableName = memService.TableName,
                PartitionKey = new DynamoDBKeyModel()
                {
                    Name = Key,
                    Value = userid
                },
                AttributesToGet = new List<string>() {
                    Key
                }
            };
            
            var handler = DynamoDBUtil.Instance.GetItemByPrimaryKeyWithinDictionary<T>(model);
            return handler;
        }



    }
}