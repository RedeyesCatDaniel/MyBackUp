using LGUVirtualOffice.Framework;
using System.Collections.Generic;


namespace LGUVirtualOffice
{
	public interface IVCUserService : IService
	{
		public VC_UserInfo GetUserInfo(uint uid);
		public void ResetState();
		public void LoadInfo(VC_UserInfo info);
		public void UnLoadInfo(VC_UserInfo info);
		public void PutInTemp(VC_UserInfo info);
		public void PutInWork(VC_UserInfo info);
		public void LeaveAround(VC_UserInfo info);
		public void ScrollQueue(bool isScrollRight);
		public bool GetLocalPushState(bool isVideo);
		public void SetUserAlpha(VC_UserInfo info, float alpha);
		public float GetUserAlpha(VC_UserInfo info);
		public List<VC_UserInfo> GetWorkUsers();
		public List<VC_UserInfo> GetTempUsers();
		public VC_UserInfo GetLocalInfo();
		public void SetUIController(VC_UI_Controller ctr);
	}
}

