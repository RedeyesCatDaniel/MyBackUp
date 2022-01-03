using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace LGUVirtualOffice
{
    public class UIShadowManager : MonoBehaviour
    {
        public Shadow myShadow;
        public Vector3[] PresetPosition;
        public Color[] PresetColor;

        private Color defaultColor;
        private Vector2 defaultPosition;

        private Tween tweener;

        private void Awake()
        {
            defaultColor = myShadow.effectColor;
            defaultPosition = myShadow.effectDistance;
        }

        public void ResetToDefault()
        {
            myShadow.effectColor = defaultColor;
            myShadow.effectDistance = defaultPosition;
        }

        public void ChangeToColor(int index) {
            myShadow.effectColor = PresetColor[index];
        }

        public void ChangeToPose(int index) {
            if (tweener != null)
            {
                tweener.Kill();
            }
            Vector3 prepos = PresetPosition[index];
            Vector2 pos = prepos;
            float time = prepos.z;

            if (time == 0)
            {
                myShadow.effectDistance = pos;
            }
            else {
                tweener = DOTween.To(() => myShadow.effectDistance, x => myShadow.effectDistance = x, pos, time);
            }

            

            
            //myShadow.effectDistance;
        }

        
    }
}