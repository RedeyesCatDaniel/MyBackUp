using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// Scrollbar动态显示
/// </summary>
namespace LGUVirtualOffice
{
    public class ScrollbarChange : AbstractController, IPointerEnterHandler
    {
        public Image Changething;
        public Scrollbar scrollbar;
        float time;
        public float wait;
        void Start()
        {
           scrollbar.onValueChanged.AddListener(OnSetScrollbar);
        }
        
        private void SetScrollbar()
        {

            Color color = Changething.color;
            color.a = 1;
            Changething.color = color;
        }

        private IEnumerator SetScrollbaryeld()
        {

            yield return new WaitForSeconds(wait);
            if (Time.time > time)
            {
                Color color = Changething.color;
                color.a = 0;
                Changething.color = color;
            }

        }
        public void OnSetScrollbar(float f)
        {
           
                SetScrollbar();
                StartCoroutine("SetScrollbaryeld");
                time = Time.time + wait;
           
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnSetScrollbar(0);
        }
    }
}