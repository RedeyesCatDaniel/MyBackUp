using LGUVirtualOffice.Framework;
using UnityEngine;

namespace LGUVirtualOffice
{
	public class AreaCollider : AbstractController
	{
		public WorkspaceAreaEnum thisArea;
		public WorkspaceAreaEnum outArea;


		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "LocalPlayer")
			{
				PlayerEnterAreaCMD cmd = new PlayerEnterAreaCMD
				{
					area = thisArea
				};
				this.SendCommand<PlayerEnterAreaCMD>(cmd);
			}
		}
		private void OnTriggerExit(Collider other)
		{
			if (outArea == WorkspaceAreaEnum.Default)
				return;
			if (other.tag == "LocalPlayer")
			{
				PlayerEnterAreaCMD cmd = new PlayerEnterAreaCMD
				{
					area = outArea
				};
				this.SendCommand<PlayerEnterAreaCMD>(cmd);
			}
		}
	}
}
