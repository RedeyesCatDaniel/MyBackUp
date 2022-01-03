using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avHeartBeat : MonoBehaviour
    {
        public UnityEvent CustomEvent;
        public UnityEvent OnAwake;
        public UnityEvent OnUpdate;
        

        private void Awake()
        {
            OnAwake.Invoke();
        }

        private void Update()
        {
            OnUpdate.Invoke();
        }

        [ContextMenu(itemName: "Trigger")]
        public void TriggerCustomEvent() {
            CustomEvent.Invoke();
        }
    }
}