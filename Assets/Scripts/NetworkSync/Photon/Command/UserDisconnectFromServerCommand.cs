using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public class UserDisconnectFromServerCommand : AbstractCommand
	{
        protected override void OnExcute()
        {
            this.TriggerEvent<DisconnectFromServerEvent>();
        }
	}
}