using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class AsyncClicker
    {
        public int Max { get; set; }
        public System.Action DONComplete ;
        public AsyncClicker(int max, System.Action OnComplete) {
            Max = max;
            DONComplete = OnComplete;
            if (max == 0) {
                DONComplete.Invoke();
            }   
        }

        public void Click() {
            Max -= 1;
            if (Max <= 0) {
                DONComplete?.Invoke();
            }
        }
    }
}