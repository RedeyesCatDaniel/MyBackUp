using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class UIRotationManager : MonoBehaviour
    {
        public float rotationSpeed;
        public RectTransform rect;
        public void Rotate(float degree) {
            rect.Rotate(transform.forward,degree);
        }

        public void Rotate()
        {
            Rotate(rotationSpeed);
        }

        public void RotateInDeltaTime()
        {
            Rotate(rotationSpeed*Time.deltaTime);
        }
    }
}