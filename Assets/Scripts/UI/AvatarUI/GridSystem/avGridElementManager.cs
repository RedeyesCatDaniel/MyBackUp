using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avGridElementManager : MonoBehaviour
    {
        public int eleIndex;
        public avUIEventManager value;
        public UnityEvent<int> OnTurnOff;
        public UnityEvent<int> OnTurnOn;
        public UnityEvent<int> OnClickEvent;
        public void TurnOff() {
            OnTurnOff.Invoke(eleIndex);
        }

        public void TurnOn() {
            OnTurnOn.Invoke(eleIndex);
        }

        public void OnClick() {
            OnClickEvent.Invoke(eleIndex);
        }

        
    }
}