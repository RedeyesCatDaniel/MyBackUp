using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public interface IProgress { 
        public float Progress { get; set; }
    }
    public class LoadingSceneManager : MonoBehaviour, IProgress
    {
        private float progress = 0;
        public UnityEvent<float> DOnValueChange;
        public UnityEvent DOnFinish;

        public float displayDelay = 0;
        public float Progress { 
            get => progress;
            set {
                progress = Mathf.Clamp01(value);
                OnValueChange();
                //throw new System.NotImplementedException(); 
            }
        }

        private void OnValueChange() {
            DOnValueChange.Invoke(progress);
            if (progress>=1) {
                
                OnFinish();
            }
        }

        public void OnFinish() {
            DOnValueChange.Invoke(1);
            if (displayDelay <= 0) {
                Finish();
            }else
            {
                Invoke("Finish", displayDelay);
            }
            
         
        }

        public void Finish() {
            DOnFinish.Invoke();
        }
    }
}