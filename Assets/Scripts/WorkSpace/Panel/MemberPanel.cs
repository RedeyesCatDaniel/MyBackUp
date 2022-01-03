using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using LGUVirtualOffice;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 收起/展开状态点击 “联系人”弹出的面板
    /// </summary>
    public class MemberPanel : AbstractController, IPointerEnterHandler, IPointerExitHandler
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

        public TMP_InputField input_Search;

        public Button btn_Personal; // 个人按钮
        public Button btn_Team;     // 团队按钮

        public GameObject obj_TextPersonal_1; // 蓝色字
        public GameObject obj_TextTeam_1;     // 蓝色字

        public GameObject obj_Personal; // 个人列表
        public GameObject obj_Team;     // 团队列表

        public GameObject obj_leftUnderline;  // 左下划线
        public GameObject obj_RightUnderline; // 右下划线

        public GameObject obj_PersonalContent;  // 预制体父物体---个人
        public GameObject obj_TeamContent;      // 预制体父物体---团队

        void Start()
        {
            Instantiate_MemberList_Prefabs(obj_PersonalContent, RequiredStringManager.str_Member_Personal_UserMember);

            input_Search.onValueChanged.AddListener(delegate { ValueChangeCheck();});

            // 点击个人按钮之后
            btn_Personal.onClick.AddListener(() => {
                SetColorSwitch(obj_TextPersonal_1);
                SetListSwitch(obj_Personal);
                SetUnderlineSwitch(obj_leftUnderline);
            });

            // 点击团队按钮之后
            btn_Team.onClick.AddListener(() => {
                SetColorSwitch(obj_TextTeam_1);
                SetListSwitch(obj_Team);
                SetUnderlineSwitch(obj_RightUnderline);
            });
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !controllerPanel && PersonalInformationPanel.activeSelf == false)
            {
                this.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 输入框变化监听
        /// </summary>
        private void ValueChangeCheck()
        {
            if (obj_Personal.activeSelf == true )
            {
                // 初始化
                for (int i = 0; i < obj_PersonalContent.transform.childCount; i++)
                {
                    if (obj_PersonalContent.transform.GetChild(i).gameObject.activeSelf == false)
                        obj_PersonalContent.transform.GetChild(i).gameObject.SetActive(true);
                }

                for (int i = 0; i < obj_PersonalContent.transform.childCount; i++)
                {
                    if (!obj_PersonalContent.transform.GetChild(i).gameObject.name.Contains(input_Search.text))
                    {
                        obj_PersonalContent.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else if (obj_Team.activeSelf == true )
            {
                // 初始化
                for (int i = 0; i < obj_TeamContent.transform.childCount; i++)
                {
                    if (obj_TeamContent.transform.GetChild(i).gameObject.activeSelf == false)
                        obj_TeamContent.transform.GetChild(i).gameObject.SetActive(true);
                }
                for (int i = 0; i < obj_TeamContent.transform.childCount; i++)
                {
                    if (!obj_TeamContent.transform.GetChild(i).gameObject.name.Contains(input_Search.text))
                    {
                        obj_TeamContent.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

        }

        /// <summary>
        /// 选择要打开的蓝色字
        /// </summary>
        /// <param name="my_Switch">要打开的蓝色字</param>
        private void SetColorSwitch(GameObject my_Switch)
        {
            obj_TextPersonal_1.SetActive(false);
            obj_TextTeam_1.SetActive(false);

            my_Switch.SetActive(true);
        }

        /// <summary>
        /// 选择打开个人列表还是团队列表
        /// </summary>
        /// <param name="my_Switch">要打开的列表</param>
        private void SetListSwitch(GameObject my_Switch)
        {
            obj_Personal.SetActive(false);
            obj_Team.SetActive(false);

            my_Switch.SetActive(true);
        }

        /// <summary>
        /// 选择打开左下划线还是右下划线
        /// </summary>
        /// <param name="my_Switch">要打开的下划线</param>
        private void SetUnderlineSwitch(GameObject my_Switch)
        {
            obj_leftUnderline.SetActive(false);
            obj_RightUnderline.SetActive(false);

            my_Switch.SetActive(true);
        }

        /// <summary>
        /// 加载预制体，所在团队的加载
        /// </summary>
        /// <param name="parent">预制体父物体</param>
        private void Instantiate_MemberList_Prefabs(GameObject parent,string str_LoadPath)
        {
            for (int i = 0; i < MemberUserInfo.Instance.TeamMemberList.Count; i++)
            {
                GameObject my_prefab = (GameObject)Resources.Load(str_LoadPath);
                if (MemberUserInfo.Instance.My_UserInfo.UserName != MemberUserInfo.Instance.TeamMemberList[i].UserName)
                {
                    // 实例化预制体并设置父对象
                    my_prefab = Instantiate(my_prefab, parent.transform);

                    my_prefab.name = MemberUserInfo.Instance.TeamMemberList[i].UserName;

                    my_prefab.GetComponent<Member_Personal_UserMember>().my_UserInfo = MemberUserInfo.Instance.TeamMemberList[i];
                }
            }
        }
    }

}

