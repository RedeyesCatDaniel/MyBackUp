using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;
using TMPro;


namespace LGUVirtualOffice
{
    public class ResolveMessage : AbstractController
    {
        public TMP_InputField inputField;
        private UserInfo userInfo;


        private void Start()
        {
            userInfo = this.GetModel<UserInfo>();
            this.SubscribeEvent<MessageEvent>((e) => {
                if(e.Reciever == userInfo.UserId)
                {
                    print(e.SendMessage);
                }
            });

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                PushMessage(userInfo.UserId);
            }
        }


        void PushMessage(string Reciever)
        {
            if (inputField.text != "")
            {
                this.GetService<IQueueMessageService>().PushEventMessage(new MessageEvent()
                {
                    SendMessage = "test",
                    Reciever = Reciever,
                    Channel = "asdf"
                    
                },()=> {
                    print("SendFail");
                });
            }
        }


        
    }
}