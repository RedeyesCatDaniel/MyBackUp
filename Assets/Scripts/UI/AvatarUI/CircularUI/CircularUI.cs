using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class CircularUI : MonoBehaviour
    {

       // private RectTransform trans;
        public float Radius { 
            get => rect.rect.height/2* multiplier;
        }

        public float multiplier;
        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }
        public Vector2 GetPositionByRadians(float radian) {
            Vector2 pos = new Vector2();
            pos.x = Mathf.Cos(radian) * Radius;
            pos.y = Mathf.Sin(radian) * Radius;
            return pos;
        }

        public Vector2 GetPositionByDegree(float degree) {
            Vector2 pos = new Vector2();
            pos.x = Mathf.Cos(Mathf.Deg2Rad*degree) * Radius;
            pos.y = Mathf.Sin(Mathf.Deg2Rad*degree) * Radius;
            return pos;
        }

       
    }
}