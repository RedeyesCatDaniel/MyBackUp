using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "AsyncCommand/AsyncAvatarCommand")]
    public class AsyncAvatarCommand : AsyncTool
    {
        public System.Action DOnFinish;
        public override void Act()
        {
            avGlobalModifierManager.GlobalPull(()=> {
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