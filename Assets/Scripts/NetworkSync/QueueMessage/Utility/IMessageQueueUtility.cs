using System;
using System.Collections.Generic;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public interface IMessageQueueUtility:IUtility
	{
		void SendMessage(EventMessageModel message, Action onMessageSendFailed, string queueUrl = null);
		void SendMessageBatch(List<EventMessageModel>  messageList,Action<List<EventMessageModel>> onMessageSendFailed, string queueUrl = null);
		void GetMessage(Action<List<EventMessageModel>> onMessageReceived, string queueUrl = null);
	}
}