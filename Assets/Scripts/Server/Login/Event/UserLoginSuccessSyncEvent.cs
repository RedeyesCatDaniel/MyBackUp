using LGUVirtualOffice.Framework;
using System;

namespace LGUVirtualOffice {
	public class UserLoginSuccessSyncEvent : IEvent
	{
		public UserLoginSuccessSyncEvent() { }
		public string UserId { get; set; }
		public string TokenMD5 { get; set; }
		public DateTime LoginTime { get; set; }
	}
}