using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    public struct UserPositionEvent : IEvent
    {
        public string UserId { get; set; }
        public string NewPosition { get; set; }
    }
}
