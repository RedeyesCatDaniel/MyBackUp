using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace LGUVirtualOffice
{
    public class avNodeOnCirlceUI : MonoBehaviour
    {
        public CircularUI circle;
       
        [HideInInspector]
        public float currentDegree;
        public float CurrentDegree { get => currentDegree; set => currentDegree = value; }
 

        public UIImageManager uimgman;
        public UIImageManager Uimgman { get => uimgman;}
        

        private RectTransform rect;
        public float transitionTime = 0.1f;

        private Tweener tween;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }
        [ContextMenu(itemName: "relocate")]
        public void Relocate() {
            Vector2 pos = circle.GetPositionByDegree(currentDegree);
            rect.localPosition = pos;
        }

        public void Relocate(float degree)
        {
            if (tween!=null) {
                tween.Kill();
            }
            if (transitionTime > 0)
            {
                ResetPos();
                tween = DOTween.To(() => currentDegree, x => currentDegree = x, degree, transitionTime);
            }
            else {
                RelocateOnce(degree);
            }
            
            //currentDegree = degree;
            //Relocate();
        }
        private void ResetPos()
        {
            RelocateOnce(90);
        }

        public void RelocateOnce(float degree) {
            currentDegree = degree;
            Relocate();
        }









    }
}