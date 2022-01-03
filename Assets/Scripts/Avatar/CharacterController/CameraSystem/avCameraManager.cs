using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
namespace Bear
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class avCameraManager : MonoBehaviour
    {
        //public float ZoomAmount = 10f;

        public float max;
        public float min;
        //private float MoveAmount;
        private CinemachineVirtualCamera cam;
        private void Awake()
        {
            cam = GetComponent<CinemachineVirtualCamera>();
           
        }

        public void Zoom() {
            Zoom(Input.mouseScrollDelta.y);
        }

   

        public void Zoom(float amount)
        {
           
            cam.m_Lens.FieldOfView -= amount;
            cam.m_Lens.FieldOfView = Mathf.Clamp(cam.m_Lens.FieldOfView, min, max);
        }

        private void Update()
        {
            Zoom();
        }
    }
}