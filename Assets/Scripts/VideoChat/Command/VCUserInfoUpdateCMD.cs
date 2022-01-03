using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class VCUserInfoUpdateCMD : AbstractCommand
	{
		public VC_UserInfo info;

		protected override void OnExcute()
		{
			this.TriggerEvent<VCUserInfoUpdateEvent>(new VCUserInfoUpdateEvent { info = info });
		}
	}
}
