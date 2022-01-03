using UnityEngine;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public struct UserJoinWorkSpaceEvent : IEvent
	{
		public string WorkSpaceNowIn { get; set; }
		public string UserId { get; set; }
	}
}