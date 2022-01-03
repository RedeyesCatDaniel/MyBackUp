using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class RefreshViewCommand : AbstractCommand
	{
		protected override void OnExcute()
		{
			this.TriggerEvent<RefreshViewEvent>(new RefreshViewEvent());
		}
	}
}
