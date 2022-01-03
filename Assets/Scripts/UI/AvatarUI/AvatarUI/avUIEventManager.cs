using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace LGUVirtualOffice
{
    public class avUIEventManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public UnityEvent OnEnter;
        public UnityEvent OnClick;
        public UnityEvent OnExit;
        

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExit.Invoke();
        }
    }
}