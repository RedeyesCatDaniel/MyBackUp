using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public class ConnectToServerFailedCommand : AbstractCommand
	{
        protected override void OnExcute()
        {
            this.TriggerEvent<ConnectToServerFailedEvent>();
        }
	}
}