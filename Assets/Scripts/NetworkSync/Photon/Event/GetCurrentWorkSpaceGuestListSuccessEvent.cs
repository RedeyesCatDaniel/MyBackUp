using System.Collections.Generic;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    public struct GetCurrentWorkSpaceGuestListSuccessEvent : IEvent
    {
        public List<UserInfo> GuestList { get; set; }
    }
}
