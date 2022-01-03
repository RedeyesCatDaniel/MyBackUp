using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice
{
    public class memSetMyIDCommand : AbstractCommand
    {
        public string idValue;

        public memSetMyIDCommand(string memId) {
            idValue = memId;
        }
        protected override void OnExcute()
        {           
            this.GetService<IMemberService>().MyMember.ID = idValue;
        }
    }
}