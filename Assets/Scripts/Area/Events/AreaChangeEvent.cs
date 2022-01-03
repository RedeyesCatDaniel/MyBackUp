using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	/// <summary>
	/// 这是向服务器传送玩家区域的事件
	/// </summary>
	public struct AreaChangeEvent : IEvent
	{
		public string userID;
		public string workSpace;
		public WorkspaceAreaEnum Area;
	}
}

