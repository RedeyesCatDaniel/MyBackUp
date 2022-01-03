using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    public class MessageEvent : IEvent
    {
        public string SendMessage;
        public string Channel;
        public string Reciever;
    }
}