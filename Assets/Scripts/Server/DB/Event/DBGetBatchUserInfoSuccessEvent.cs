using System.Collections;
using System.Collections.Generic;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice 
{
    public struct DBGetBatchUserInfoSuccessEvent : IEvent
    {
        public List<UserInfo> UserList { get; set; }
    }
}
