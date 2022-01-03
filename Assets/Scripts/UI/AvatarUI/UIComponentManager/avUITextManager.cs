using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avUITextManager : MonoBehaviour
    {
        public UnityEvent<string> DSetString;

        private void Start()
        {
           // GlobalModelUIManager.instance.DOnGroupSelected.RemoveListener(SetString);
           // GlobalModelUIManager.instance.DOnGroupSelected.AddListener(SetString);
        }

        public void SetString(int index) {
            //string info = GlobalModelUIManager.instance.GetCurrentTitle(index);
            //DSetString.Invoke(info);
        }
    }
}