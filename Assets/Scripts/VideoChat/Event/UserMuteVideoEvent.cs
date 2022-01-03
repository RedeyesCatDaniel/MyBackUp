using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public struct UserMuteVideoEvent : IEvent
	{
		public uint uid;
		public bool isMuted;
	}
}
