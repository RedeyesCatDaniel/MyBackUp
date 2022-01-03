using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LGUVirtualOffice.Framework;


namespace LGUVirtualOffice
{
    public class MessageSendTest : AbstractController
    {
        public TMP_InputField InputMessage;
        MessageSendService messageSendService;

        // Start is called before the first frame update
        void Start()
        {
            messageSendService = this.GetService<MessageSendService>();
        }

        // Update is called once per frame
        void Update()
        {
            SendTest();
        }

        void SendTest()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (InputMessage.text != "")
                {
                    messageSendService.PushMessage("asdf", "sdfasef", InputMessage.text);
                }
                //DynamoDBUtil.Instance.GetItemByPrimaryKeyWithinDictionary<object>();
                InputMessage.text = "";
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                messageSendService.CheckIsRead("asdf", "2021-12-14T05:03:43.43").OnCompleted((dic)=> {
                    print(dic["IsRead"]);
                });
            }

        }

    }
}