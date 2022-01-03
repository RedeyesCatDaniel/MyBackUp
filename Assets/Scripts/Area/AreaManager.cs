using LGUVirtualOffice.Framework;
using System.Collections.Generic;


namespace LGUVirtualOffice
{
	public class AreaManager : AbstractService, IAreaManager
	{
		public static VCRuleEnum CurRule { get; private set; }
		public static WorkspaceAreaEnum CurArea { get; private set; }


		public List<BaseArea> Areas;
		public VC_UserInfo localInfo { get; private set; }



		protected override void OnInit()
		{
			CurArea = WorkspaceAreaEnum.Default;
			CurRule = VCRuleEnum.Default;
			Areas = new List<BaseArea>();
			var e = new WorkspaceAreaEnum();
			string[] areas = System.Enum.GetNames(e.GetType());
			for (int i = 0; i < areas.Length; i++)
			{
				BaseArea area = InitailizeArea((WorkspaceAreaEnum)i);
				Areas.Add(area);
			}
			this.SubscribeEvent<PlayerCreatSuccessedEvent>((e) => { localInfo = e.localInfo; });
			this.SubscribeEvent<AreaChangeEvent>((e) => LocalChangeArea(e.Area));
		}

		public List<VC_UserInfo> GetTargetAreaUsers(WorkspaceAreaEnum areaType)
		{
			BaseArea area = GetBaseArea(areaType);
			if (area == null)
				return null;
			return area.users;
		}

		BaseArea InitailizeArea(WorkspaceAreaEnum areaEnum)
		{
			BaseArea area = null;
			switch (areaEnum)
			{
				default:
					area = new AreaDefault();
					area.Initailize("Defalut", areaEnum, VCRuleEnum.Default, 999);
					break;
				case WorkspaceAreaEnum.Hallway:
					area = new AreaHallway();
					area.Initailize("Hallway", areaEnum, VCRuleEnum.Hallway, 17);
					break;
				case WorkspaceAreaEnum.WorkArea:
					area = new AreaWorkArea();
					area.Initailize("WorkArea", areaEnum, VCRuleEnum.Fade, 17);
					break;
				case WorkspaceAreaEnum.WorkAreaTable1:
					area = new AreaWorkAreaTable();
					area.Initailize("WorkAreaTable1", areaEnum, VCRuleEnum.AutoConnect, 4);
					break;
				case WorkspaceAreaEnum.WorkAreaTable2:
					area = new AreaWorkAreaTable();
					area.Initailize("WorkAreaTable2", areaEnum, VCRuleEnum.AutoConnect, 4);
					break;
				case WorkspaceAreaEnum.WorkAreaTable3:
					area = new AreaWorkAreaTable();
					area.Initailize("WorkAreaTable3", areaEnum, VCRuleEnum.AutoConnect, 4);
					break;
				case WorkspaceAreaEnum.WorkAreaTable4:
					area = new AreaWorkAreaTable();
					area.Initailize("WorkAreaTable4", areaEnum, VCRuleEnum.AutoConnect, 4);
					break;
				case WorkspaceAreaEnum.WorkAreaTable5:
					area = new AreaWorkAreaTable();
					area.Initailize("WorkAreaTable5", areaEnum, VCRuleEnum.AutoConnect, 4);
					break;
				case WorkspaceAreaEnum.LongTable:
					area = new AreaLongTable();
					area.Initailize("LongTable", areaEnum, VCRuleEnum.AutoConnect, 17);
					break;
				case WorkspaceAreaEnum.Carpet1:
					area = new AreaCarpet();
					area.Initailize("Carpet1", areaEnum, VCRuleEnum.AutoConnect, 17);
					break;
				case WorkspaceAreaEnum.Carpet2:
					area = new AreaCarpet();
					area.Initailize("Carpet2", areaEnum, VCRuleEnum.AutoConnect, 17);
					break;
				case WorkspaceAreaEnum.Carpet3:
					area = new AreaCarpet();
					area.Initailize("Carpet3", areaEnum, VCRuleEnum.AutoConnect, 17);
					break;
				case WorkspaceAreaEnum.PrivateRoom1:
					area = new AreaPrivateRoom();
					area.Initailize("PrivateRoom1", areaEnum, VCRuleEnum.AutoConnect, 2);
					break;
				case WorkspaceAreaEnum.PrivateRoom2:
					area = new AreaPrivateRoom();
					area.Initailize("PrivateRoom2", areaEnum, VCRuleEnum.AutoConnect, 2);
					break;
				case WorkspaceAreaEnum.PrivateRoom3:
					area = new AreaPrivateRoom();
					area.Initailize("PrivateRoom3", areaEnum, VCRuleEnum.AutoConnect, 2);
					break;
				case WorkspaceAreaEnum.PrivateRoom4:
					area = new AreaPrivateRoom();
					area.Initailize("PrivateRoom4", areaEnum, VCRuleEnum.AutoConnect, 2);
					break;
			}
			return area;
		}


		public void LocalChangeArea(WorkspaceAreaEnum area)
		{
			if (localInfo == null)
				return;
			// 本地玩家变更，进行试图变更操作

			CurArea = area;
			CurRule = GetBaseArea(area).VCRule;

		}

		public BaseArea GetBaseArea(WorkspaceAreaEnum areaType)
		{
			foreach (BaseArea area in Areas)
			{
				if (area.AreaType == areaType)
				{
					return area;
				}
			}
			return null;
		}

		public bool LocalTryToSwitchArea(VC_UserInfo localInfo, WorkspaceAreaEnum areaType)
		{
			BaseArea oldArea = GetBaseArea(localInfo.Area);
			if (oldArea != null)
				oldArea.LocalLeaveArea(localInfo);
			BaseArea area = GetBaseArea(areaType);
			if (area == null) return false;
			return area.LocalTryToEnterArea(localInfo);
		}

		public void RemoteSwitchArea(VC_UserInfo info, WorkspaceAreaEnum targetArea)
		{
			BaseArea oldArea = GetBaseArea(info.Area);
			if (oldArea != null)
				oldArea.RemoteLeaveArea(info);
			BaseArea newArea = GetBaseArea(targetArea);
			if (newArea == null) return;
			newArea.RemoteEnterArea(info);
		}
	}
}
