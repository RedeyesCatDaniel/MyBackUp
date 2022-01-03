using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice{
    public class avSimpleLogin : MonoBehaviourPunCallbacks
    {
		public UnityEvent OnLogin;
		public bool LoginOnAwake;
		public void Awake()
		{
			if (LoginOnAwake) {
				Login();
			}
		}

		public void Login() {
			UserToolkit.DefaultLogin(()=> { OnLogin.Invoke(); },()=> { });
		}
	}
}