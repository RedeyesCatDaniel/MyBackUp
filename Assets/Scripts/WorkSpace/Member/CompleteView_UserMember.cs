using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 完整视图中的 UserName
    /// </summary>
    public class CompleteView_UserMember : AbstractController
    {
        public UserInfo my_UserInfo;

        public Button btn_CompleteView_UserMember; // 完整视图中每个用户的---自身---按钮

        public TextMeshProUGUI text_UserName;      // 完整视图中每个用户的---用户名
        public TextMeshProUGUI text_TeamName;      // 完整视图中每个用户的---团队名
        public TextMeshProUGUI text_Siganature;    // 完整视图中每个用户的---签名
        public Image image_TeamNameParent;    // 完整视图中每个用户的---用户名的父物体Image

        public Image image_State;

        void Start()
        {
            Init(my_UserInfo);
            ImageLength();
            btn_CompleteView_UserMember.onClick.AddListener(CreatePersonalInformation);  // 点击按钮---出现用户个人信息面板

            my_UserInfo.Signature.Subscribe((newSigenature) =>  // 获取新签名
            {
                text_Siganature.text = newSigenature;
                ImageLength();
            });

            ImagePicture(my_UserInfo.UserStatus.Value);
            my_UserInfo.UserStatus.Subscribe((NewState) =>  // 获取新状态
            {
                ImagePicture(NewState);
            });

            // 为了修复第一次打开完整视图不同列表，团队名和用户名重叠，没有等距问题
            image_TeamNameParent.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;

        }

      
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="userInfo"></param>
        private void Init(UserInfo userInfo)
        {
            text_UserName.text = my_UserInfo.UserName;
            text_TeamName.text = my_UserInfo.TeamInfo.TeamName;
            text_Siganature.text = my_UserInfo.Signature.Value;           
        }


        /// <summary>
        /// // 点击按钮---出现用户个人信息面板
        /// </summary>
        private void CreatePersonalInformation()
        {
            #region
            //float x = GetComponent<RectTransform>().position.x;
            //float y = GetComponent<RectTransform>().position.y;
            //if (LeftStatusBarEachButton.Instance.PersonalInformationPanel.activeSelf == false)
            //    LeftStatusBarEachButton.Instance.PersonalInformationPanel.SetActive(true);
            // 暂时无法设定所在位置，临时位置---本团队
            //PersonalInformationPanel.Instance.text_Position.text = my_UserInfo.TeamInfo.TeamName;
            // 设置加载的坐标
            //LeftStatusBarEachButton.Instance.PersonalInformationPanel.GetComponent<RectTransform>().localPosition = new Vector3(PanelPosition.Instance.ControllerCompleteView_UserMemberPersonalInformationPosition(x, y)[0],
            //PanelPosition.Instance.ControllerPersonalInformationPosition(x, y)[1], 0);
            #endregion
            PersonalInformationPanel.Instance.CreatePersonalInformationPanelPosition(this.gameObject);
            PersonalInformationPanel.Instance.UpdatePersonalInformation(my_UserInfo);
            PersonalInformationPanel.Instance.my_UserInfo = my_UserInfo;
        }

        private void ImageLength()
        {
            image_TeamNameParent.GetComponent<RectTransform>().sizeDelta = new Vector2(text_TeamName.text.Length*5+10,15.5f);
        }


        private void ImagePicture(int status)
        {
            if (status == (int)UserStateEnum.Online)
            {
                image_State.sprite = Resources.Load<Sprite>(RequiredStringManager.str2_Online);
            }
            else if (status == (int)UserStateEnum.Offline)
            {
                image_State.sprite = Resources.Load<Sprite>(RequiredStringManager.str2_Offline);
            }
            else if (status == (int)UserStateEnum.InMeeting)
            {
                image_State.sprite = Resources.Load<Sprite>(RequiredStringManager.str2_Meeting);
            }
            else if (status == (int)UserStateEnum.Busy)
            {
                image_State.sprite = Resources.Load<Sprite>(RequiredStringManager.str2_Busy);
            }
        }
    }

}
