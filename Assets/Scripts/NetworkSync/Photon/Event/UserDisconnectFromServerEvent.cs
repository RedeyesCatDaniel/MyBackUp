using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public struct UserDisconnectFromServerEvent : IEvent
	{
		public string UserId { get; set; }
	}
}