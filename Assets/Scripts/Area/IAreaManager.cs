using LGUVirtualOffice.Framework;
using System.Collections.Generic;

namespace LGUVirtualOffice
{
	public interface IAreaManager : IService
	{
		public void LocalChangeArea(WorkspaceAreaEnum area);
		public List<VC_UserInfo> GetTargetAreaUsers(WorkspaceAreaEnum area);
		public BaseArea GetBaseArea(WorkspaceAreaEnum areaType);
		public bool LocalTryToSwitchArea(VC_UserInfo localInfo, WorkspaceAreaEnum areaType);
		public void RemoteSwitchArea(VC_UserInfo info, WorkspaceAreaEnum targetArea);
	}
}
