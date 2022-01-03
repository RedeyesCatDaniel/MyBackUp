using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avAWSConnector : MonoBehaviour
    {
        public UnityEvent OnConnectedToAWS;

        public void ConnectToAWS() {
            logLoginManager.Instance.Login(UserToolkit.GetDefaultInfo()).OnSuccess((x)=> {
                OnConnectedToAWS.Invoke();
            });
        }
    }
}