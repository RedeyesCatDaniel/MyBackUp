using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class MemberUserInfo : Singleton<MemberUserInfo>
    {
        private MemberUserInfo() { }

        private UserInfo my_UserInfo;
        private List<UserInfo> teamMemberList;
        private List<UserInfo> teamGuestList;

        public UserInfo My_UserInfo { get => my_UserInfo; set => my_UserInfo = value; }
        public List<UserInfo> TeamMemberList { get => teamMemberList; set => teamMemberList = value; }
        public List<UserInfo> TeamGuestList { get => teamGuestList; set => teamGuestList = value; }
    }

}
