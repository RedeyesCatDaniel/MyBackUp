using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class PlayAnimationCMD : AbstractCommand
	{
		public int animID;

		protected override void OnExcute()
		{
			PlayAnimationEvent e = new PlayAnimationEvent
			{
				animID = animID
			};
			this.TriggerEvent<PlayAnimationEvent>(e);
		}
	}
}
