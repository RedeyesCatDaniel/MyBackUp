using System.Collections.Generic;
using Hashtable=ExitGames.Client.Photon.Hashtable;
using LGUVirtualOffice.Framework;
using Photon.Realtime;
using Photon.Pun;

namespace LGUVirtualOffice {
	public interface INetworkSyncService : IService
	{
		public void ConnectToServer(UserInfo userInfo);
		public void JoinWorkSpace(string workSpaceName = null);
		public void DisConnectFromPhontonServer();
		public void UpdateWorkSpaceList(List<RoomInfo> roomList);
		public void UpdateRoomGuestList(Player guestPlayer,bool enterRoom);
		public void UpdatePlayerProperties(Player targetPlayer,Hashtable playerProperties);
		public void SetPlayerVCInfo(string agoraid);
		public string GetPlayerVCInfo(PhotonView playerPhotonView);

		public UserInfo GetGuestInfo(string userId);

		public bool IsChangeWorkspace();

		///Get the infomation of  users who not belong to current workspace,
		///Listen to the GetCurrentWorkSpaceGuestListSuccessEvent to get the result
		public void GetCurrentWorkSpaceGuestList();
		//procedure the situation that a user quit the application not by a normal way
		public void CheckAfterOtherPlayerLeftWorkSpace(Player otherPlayer);
	}
}