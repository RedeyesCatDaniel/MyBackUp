using LGUVirtualOffice;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{

    /// <summary>
    /// 点击 “+”（修改状态） 后弹出的面板
    /// </summary>
    public class ChangeStatusPanel : AbstractController, IPointerEnterHandler, IPointerExitHandler
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

        public Button btn_OnLine;    // 在线中按钮
        public Button btn_Working;   // 工作中按钮
        public Button btn_Leave;     // 离开中按钮
        public Button btn_Meeting;   // 会议中按钮

        public Image image_State;   // 状态点
        public Image image_State_BG;   // 状态点__背景

        public TextMeshProUGUI text_State;    // 用户个性签名字数

        public Button btn_ChangeSignature;     // 个性签名按钮
        public GameObject obj_ChangeSignature; // 个性签名界面
        public Button btn_Exit;                // 退出按钮
        public GameObject obj_Exit;            // 退出界面

        public GameObject obj_Green1;
        public GameObject obj_Green2;
        public GameObject obj_Green3;
        public GameObject obj_Green4;

        private IQueueMessageService queueMessageService;

        void Start()
        {
            obj_Green1.SetActive(false); obj_Green2.SetActive(false); obj_Green3.SetActive(false); obj_Green4.SetActive(false);

            queueMessageService = this.GetService<IQueueMessageService>();

            // 点击了个性签名按钮触发事件
            btn_ChangeSignature.onClick.AddListener(() => {
                obj_ChangeSignature.SetActive(true);
            });

            // 点击了退出按钮触发事件
            btn_Exit.onClick.AddListener(() => {
                obj_Exit.SetActive(true);
            });

            // 修改状态
            btn_OnLine.onClick.AddListener(() => {
                State(RequiredStringManager.str1_Online, RequiredStringManager.str4_Online);
                
                StateAddLisTener((int)UserStateEnum.Online);
            });
            btn_Working.onClick.AddListener(() => {
                State(RequiredStringManager.str1_Busy, RequiredStringManager.str4_Busy);
                
                StateAddLisTener((int)UserStateEnum.Busy);
            });
            btn_Leave.onClick.AddListener(() => {
                State(RequiredStringManager.str1_Offline, RequiredStringManager.str4_Offline);
                
                StateAddLisTener((int)UserStateEnum.Offline);
            });
            btn_Meeting.onClick.AddListener(() => {
                State(RequiredStringManager.str1_Meeting, RequiredStringManager.str4_Meeting);
                
                StateAddLisTener((int)UserStateEnum.InMeeting);
            });
        }

        /// <summary>
        /// 上传状态+Push事件消息
        /// </summary>
        /// <param name="state"></param>
        private void StateAddLisTener(int state)
        {
            
            if (MemberUserInfo.Instance.My_UserInfo.UserStatus.Value != state)
            {
                MemberUserInfo.Instance.My_UserInfo.UserStatus.Value = state;
                
                // Push事件消息
                queueMessageService.PushEventMessage(new UserStatusModifiedEvent
                {
                    UserId = MemberUserInfo.Instance.My_UserInfo.UserId,
                    NewState = state
                }, null);
                Debug.Log("修改了状态");
            }

        }


        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !controllerPanel && obj_ChangeSignature.activeSelf == false && obj_Exit.activeSelf == false)
            {
                this.gameObject.SetActive(false);
            }
        }

        private void State(string url1,string url2)
        {
            this.gameObject.SetActive(false);

            image_State.sprite = Resources.Load<Sprite>(url1);
            image_State_BG.sprite= Resources.Load<Sprite>(url2);
            text_State.text = image_State.sprite.name;
        }
    }
   
}
