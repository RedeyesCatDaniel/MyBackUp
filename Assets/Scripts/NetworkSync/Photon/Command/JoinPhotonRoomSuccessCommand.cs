using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public class JoinPhotonRoomSuccessCommand : AbstractCommand
	{
        protected override void OnExcute()
        {
            this.TriggerEvent<JoinPhotonRoomSuccessEvent>();
        }
	}
}