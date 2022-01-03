using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public struct StreamStateEvent : IEvent
	{
		public VC_UserInfo info;
		public int state;
	}
}
