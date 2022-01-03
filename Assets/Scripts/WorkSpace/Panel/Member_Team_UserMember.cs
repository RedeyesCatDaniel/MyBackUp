using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LGUVirtualOffice;
using TMPro;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 挂载到每个UsertName上，Member界面的UserName，每个UserName在屏幕上的坐标
    /// </summary>
    public class Member_Team_UserMember : AbstractController
    {
        public UserInfo my_UserInfo;

        public Button btn_UserName;
        public TextMeshProUGUI text_UserName;
        public TextMeshProUGUI text_Sigenature;
        public Image image_State;

        private UnityEvent e;
        void Start()
        {
            Init(); 

            btn_UserName.onClick.AddListener(CreatePersonalInformation);

            my_UserInfo.Signature.Subscribe((newSigenature) =>
            {
                text_Sigenature.text = newSigenature;
            });

            ImagePicture(my_UserInfo.UserStatus.Value);
            my_UserInfo.UserStatus.Subscribe((NewState) =>  // 获取新状态
            {
                ImagePicture(NewState);
            });
        }

        /// <summary>
        /// 初始化操作
        /// 对姓名、个性签名初始化
        /// </summary>
        private void Init()
        {
            text_UserName.text = my_UserInfo.UserName;
            if (my_UserInfo.Signature.Value == null)
                text_Sigenature.text = "";
            else
                text_Sigenature.text = my_UserInfo.Signature.Value;
        }

        /// <summary>
        /// 点击用户，出现个人信息面板并更新面板信息
        /// </summary>
        private void CreatePersonalInformation()
        {
            #region
            //float x = GetComponent<RectTransform>().position.x;
            //float y = GetComponent<RectTransform>().position.y;
            //if (LeftStatusBarEachButton.Instance.PersonalInformationPanel.activeSelf == false)
            //    LeftStatusBarEachButton.Instance.PersonalInformationPanel.SetActive(true);

            //// 暂时无法设定所在位置，临时位置---本团队
            //PersonalInformationPanel.Instance.text_Position.text = my_userInfo.TeamInfo.TeamName;

            //LeftStatusBarEachButton.Instance.PersonalInformationPanel.GetComponent<RectTransform>().localPosition = new Vector3(PanelPosition.Instance.ControllerCompleteView_UserMemberPersonalInformationPosition(x, y)[0],
            //                                                                                                        PanelPosition.Instance.ControllerCompleteView_UserMemberPersonalInformationPosition(x, y)[1], 0);

            #endregion
            PersonalInformationPanel.Instance.CreatePersonalInformationPanelPosition(this.gameObject);
            PersonalInformationPanel.Instance.UpdatePersonalInformation(my_UserInfo);
            PersonalInformationPanel.Instance.my_UserInfo = my_UserInfo;
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


