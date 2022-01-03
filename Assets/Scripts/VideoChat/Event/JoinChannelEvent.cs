using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public struct JoinChannelEvent : IEvent
	{
		public string channelName;
		public uint uid;
	}
}
