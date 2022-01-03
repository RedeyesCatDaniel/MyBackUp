using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avAvatarBodySwitcher : MonoBehaviour
    {
        public avAvatarRenderer[] bodies;
        [HideInInspector]
        public avAvatarRenderer body;
        public UnityEvent AfterMake;

        public void Make(int index) {
            avAvatarRenderer myBody = bodies[index];
            body = Instantiate<avAvatarRenderer>(myBody, transform);
            AfterMake.Invoke();
        }

        public void Clear() {
            if (body!=null) {
                Debug.Log("I am asked to destory body");
                Destroy(body.gameObject);
            }
        }

        public void Switch(int index) {
            Clear();
            Make(index);
        }
                
    }
}