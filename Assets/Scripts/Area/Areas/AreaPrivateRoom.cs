using System.Collections.Generic;

namespace LGUVirtualOffice
{
	public class AreaPrivateRoom : BaseArea
	{
#if Cat_Next_Version
		bool isAllowedToJoin;
		bool isWaitingToJoin;

		public override void Initailize(string ChannelName, WorkspaceAreaEnum AreaType, VCRuleEnum VCRule, int NumberLimit = 17)
		{
			base.Initailize(ChannelName, AreaType, VCRule, NumberLimit);

			isAllowedToJoin = isWaitingToJoin = false;
		}

		public override bool LocalTryToEnterArea(VC_UserInfo info)
		{
			if (users == null) users = new List<VC_UserInfo>();

			// 已经在等待加入，返还加入状态
			if (isWaitingToJoin)
			{
				if (isAllowedToJoin)
				{
					isAllowedToJoin = false;
					return true;
				}
			}

			// 房间人数允许加入
			if (users.Count < NumberLimit)
			{
				// 弹出面板<进行创建或输入密码>，对其进行初始化


				// 发出等待命令
				isWaitingToJoin = true;
				return false;
			}
			// 房间已有2人
			else
			{
				return false;
			}
		}

		public override void LocalLeaveArea(VC_UserInfo info)
		{
			base.LocalLeaveArea(info);
			isAllowedToJoin = isWaitingToJoin = false;
		}
#endif

	}
}
