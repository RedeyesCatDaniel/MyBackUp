using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public struct LocalPushStateEvent : IEvent
	{
		public bool isVideo;
		public bool isMuted;
	}
}
