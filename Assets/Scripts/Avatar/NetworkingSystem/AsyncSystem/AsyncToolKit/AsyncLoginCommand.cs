using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGUVirtualOffice.Framework;
using System;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "AsyncCommand/AsyncLoginCommand")]
    public class AsyncLoginCommand : AsyncTool
    {
        private System.Action DOnFinish;
        public override void Act()
        {
            logLoginManager.Instance.Login(UserInfo.Instance).OnSuccess((info)=> { 
                DOnFinish?.Invoke();
                DOnFinish = null;
            });
            
        }

        public override void OnFinish(Action action)
        {
            DOnFinish = action;
        }

    }
}