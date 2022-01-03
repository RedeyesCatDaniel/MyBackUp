using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice {
	public interface ILoginService : IService
	{
		public void Login(UserInfo userInfo);
		public void Logout(string userId=null);
	}
}