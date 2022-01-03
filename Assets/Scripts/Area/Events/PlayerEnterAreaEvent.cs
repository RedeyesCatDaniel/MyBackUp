using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public struct PlayerEnterAreaEvent : IEvent
	{
		public VC_UserInfo info;
		public WorkspaceAreaEnum Area;
	}
}
