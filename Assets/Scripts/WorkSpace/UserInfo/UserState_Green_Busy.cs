using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGUVirtualOffice
{
    public class UserState_Green_Busy : AbstractController, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            obj_Green.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            obj_Green.SetActive(false);
        }

        public GameObject obj_Green;
    }
}

