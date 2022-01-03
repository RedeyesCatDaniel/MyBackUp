using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using LGUVirtualOffice;

namespace LGUVirtualOffice 
{
    /// <summary>
    /// 脚本挂载在 LeftStatusBar_Unfold 面板，点击团队和嘉宾切换成员列表
    /// </summary>
    public class LeftStatusBar_UnfoldPanel : AbstractController
    {
        public GameObject obj_Team;
        public GameObject obj_Guest;

        private Button btn_Team;    // 团队按钮
        private Button btn_Guest;   // 嘉宾按钮

        private TextMeshProUGUI text_Team;
        private TextMeshProUGUI text_Guest;

        public GameObject obj_TeamList;    // 团队成员列表
        public GameObject obj_GuestList;   // 嘉宾成员列表


        public GameObject blackDot; // 黑色点
        private int index;          // 黑色点索引
        private double listCount;

        public Button leftArrows;   // 左箭头按钮
        public Button rightArrows;  // 右箭头按钮

        public GameObject obj_Memeber_Team;  // 团队成员列表的Obj
        public GameObject obj_Memeber_Guest; // 嘉宾成员列表的Obj

        private List<GameObject> teamMemberList_1;
        private List<GameObject> teamMemberList_2;
        private List<GameObject> teamMemberList_3;

        void ListMember()
        {
            if(obj_Memeber_Team.transform.childCount<=9)
                teamMemberList_1 = new List<GameObject>();
            else if (obj_Memeber_Team.transform.childCount <18)
            {
                teamMemberList_1 = new List<GameObject>();
                teamMemberList_2 = new List<GameObject>();
            }
            else if(obj_Memeber_Team.transform.childCount < 27)
            {
                teamMemberList_1 = new List<GameObject>();
                teamMemberList_2 = new List<GameObject>();
                teamMemberList_3 = new List<GameObject>();
            }


            

            int count = 0; //计数器
            for (int i = 0; i < obj_Memeber_Team.transform.childCount; i++)
            {
                if(obj_Memeber_Team.transform.GetChild(i).gameObject.name!=MemberUserInfo.Instance.My_UserInfo.UserName)
                {
                    count++;
                    if(count<=9)
                        teamMemberList_1.Add(obj_Memeber_Team.transform.GetChild(i).gameObject);
                    else if(count>9 && count<=18)
                        teamMemberList_2.Add(obj_Memeber_Team.transform.GetChild(i).gameObject);
                    else if(count>18 && count<=27)
                        teamMemberList_3.Add(obj_Memeber_Team.transform.GetChild(i).gameObject);
                }
            }

        }

        void Start()
        {
            ListMember();

            btn_Team = obj_Team.GetComponent<Button>();
            btn_Guest = obj_Guest.GetComponent<Button>();

            text_Team = obj_Team.GetComponent<TextMeshProUGUI>();
            text_Guest = obj_Guest.GetComponent<TextMeshProUGUI>();

            double listCount_Team = Math.Ceiling((double)obj_Memeber_Team.transform.childCount / 9);   // 成员页面的页数
            double listCount_Guest = Math.Ceiling((double)obj_Memeber_Guest.transform.childCount / 9);  // 成员页面的页数
            listCount = listCount_Team; // 初始化操作



            // 点击嘉宾按钮
            btn_Team.onClick.AddListener(() => {

                ChangeList(obj_TeamList); // 切换团队List列表

                // 切换字体颜色
                text_Team.color = new Color32(0, 0, 0, 255);
                text_Guest.color = new Color32(145, 145, 145, 255);

                listCount = listCount_Team; // 不同情况下黑点所在列表

                index = 0;
                blackDot.transform.SetSiblingIndex(index); // 黑点初始化
            });
            // 点击团队按钮
            btn_Guest.onClick.AddListener(() => {

                ChangeList(obj_GuestList); // 切换到嘉宾List列表

                // 切换字体颜色
                text_Guest.color = new Color32(0, 0, 0, 255);
                text_Team.color = new Color32(145, 145, 145, 255);

                listCount = listCount_Guest; // 不同情况下黑点所在列表

                index = 0;
                blackDot.transform.SetSiblingIndex(index); // 黑点初始化
            });

            // 点击左箭头
            leftArrows.onClick.AddListener(() => {
                index--;
                if (index < 0)
                    index += (int)listCount;
                Index(index);
                blackDot.transform.SetSiblingIndex(index);
            });
            // 点击右箭头
            rightArrows.onClick.AddListener(() => {
                index++;
                if (index >= listCount)
                    index -= (int)listCount;
                Index(index);
                blackDot.transform.SetSiblingIndex(index);
            });
        }

        /// <summary>
        /// 根据情况打开对应的 List 列表
        /// </summary>
        /// <param name="index"></param>
        private void Index(int index)
        {
            if (index == 0)
                OpenList_x(obj_Memeber_Team, teamMemberList_1);
            else if (index == 1)
                OpenList_x(obj_Memeber_Team, teamMemberList_2);
            else if (index == 2)
                OpenList_x(obj_Memeber_Team, teamMemberList_3);
        }

        /// <summary>
        /// 打开对应的List,关闭其他
        /// </summary>
        /// <param name="obj"> Member父物体 </param>
        /// <param name="list_obj"> 要打开的列表 </param>
        private void OpenList_x(GameObject obj, List<GameObject> list_obj)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                obj.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < list_obj.Count; i++)
            {
                list_obj[i].SetActive(true);
            }
        }

        /// <summary>
        /// 切换列表
        /// </summary>
        /// <param name="myList">需要打开的列表</param>
        private void ChangeList(GameObject myList)
        {
            obj_TeamList.SetActive(false);
            obj_GuestList.SetActive(false);

            myList.SetActive(true);
        }
    }

}

