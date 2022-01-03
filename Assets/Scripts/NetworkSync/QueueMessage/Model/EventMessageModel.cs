using System;
namespace LGUVirtualOffice {
	public class EventMessageModel
	{
		public DateTime MessageSendTime { get; set; }
		public string EventTypeName { get; set; }
		public string EventData { get; set; }
	}
}