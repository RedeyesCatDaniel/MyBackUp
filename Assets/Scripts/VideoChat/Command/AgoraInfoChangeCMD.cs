using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class AgoraInfoChangeCMD : AbstractCommand
	{
		public string agoraInfo;
		public VC_UserInfo info;

		protected override void OnExcute()
		{
			AgoraInfoChangeEvent e = new AgoraInfoChangeEvent
			{
				agoraInfo = agoraInfo,
				info = info
			};
			this.TriggerEvent<AgoraInfoChangeEvent>(e);
		}
	}
}
