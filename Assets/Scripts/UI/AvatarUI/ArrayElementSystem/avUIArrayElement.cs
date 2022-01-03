using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avUIArrayElement : MonoBehaviour
    {
        public int index;
        public int Index { get => index; set => index = value; }

        public UnityEvent<int> OnNotify;

        public void Notify()
        {
            OnNotify.Invoke(index);
        }

        
    }
}