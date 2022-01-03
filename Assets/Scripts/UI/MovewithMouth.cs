using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGUVirtualOffice
{
    public class MovewithMouth : AbstractController, IDragHandler,IBeginDragHandler
    {
       
        Vector3 point;
        Vector2 maxpos;
        public Transform moveObj;
        void Start()
        {
            maxpos = new Vector2(Screen.width, Screen.height);
        }
        public void Move(Vector3 pos) 
        {
            pos.x = Mathf.Clamp(pos.x, 0, maxpos.x);
            pos.y = Mathf.Clamp(pos.y, 0, maxpos.y);
            transform.position =pos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 input = Input.mousePosition;
            input.x = Mathf.Clamp(input.x, 0, maxpos.x);
            input.y = Mathf.Clamp(input.y, 0, maxpos.y);
            moveObj.position = point + input;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            point = moveObj.position - Input.mousePosition;
        }
    }
}