using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    public class PackUpTeamListPanel : AbstractController
    {
        public UserInfo my_UserInfo;
        public Image image_State;

        void Start()
        {           
            ImagePicture(my_UserInfo.UserStatus.Value);
            my_UserInfo.UserStatus.Subscribe((NewState) =>  // 获取新状态
            {
                ImagePicture(NewState);
            });
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
