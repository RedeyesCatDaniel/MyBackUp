using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public struct DBGetUserInfoSuccessEvent : IEvent
	{
		public UserInfo UserInfo { get; set; }
	}
}