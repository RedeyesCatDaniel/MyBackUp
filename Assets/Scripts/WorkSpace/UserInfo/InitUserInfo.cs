using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 进入场景初始化
    /// 姓名，团队名，个性签名，状态，团队成员加载
    /// </summary>
    public class InitUserInfo : AbstractController
    {
        public TextMeshProUGUI text_UserName;
        public TextMeshProUGUI text_TeamName;
        public TextMeshProUGUI text_Signature;

        public GameObject obj_teamListParent_Unfold; // 展开状态teamListParent
        public GameObject obj_teamListParent_PackUp; // 展开状态teamListParent
        public GameObject obj_GuestListParent_PackUp; // 展开状态teamListParent---Guest
        
        private My_GuestList my_GuestList;

        

        public static InitUserInfo Instance;
        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            my_GuestList = this.GetService<My_GuestList>();
            my_GuestList.GetList();
            
            
            // UserName  初始化
            // TeamName  初始化
            // Signature 初始化
            text_UserName.text = MemberUserInfo.Instance.My_UserInfo.UserName;
            text_TeamName.text = MemberUserInfo.Instance.My_UserInfo.TeamInfo.TeamName;
            if (MemberUserInfo.Instance.My_UserInfo.Signature.Value == null)
                text_Signature.text = "Please enter your signature";
            else
                text_Signature.text = MemberUserInfo.Instance.My_UserInfo.Signature.Value;

            // 本团队预制体---展开状态
            // 本团队预制体---收起状态
            // Guest预制体---展开状态
            MemberListPrefab(MemberUserInfo.Instance.TeamMemberList, obj_teamListParent_Unfold, 9, "Prefabs/UI/Member/Unfold_Member");
            MemberListPrefab_PackUp(MemberUserInfo.Instance.TeamMemberList,obj_teamListParent_PackUp, 8, "Prefabs/UI/Member/PackUpStatus_Member");
            MemberListPrefab(MemberUserInfo.Instance.TeamGuestList,obj_GuestListParent_PackUp, 9, "Prefabs/UI/Member/Unfold_Member");
        }
        #region
        ///// <summary>
        ///// 实例化预制体
        ///// </summary>
        ///// <param name = "list" > 要加载那个list列表 </ param >
        ///// < param name="obj"> 父物体</param>
        ///// <param name = "number" > 多少个 </ param >
        ///// < param name="url"> 预制体路径</param>
        ////public void Instantiate_MemberList_Prefabs(List<UserInfo> list, GameObject parent, int number, string url)
        ////{
        ////    int count = 0; // 设置临时计数器

        ////    for (int i = 0; i < list.Count; i++)
        ////    {
        ////        if (text_UserName.text != list[i].UserName)
        ////        {
        ////            count++;
        ////            加载预设体资源
        ////           GameObject my_prefab = Resources.Load<GameObject>(url);

        ////            实例化预制体并设置父对象
        ////           my_prefab = Instantiate(my_prefab, parent.transform);

        ////            my_prefab.name = list[i].UserName;

        ////            if (count > number)
        ////            {
        ////                my_prefab.SetActive(false);
        ////            }
        ////        }
        ////    }
        ////}
        #endregion

        public void MemberListPrefab(List<UserInfo> list, GameObject parent, int number, string url)
        {
            int count = 0; // 设置临时计数器
                           // 加载预设体资源
            GameObject my_prefab = Resources.Load<GameObject>(url);
            for (int i = 0; i < list.Count; i++)
            {             
                if (text_UserName.text != list[i].UserName)
                {
                    count++;                 
                    // 实例化预制体并设置父对象
                    my_prefab = Instantiate(my_prefab, parent.transform);

                    my_prefab.name = list[i].UserName;
                    my_prefab.GetComponent<MemberAndHeadPorteaitPanel>().my_UserInfo = list[i];

                    if (count > number)
                    {
                        my_prefab.SetActive(false);
                    }
                }
            }
        }

        // 收起状态 8 个同事
        public void MemberListPrefab_PackUp(List<UserInfo> list, GameObject parent, int number, string url)
        {
            int count = 0; // 设置临时计数器
            // 加载预设体资源
            GameObject my_prefab = Resources.Load<GameObject>(url);
            for (int i = 0; i < list.Count; i++)
            {               
                if (text_UserName.text != list[i].UserName)
                {
                    count++;

                    // 实例化预制体并设置父对象
                    my_prefab = Instantiate(my_prefab, parent.transform);

                    my_prefab.name = list[i].UserName;
                    my_prefab.GetComponent<PackUpTeamListPanel>().my_UserInfo = list[i];

                    if (count > number)
                    {
                        my_prefab.SetActive(false);
                    }
                }
            }
        }


        public void Des(GameObject parent)
        {
            if(parent.transform.childCount>0)
            {
                for (int i = 0; i < parent.transform.childCount; i++)
                {
                    Destroy(parent.transform.GetChild(i).gameObject);
                }
            }
            
        }

    }

}
