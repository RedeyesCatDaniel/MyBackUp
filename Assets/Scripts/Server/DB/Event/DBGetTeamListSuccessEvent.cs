using System.Collections.Generic;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public struct DBGetTeamListSuccessEvent : IEvent
	{
		public List<TeamModel> TeamList { get; private set; }
		public DBGetTeamListSuccessEvent(List<TeamModel> teamList) 
		{
			TeamList = teamList;
		}
	}
}