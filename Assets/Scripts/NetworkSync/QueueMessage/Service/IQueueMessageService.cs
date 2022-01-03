using System;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public interface IQueueMessageService:IService
	{
		void ProcessMessage();
		void PushEventMessage(IEvent eventMessage, Action onMessageSendFailed);
	}
}