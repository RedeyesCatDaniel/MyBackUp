using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avPlayerStateManager : MonoBehaviour
    {
        public static avPlayerStateManager instance;
        public avStateManager stateManager;

        private void Awake()
        {
            instance = this;     
        }

        public static bool TryGetAvPlayerStateManager(out avPlayerStateManager instance) {
            instance = avPlayerStateManager.instance;
            if (instance != null) {
                return true;
            }
            return false;
        }


    }
}