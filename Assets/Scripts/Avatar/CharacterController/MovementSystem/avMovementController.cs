using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avMovementController : MonoBehaviour
    {
        Camera cam;
        public UnityEvent<Vector3> move;
        

        private void Awake()
        {
            cam = Camera.main;
        }

        public Vector3 GetMoveDir() {
            Vector2 inputDir = avPlayerInputManager.GetMoveDir();
            Vector3 finalDir = cam.transform.forward * inputDir.y + cam.transform.right * inputDir.x;
            finalDir.y = 0;
            return finalDir;
        }

        private void Update()
        {
            move.Invoke(GetMoveDir());
        }


    }
}