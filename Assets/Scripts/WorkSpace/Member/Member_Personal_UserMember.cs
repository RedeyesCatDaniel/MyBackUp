using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LGUVirtualOffice
{
    /// <summary>
    /// Member 面板中 个人中的 UesrMember
    /// </summary>
    public class Member_Personal_UserMember : AbstractController
    {
        private Button btn_UserName;
        public UserInfo my_UserInfo;

        public TextMeshProUGUI text_UserName;
        public TextMeshProUGUI text_TeamName;
        public TextMeshProUGUI text_Siganature;
        public Image image_sig_Parent;
        public Image image_State;
        public static Member_Personal_UserMember Instance;
        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            text_UserName.text = my_UserInfo.UserName;
            text_TeamName.text = my_UserInfo.TeamInfo.TeamName;
            ImageLength(); // Image 根据字符串长度变化


            btn_UserName = GetComponent<Button>();
            btn_UserName.onClick.AddListener(CreatePersonalInformation);

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
        }

        private void CreatePersonalInformation()
        {
            #region
            //float x = GetComponent<RectTransform>().position.x;
            //float y = GetComponent<RectTransform>().position.y;
            //if (LeftStatusBarEachButton.Instance.PersonalInformationPanel.activeSelf == false)
            //    LeftStatusBarEachButton.Instance.PersonalInformationPanel.SetActive(true);
            // 暂时无法设定所在位置，临时位置---本团队
            //PersonalInformationPanel.Instance.text_Position.text = my_UserInfo.TeamInfo.TeamName;
            // 设置坐标       
            //LeftStatusBarEachButton.Instance.PersonalInformationPanel.GetComponent<RectTransform>().localPosition = new Vector3(PanelPosition.Instance.ControllerPersonalInformationPosition(x, y)[0],
            //PanelPosition.Instance.ControllerPersonalInformationPosition(x, y)[1], 0);
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

        private void ImageLength()
        {
            if (my_UserInfo.Signature.Value == null)
                text_Siganature.gameObject.transform.parent.gameObject.SetActive(false);
            else
            {
                text_Siganature.text = my_UserInfo.Signature.Value;
                float imageLength;
                if (text_Siganature.text.Length <= 15)
                    imageLength = 16 + text_Siganature.text.Length * 9;
                else
                    imageLength = 16 + 15 * 9;
                image_sig_Parent.GetComponent<RectTransform>().sizeDelta = new Vector2(imageLength, 29);
            }
        }
    }

}

