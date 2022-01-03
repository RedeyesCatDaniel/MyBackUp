using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "AsyncCommand/AsyncConnectPhotonCommand")]
    public class AsyncConnectPhotonCommand : AsyncTool
    {
        public System.Action DOnFinish;
        public override void Act()
        {
            PhotonManager.Instance.ConnectToServer(UserInfo.Instance).OnSuccess((x)=> {
                if (x) {
                    DOnFinish?.Invoke();
                    DOnFinish = null;
                }
            });
        }

        public override void OnFinish(Action action)
        {
            DOnFinish = action;
        }
    }
}