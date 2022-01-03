using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{

    //This class us used mainly for visual effect like connect face icon to the right

    public class avUIAvatarFeatureGroupElementManager : MonoBehaviour
    {
        public int index;

        public UIImageManager imageMan;

        public UnityEvent<int> OnSelected;
        public UnityEvent OnConnectToRight;
        public UnityEvent OnShrink;
        public UnityEvent OnExtendDown;
        public void Select()
        {
            OnSelected.Invoke(index);
        }

        public void ConnectToRight() {
           // Debug.Log("I connected to right");
            OnConnectToRight.Invoke();
        }

        public void Shrink() {
            OnShrink.Invoke();
        }

        public void ExtendDown() {
            OnExtendDown.Invoke();
        }
    }
}