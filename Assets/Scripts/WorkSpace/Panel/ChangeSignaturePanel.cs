using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    /// <summary>
    /// 点击修改个性签名后弹出的面板
    /// </summary>
    public class ChangeSignaturePanel : AbstractController, IPointerEnterHandler, IPointerExitHandler
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


        public Button btn_Yes;
        public Button btn_No;
        public TextMeshProUGUI text_Signature; // 用户个性签名
        public TextMeshProUGUI text_Number;    // 用户个性签名字数
        public TMP_InputField input_Signature; // 用户个性输入

        private IQueueMessageService queueMessageService;
        void Start()
        {
            input_Signature.text = text_Signature.text; // 将用户个性签名给到输入框
            queueMessageService = this.GetService<IQueueMessageService>();
            int length = input_Signature.text.Length;  //字符串长度
            text_Number.text = length + "/80";

            btn_Yes.onClick.AddListener(() => {
                if (input_Signature.text != text_Signature.text)
                {
                    DynamoDBUpdateModel model = new DynamoDBUpdateModel()
                    {
                        TableName = DynamoDBTableConst.TABLE_USER_MEMBER_INFO,
                        PartitionKey = new DynamoDBKeyModel() { Name = "TeamCode", Value = MemberUserInfo.Instance.My_UserInfo.TeamInfo.TeamCode },
                        SortKey = new DynamoDBKeyModel() { Name = "UserId", Value = MemberUserInfo.Instance.My_UserInfo.UserId }
                    };
                    model.items = new Dictionary<string, object>();
                    model.items.Add("Signature", input_Signature.text);
                    DynamoDBUtil.Instance.UpdateItemByPrimarykey(model);
                    
                    queueMessageService.PushEventMessage(new UserSignatureModifiedEvent { 
                        UserId=MemberUserInfo.Instance.My_UserInfo.UserId,
                        NewSignature= input_Signature.text
                    },null);
                }

                this.gameObject.SetActive(false);
                text_Signature.text = input_Signature.text; // 将输入框给到用户个性签名
            });
            btn_No.onClick.AddListener(() => {
                input_Signature.text = text_Signature.text; // 还原设置
            });


            input_Signature.onValueChanged.AddListener(delegate {
                int length = input_Signature.text.Length;
                text_Number.text = length + "/80";
            });
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !controllerPanel)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

}

