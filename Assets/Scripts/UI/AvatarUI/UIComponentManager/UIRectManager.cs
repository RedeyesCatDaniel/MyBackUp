using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace LGUVirtualOffice
{
    public class UIRectManager : MonoBehaviour
    {
        public RectTransform myRect;
        public float showTime;
        public float hideTime;

        private Tween show;
        private Tween hide;

        //[ContextMenu(itemName: "Rescale")]
        public void Show() {
            Clear();

            if (hideTime <= 0) {
                myRect.localScale = Vector3.one;
            }
            else {
                //Tween tween = DOTween.To(()=> myRect.size,x=> myRect.size = x,Vector2.one,0.1f).OnU
                myRect.DOScale(Vector3.one, showTime).SetEase(Ease.Linear);
            }
            
        }

        public void Hide()
        {

            myRect.localScale = Vector3.zero;
            //Clear();

            //if (showTime <= 0)
            //{
            //    myRect.localScale = Vector3.zero;
            //}
            //else
            //{
            //    myRect.DOScale(Vector3.zero, hideTime).SetEase(Ease.Linear);
            //}
        }

        public void Clear() {
            if (show != null)
            {
                show.Kill();
            }

            if (hide != null) {
                hide.Kill();
            }
        }
    }
}