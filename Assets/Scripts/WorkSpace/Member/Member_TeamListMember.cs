using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class Member_TeamListMember : AbstractController
    {
        public GameObject obj_TeamList_parent;

        void Start()
        {
            Instantiate_MemberList_Prefabs();
        }

        // Update is called once per frame
        void Update()
        {

        }
        // 加载预制体
        void Instantiate_MemberList_Prefabs()
        {
            GameObject my_prefab = Resources.Load<GameObject>(RequiredStringManager.str_Member_Team_Listmember);
            for (int i = 0; i < UIPanelData_Model.Instance.my_Team.Count; i++)
            {
                GameObject newGO = Instantiate(my_prefab, obj_TeamList_parent.transform);
                newGO.name = UIPanelData_Model.Instance.my_Team[i].TeamName;

                //UserInfo userInfo = MemberUserInfo.Instance.My_UserInfo; // 初始化 UserInfo
                //userInfo.TeamInfo.TeamName = UIPanelData_Model.Instance.my_Team[i].TeamName;
                //userInfo.TeamInfo.TeamCode = UIPanelData_Model.Instance.my_Team[i].TeamCode;
                //newGO.GetComponent<TeamMemberSort>().SetUserInfo(userInfo);
                newGO.GetComponent<TeamMemberSort>().text_TeamCode = UIPanelData_Model.Instance.my_Team[i].TeamCode;
                newGO.GetComponent<TeamMemberSort>().text_teamName.text = UIPanelData_Model.Instance.my_Team[i].TeamName;
                if (UIPanelData_Model.Instance.my_Team[i].TeamName == MemberUserInfo.Instance.My_UserInfo.TeamInfo.TeamName)
                    newGO.GetComponent<TeamMemberSort>().text_Number.text = (MemberUserInfo.Instance.TeamMemberList.Count - 1).ToString();
            }

        }

    }
}

