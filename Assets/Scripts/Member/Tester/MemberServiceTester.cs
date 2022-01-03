using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice
{
    public class MemberServiceTester : AbstractController
    {

        public Member m;
        public string gender;

        private void Awake()
        {
            Login();
        }

        [ContextMenu(itemName:"Login")]
        public void Login() {
            UserToolkit.DefaultLogin();
            m = new Member(UserInfo.Instance.UserId);
        }

        [ContextMenu(itemName: "Push")]
        public void Push() {
            memService.PushData<string>(m.id,"Gender", gender, (x)=> { if (x) { Debug.Log("Successfully writen"); } });
        }

        [ContextMenu(itemName: "Pull")]
        public void Pull() {
            memService.PullData<string>(m.id, "Gender",(x)=> { gender = x; });
        }
    }
}
