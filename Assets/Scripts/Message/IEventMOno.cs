using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    public class IEventMOno : AbstractController
    {
        
        // Start is called before the first frame update
        void Start()
        {
            this.SubscribeEvent<IEventTest>((e) =>
            {
                print("IEventTest TRIGGERED!");
                print(e.Name);
            });

            

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                this.GetService<IQueueMessageService>().PushEventMessage(new IEventTest() { Name = "HAHA"}, () => { print("send failed"); });
            }
        }


        
    }
}
