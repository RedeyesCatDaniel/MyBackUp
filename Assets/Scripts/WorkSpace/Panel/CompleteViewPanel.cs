using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 收起状态点击 “。。。”或者展开状态点击 “完整试图”弹出的面板
    /// </summary>
    public class CompleteViewPanel : AbstractController, IPointerEnterHandler, IPointerExitHandler
    {
        private bool controllerPanel;
        public void OnPointerEnter(PointerEventData eventData)
        {
            controllerPanel = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            controllerPanel = false;
        }


        public GameObject PersonalInformationPanel; // 点击用户之后，出现的子面板---个人信息面板

        public TextMeshProUGUI text_teamName;
        public TextMeshProUGUI text_5_24;

        public Toggle toggle_All;
        public Toggle toggle_Team;
        public Toggle toggle_Guest;

        public GameObject obj_All;
        public GameObject obj_Team;
        public GameObject obj_Guest;

        public GameObject obj_AllContent;  // AllMember预制体父物体
        public GameObject obj_TeamContent; // TeamMember预制体父物体
        public GameObject obj_GuestContent; // TeamMember预制体父物体

        void Start()
        {
            text_teamName.text = MemberUserInfo.Instance.My_UserInfo.TeamInfo.TeamName + " 전체보기";
            text_5_24.text = "5/" + (MemberUserInfo.Instance.TeamMemberList.Count - 1+ MemberUserInfo.Instance.TeamGuestList.Count).ToString();

            Instantiate_MyTeamList(obj_AllContent, MemberUserInfo.Instance.TeamMemberList);
            Instantiate_GuestList(obj_AllContent, MemberUserInfo.Instance.TeamGuestList);

            Instantiate_MyTeamList(obj_TeamContent, MemberUserInfo.Instance.TeamMemberList);
            Instantiate_GuestList(obj_GuestContent, MemberUserInfo.Instance.TeamGuestList);

            toggle_All.onValueChanged.AddListener((bool value) =>
            {
                if (value)
                    ChangeList(obj_All);
            });
            toggle_Team.onValueChanged.AddListener((bool value) =>
            {
                if (value)
                    ChangeList(obj_Team);
            });
            toggle_Guest.onValueChanged.AddListener((bool value) =>
            {
                if (value)
                    ChangeList(obj_Guest);
            });
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !controllerPanel && PersonalInformationPanel.activeSelf == false)
            {
                this.gameObject.SetActive(false);
            }
        }

        private void ChangeList(GameObject myList)
        {
            obj_All.SetActive(false);
            obj_Team.SetActive(false);
            obj_Guest.SetActive(false);

            myList.SetActive(true);
        }

        /// <summary>
        /// 加载预制体
        /// </summary>
        /// <param name="parent"> 预制体父物体 </param>
        /// <param name="list1"> MyTeam </param>
        private void Instantiate_MyTeamList(GameObject parent, List<UserInfo> list1)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                GameObject my_prefab = Resources.Load<GameObject>(RequiredStringManager.str_CompleteView_UserMember);
                if (MemberUserInfo.Instance.My_UserInfo.UserName != list1[i].UserName)
                {
                    // 实例化预制体并设置父对象
                    my_prefab = Instantiate(my_prefab, parent.transform);

                    my_prefab.name = list1[i].UserName;

                    my_prefab.GetComponent<CompleteView_UserMember>().my_UserInfo = list1[i];
                }
            }
          
        }
        /// <summary>
        /// 加载预制体
        /// </summary>
        /// <param name="parent"> 预制体父物体 </param>
        /// <param name="list2"> Guest </param>
        private void Instantiate_GuestList(GameObject parent,List<UserInfo> list2)
        {
            for (int i = 0; i < list2.Count; i++)
            {
                GameObject my_prefab = Resources.Load<GameObject>(RequiredStringManager.str_CompleteView_UserMember);
                // 实例化预制体并设置父对象
                my_prefab = Instantiate(my_prefab, parent.transform);

                my_prefab.name = list2[i].UserName;

                my_prefab.GetComponent<CompleteView_UserMember>().my_UserInfo = list2[i];
            }
        }


    }

}

