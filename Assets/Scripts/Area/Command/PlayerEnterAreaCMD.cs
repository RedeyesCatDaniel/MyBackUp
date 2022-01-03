using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class PlayerEnterAreaCMD : AbstractCommand
	{
		public WorkspaceAreaEnum area;
		protected override void OnExcute()
		{
			this.TriggerEvent<PlayerEnterAreaEvent>(new PlayerEnterAreaEvent
			{
				Area = area
			});
		}
	}
}
