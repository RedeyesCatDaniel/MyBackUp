using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    public struct UserStatusModifiedEvent : IEvent
    {
        public string UserId { get; set; }
        public int NewState { get; set; }
    }
}
