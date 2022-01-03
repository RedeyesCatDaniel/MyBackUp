using System;
using System.Reflection;
using LGUVirtualOffice.Framework;
using Newtonsoft.Json;

namespace LGUVirtualOffice {
    public class QueueMessageService : AbstractService,IQueueMessageService
    {
        private IMessageQueueUtility messageQueueUtility;
        protected override void OnInit()
        {
            messageQueueUtility = this.GetUtility<IMessageQueueUtility>();
        }

        public void ProcessMessage() 
        {
            messageQueueUtility.GetMessage((eventMessageList) => {
                if (eventMessageList == null)
                {
                    return;
                }
                LogUtil.LogDebug("eventMessageList count:" + eventMessageList.Count);
                foreach (var eventMessage in eventMessageList)
                {
                    IEvent e=GetEvent(eventMessage);
                    this.TriggerEvent(e);
                }
            });
        }
        public void PushEventMessage(IEvent eventMessage,Action onMessageSendFailed) 
        {
            EventMessageModel message = new EventMessageModel();
            message.EventTypeName = eventMessage.GetType().FullName;
            message.EventData = JsonConvert.SerializeObject(eventMessage);
            message.MessageSendTime = DateTime.Now;
            messageQueueUtility.SendMessage(message, onMessageSendFailed);
        }
        private IEvent GetEvent(EventMessageModel eventMessage) 
        {
            Type eventType = Type.GetType(eventMessage.EventTypeName);
            object eventInstance = JsonConvert.DeserializeObject(eventMessage.EventData, eventType);
            return (IEvent)eventInstance;
        }
    }
}



