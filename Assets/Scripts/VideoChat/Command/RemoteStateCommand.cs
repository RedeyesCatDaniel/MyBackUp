using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class RemoteStateCommand : AbstractCommand
	{
		public VC_UserInfo info;
		/// <summary>
		/// 目标状态： 0，offline;1,temp;2,work
		/// </summary>
		public int targetState;

		protected override void OnExcute()
		{
			this.TriggerEvent<StreamStateEvent>(new StreamStateEvent { info = info, state = targetState });
		}
	}
}
