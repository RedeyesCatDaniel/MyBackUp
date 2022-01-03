using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 鼠标移动到展开状态下九个头像上，显示人物信息
    /// </summary>
    public class MemberAndHeadPorteaitPanel : AbstractController, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject memberHeadPortraitPanel;  // 面板弹窗

        public TextMeshProUGUI Text_UserName1;  // 头像下的用户名

        public TextMeshProUGUI Text_UserName2; // 弹窗下的用户名
        public TextMeshProUGUI Text_TeamName;  // 弹窗下的团队名

        public UserInfo my_UserInfo;

        public Image image_State;
        public void OnPointerEnter(PointerEventData eventData)
        {
            memberHeadPortraitPanel.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            memberHeadPortraitPanel.SetActive(false);
        }


        void Start()
        {
            Text_UserName1.text = this.gameObject.name;
            Text_UserName2.text = this.gameObject.name;
            Text_TeamName.text = my_UserInfo.TeamInfo.TeamName;

            ImagePicture(my_UserInfo.UserStatus.Value);


            my_UserInfo.UserStatus.Subscribe((NewState) =>  // 获取新状态
            {
                ImagePicture(NewState);
            });
        }

        private void ImagePicture(int status)
        {
            if(status == (int)UserStateEnum.Online)
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

