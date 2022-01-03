using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LGUVirtualOffice
{
    public class UIOutlineManager : MonoBehaviour
    {
        public Outline outline;
       // public Color changeTo;
        public float changeTime;
        //private Color defaultColor;

        //private void Awake()
        //{
        //    defaultColor = outline.effectColor;
        //}


        public void ResetColor()
        {
            outline.DOFade(0, changeTime);
        }

        public void ChangeColor()
        {
            outline.DOFade(1, changeTime);
        }

    }
}