using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public struct JoinResultEvent : IEvent
	{
		public WorkspaceAreaEnum Area;
		public bool isSuccessed;
	}
}
