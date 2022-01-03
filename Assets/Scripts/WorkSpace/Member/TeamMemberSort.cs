using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 挂载到每一个TeamMember上
    /// </summary>
    public class TeamMemberSort : AbstractController
    {
        private VerticalLayoutGroup verticalLayoutGroup;
        private ContentSizeFitter contentSizeFitter;
        public Button btn_PackUpState;
        public Button btn_UnfoldState;
        public GameObject obj_PackUp;
        public GameObject obj_Unfold;

        private int index;

        private int childCount;      //子物体数量
        private float height_PackUp; // 收起状态高度
        private float height_Unfold; // 展开状态高度
        private float offset;        // 子物体间隔
        private float contentHeight; // content的高度

        //public UserInfo my_UserInfo { get; private set; }  // 用于接受TeamCode和TeamName(后期替换方法，此处为了便于功能实现)

        public GameObject TeamUserMemberContent; // 要加载的物体父物体

        public TextMeshProUGUI text_teamName; // 团队名
        public TextMeshProUGUI text_Number;   // 团队数量
        public string text_TeamCode;

        private GetOtherListMember getOtherListMember;

        private bool ctl;
        void Start()
        {
            getOtherListMember = this.GetService<GetOtherListMember>();

            verticalLayoutGroup = this.gameObject.GetComponentInParent<VerticalLayoutGroup>(); // 获取 Content 父物体的垂直布局组件
            contentSizeFitter = this.gameObject.GetComponentInParent<ContentSizeFitter>();

            childCount = this.transform.parent.transform.childCount;             // 子物体的数量
            height_PackUp = obj_PackUp.GetComponent<RectTransform>().rect.height;// 收起状态高度
            height_Unfold = obj_Unfold.GetComponent<RectTransform>().rect.height;// 展开状态高度
            offset = verticalLayoutGroup.spacing - height_PackUp;                // 子物体间隔

            contentHeight = childCount * height_PackUp + height_Unfold + (childCount - 1) * offset;

            btn_PackUpState.onClick.AddListener(() => {
                OffUnfold();
                StartCoroutine(ControllerVerticalLayOut());

                if (!ctl)
                {
                    ctl = true;
                    if (text_TeamCode != MemberUserInfo.Instance.My_UserInfo.TeamInfo.TeamCode)
                        getOtherListMember.GetList(text_TeamCode, OtherList);
                    else
                        MyrList();
                }
                
            });

            btn_UnfoldState.onClick.AddListener(() => {
                verticalLayoutGroup.enabled = true;
                contentSizeFitter.enabled = true;
                obj_PackUp.SetActive(true);
                obj_Unfold.SetActive(false);
            });
        }

        private void OtherList(List<UserInfo> S1)
        {
            Debug.Log(S1.Count);
            GameObject my_prefab = (GameObject)Resources.Load(RequiredStringManager.str_Member_Team_UesrMember);
            for (int i = 0; i < S1.Count; i++)
            {
                // 实例化预制体并设置父对象
                my_prefab = Instantiate(my_prefab, TeamUserMemberContent.transform);
                my_prefab.name = S1[i].TeamInfo.TeamName;
                my_prefab.GetComponent<Member_Team_UserMember>().my_UserInfo = S1[i];
            }
        }
        private void MyrList()
        {
            // 加载预设体资源
            GameObject my_prefab = (GameObject)Resources.Load(RequiredStringManager.str_Member_Team_UesrMember);
            for (int i = 0; i < MemberUserInfo.Instance.TeamMemberList.Count; i++)
            {
                if (MemberUserInfo.Instance.My_UserInfo.UserName != MemberUserInfo.Instance.TeamMemberList[i].UserName)
                {
                    my_prefab = Instantiate(my_prefab, TeamUserMemberContent.transform);
                    my_prefab.name = MemberUserInfo.Instance.TeamMemberList[i].UserName;
                    my_prefab.GetComponent<Member_Team_UserMember>().my_UserInfo = MemberUserInfo.Instance.TeamMemberList[i];

                }
            }
        }
        public void SetUserInfo(UserInfo info)
        {
            //my_UserInfo = new UserInfo();
            TeamModel temeInfo = new TeamModel();
            temeInfo.TeamCode = info.TeamInfo.TeamCode;
            temeInfo.TeamName = info.TeamInfo.TeamName;
            //my_UserInfo.TeamInfo = temeInfo;
        }

        /// <summary>
        /// 脚本手动排序
        /// </summary>
        private void ScriptSort()
        {
            // 间隔=Spacing（Vertical预设的）- height_PackUp高 --- (48.5-36=12.5)
            offset = verticalLayoutGroup.spacing - height_PackUp;

            for (int i = index + 1; i < childCount; i++)
            {
                //float y = height_PackUp - height_Unfold - offset; y高度=收起状态高度*数量+展开状态高度+偏移量
                float y = -height_Unfold - height_PackUp * (i - 1) - offset * i;
                this.transform.parent.GetChild(i).transform.localPosition = new Vector3(0, y, 0);
            }
        }

        /// <summary>
        /// 自动布局排序
        /// </summary>
        private void OffUnfold()
        {
            // 还原设置
            for (int i = 0; i < childCount; i++)
            {
                if (this.transform.parent.GetChild(i).transform.GetChild(1).gameObject.activeSelf == true)
                {
                    this.transform.parent.GetChild(i).transform.GetChild(1).gameObject.SetActive(false);
                    this.transform.parent.GetChild(i).transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }


        private IEnumerator ControllerVerticalLayOut()
        {
            contentSizeFitter.enabled = true;
            yield return verticalLayoutGroup.enabled = true;
            contentSizeFitter.enabled = false;
            this.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contentHeight);
            verticalLayoutGroup.enabled = false;

            obj_Unfold.SetActive(true);
            obj_PackUp.SetActive(false);

            // 获取所在 Content 父物体索引
            index = transform.GetSiblingIndex();
            // 此处应该调用一个方法，执行TeamMember排序方法
            ScriptSort();
        }


    }

}
