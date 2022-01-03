using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class PlayerQuitCMD : AbstractCommand
	{
		public VC_UserInfo info;

		protected override void OnExcute()
		{
			PlayerQuitEvent e = new PlayerQuitEvent
			{
				info = info
			};
			this.TriggerEvent<PlayerQuitEvent>(e);
		}
	}
}
