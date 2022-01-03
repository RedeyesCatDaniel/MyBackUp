using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;

namespace LGUVirtualOffice
{
    public class memIDController : AbstractController
    {
        public void SetId(string idValue) 
        {
            this.SendCommand<memSetMyIDCommand>(new memSetMyIDCommand(""));   
        }
    }
}