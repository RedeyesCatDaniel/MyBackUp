using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice 
{
    public struct UserSignatureModifiedEvent : IEvent
    {
        public string UserId { get; set; }
        public string NewSignature { get; set; }
    }
}
