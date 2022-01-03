using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    public class UIImageManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public Image image;
        public Color[] PresetColor;
        public float colorChangeTime;
        public bool colorLock;
        public void ChangeMaterialTo(Material material)
        {
            image.material = material;
        }

        public void ChangeColorTo(int index)
        {
            if (!colorLock)
                image.DOColor(PresetColor[index], colorChangeTime);
        }

        public void ChangeColorTo(Color color)
        {
            image.color = color;
        }

        public void SetColorLock(bool clock)
        {
            colorLock = clock;
        }

        public void ChangeImageTo(Sprite sprite)
        {
            image.sprite = sprite;
        }





    }
}