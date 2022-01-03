using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
	public class AreaChangeCommand : AbstractCommand
	{
		public string userID;
		public string workSpace;
		public WorkspaceAreaEnum area;
		private IQueueMessageService queueMessageService;


		protected override void OnExcute()
		{
			this.TriggerEvent<AreaChangeEvent>(new AreaChangeEvent
			{
				userID = userID,
				workSpace = workSpace,
				Area = area
			});
			//queueMessageService = this.GetService<IQueueMessageService>();
			//queueMessageService.PushEventMessage(new AreaChangeNetEvent
			//{
			//	userID = userID,
			//	Area = area
			//}, null);
		}
	}
}
