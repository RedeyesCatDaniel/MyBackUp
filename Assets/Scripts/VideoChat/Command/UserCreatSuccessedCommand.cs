using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class UserCreatSuccessedCommand : AbstractCommand
	{
		public VC_UserInfo localInfo;
		protected override void OnExcute()
		{
			this.TriggerEvent<PlayerCreatSuccessedEvent>(new PlayerCreatSuccessedEvent { localInfo = localInfo });
		}
	}
}
