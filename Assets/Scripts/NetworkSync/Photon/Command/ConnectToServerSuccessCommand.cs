using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public class ConnectToServerSuccessCommand : AbstractCommand
	{
        protected override void OnExcute()
        {
            this.TriggerEvent<ConnectToServerSuccessEvent>();
        }
	}
}