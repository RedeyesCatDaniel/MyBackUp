using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LGUVirtualOffice
{
    public class UISizeManager : MonoBehaviour
    {
        public RectTransform rect;
        public float popUpTime;
        public void PopUp() {
            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one,popUpTime);
        }

        private void OnEnable()
        {
            PopUp();
        }
    }
}