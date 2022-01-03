using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using LGUVirtualOffice.Framework;


namespace LGUVirtualOffice
{
	public class BaseArea
	{
		public List<VC_UserInfo> users;

		public string ChannelName;
		public WorkspaceAreaEnum AreaType;
		public VCRuleEnum VCRule;
		public int NumberLimit;

		public virtual void Initailize(string ChannelName, WorkspaceAreaEnum AreaType, VCRuleEnum VCRule, int NumberLimit = 17)
		{
			this.ChannelName = ChannelName;
			this.AreaType = AreaType;
			this.VCRule = VCRule;
			this.NumberLimit = NumberLimit;
		}


		/// <summary>
		/// 本地进入规则
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public virtual bool LocalTryToEnterArea(VC_UserInfo info)
		{
			if (users == null) users = new List<VC_UserInfo>();

			if (users.Count < NumberLimit)
			{
				if (!users.Contains(info))
					users.Add(info);
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 本地离开此区域
		/// </summary>
		public virtual void LocalLeaveArea(VC_UserInfo info)
		{
			if (users != null)
			{
				if (users.Contains(info))
				{
					users.Remove(info);
				}
			}
		}

		public void RemoteEnterArea(VC_UserInfo info)
		{
			if (users == null) users = new List<VC_UserInfo>();
			if (!users.Contains(info))
				users.Add(info);
		}

		public void RemoteLeaveArea(VC_UserInfo info)
		{
			if (users != null)
			{
				if (users.Contains(info))
				{
					users.Remove(info);
				}
			}
		}
	}
}

