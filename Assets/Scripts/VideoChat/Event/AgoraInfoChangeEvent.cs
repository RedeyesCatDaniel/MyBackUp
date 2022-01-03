using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	/// <summary>
	/// 当Agora信息发生变更时触发此消息，有UserService接受并进行更新
	/// </summary>
	public struct AgoraInfoChangeEvent : IEvent
	{
		public string agoraInfo;
		public VC_UserInfo info;
	}
}
