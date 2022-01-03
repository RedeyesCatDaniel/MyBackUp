using UnityEngine;

namespace LGUVirtualOffice {
	public static class ReturnMessageConst
	{
		public static string log_User_Not_Exist = "User Not Exist,Please Check Your Input";
		public static string log_UserName_Incorrect = "Parameter Incollect,Please Check Your Input";
		public static string log_Team_Incorrect = "Team Incorrect,Please Try Another One";
		public static string log_Status_Wrong = "Login status Wrong!";
		public static string sys_System_Error = "System Error,Please Try Again!";

		//Connect To photon Server Failed
		public static string photon_Connect_Fail = "Connect To Server Failed!";
		//Join Workspace Failed
		public static string photon_Join_Room_Failed = "Join Workspace Failed!";
		//Not a Registered Workspace
		public static string photon_Room_Not_Exist = "Not a Registered Workspace!";
		//Workspace Exceed The Quota
		public static string photon_Room_Full = "Workspace Exceed The Quota!";
		public static string photon_Same_Room = "Same Workspace!";
	}
}