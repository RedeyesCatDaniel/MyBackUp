using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class LeaveChannelCommand : AbstractCommand
	{

		protected override void OnExcute()
		{
			this.TriggerEvent<LeaveChannelEvent>(new LeaveChannelEvent());
		}
	}
}
