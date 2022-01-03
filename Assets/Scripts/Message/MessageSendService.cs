using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGUVirtualOffice {
    public class MessageSendService : AbstractService
    {
        private IQueueMessageService queueMessageService;
        private IDBUtility dBUtility;
        private UserInfo userInfo;
        protected override void OnInit()
        {
            userInfo = this.GetModel<UserInfo>();
            queueMessageService = this.GetService<IQueueMessageService>();
            dBUtility = this.GetUtility<IDBUtility>();
        }

        public DBInvokeHandler<bool> PushMessage(string chatchannel,string Reciever,string content)
        {
            DynamoDBUpdateModel model = new DynamoDBUpdateModel()
            {
                TableName = "LGU_Message",
                PartitionKey = new DynamoDBKeyModel
                {
                    Name = "ChatChannel",
                    Value = chatchannel
                },
                SortKey = new DynamoDBKeyModel
                {
                    Name = "SendTime",
                    Value = DynamoDBDataConverter.Instance.ConverteDateTimeToString(System.DateTime.Now)
                },
                items = new Dictionary<string, object>
                {
                    {"Conversation",content },
                    {"IsRead",false},
                    {"Reciever",Reciever },
                    {"Sender","33333" }
                }
            };

            return dBUtility.AddItem(model);
        }

        public DBInvokeHandler<Dictionary<string,object>> CheckIsRead(string chatchannel,string sendtime)
        {
            DynamoDBQueryModel model = new DynamoDBQueryModel()
            {
                TableName = "LGU_Message",
                PartitionKey = new DynamoDBKeyModel
                {
                    Name = "ChatChannel",
                    Value = chatchannel
                },
                SortKey = new DynamoDBKeyModel
                {
                    Name = "SendTime",
                    Value = sendtime
                }
            };
            return dBUtility.GetItemByPrimaryKeyWithinDictionary<object>(model);
        }



    }
}