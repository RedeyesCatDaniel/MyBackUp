using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bear
{
    public class avRotateWithMouse : MonoBehaviour
    {
        private Vector2 pos;
        public float speed = 0.1f;
        public UnityEvent<Vector3> Rotate;
        //public Vector2 delta;
        private void Awake()
        {
            pos = Input.mousePosition;
        }
        public Vector2 GetMouseDelta() {
            Vector2 curr = Input.mousePosition;
            Vector2 delta = curr - pos;
            pos = Input.mousePosition;
            return delta;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) {
                pos = Input.mousePosition;
            }


            if (Input.GetMouseButton(0)) {
                float y = GetMouseDelta().x;
                Rotate.Invoke(new Vector3(0, -y * speed, 0));
                //transform.Rotate();
            }
        }


    }
}