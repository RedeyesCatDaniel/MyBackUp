using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avAvatarPlaceHolderManager : MonoBehaviour
    {
        private Camera cam;
        public avAvatarRenderer renderer;
        public Transform[] Targets;

        private void Awake()
        {
            cam = Camera.main;   
        }

        [ContextMenu(itemName:"FaceToCam")]
        public void FaceToCam() {
            FaceToTarget(cam.transform);
        }

        public void FaceToTarget(Transform target) {
            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0;
            renderer.transform.forward = dir;
        }

        public void FaceToTarget(int index) {
            if (index>=0&& index< Targets.Length) {
                FaceToTarget(Targets[index]);
            }
        }

        public void Rotate(Vector3 dir) {
            renderer.transform.Rotate(dir);
        }

        public void FaceForward() {
            renderer.transform.forward = transform.forward;
        }


    }
}