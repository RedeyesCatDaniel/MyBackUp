using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public class RoomPropertyUpdateCommand : AbstractCommand
	{
        protected override void OnExcute()
        {
            this.TriggerEvent<RoomPropertyUpdateEvent>();
        }
	}
}
