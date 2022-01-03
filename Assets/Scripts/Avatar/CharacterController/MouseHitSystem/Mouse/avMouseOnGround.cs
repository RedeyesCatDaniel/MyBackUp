using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avMouseOnGround
    {
        public static Camera cam;
        public static Camera GetCam() {
            if (cam == null) {
                cam = Camera.main;
            }
            return cam;
        }

        public static Vector3 GetMouseOnWorldPose() {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = GetCam().ScreenPointToRay(mousePos);
            Vector3 worldPos = ray.direction + GetCam().transform.position;
            return worldPos;
        }

        public static bool RayCastToGround(out RaycastHit rs) {
            Vector3 origin = GetMouseOnWorldPose();
            Vector3 dir = origin - GetCam().transform.position;
            Ray ray = new Ray(origin, dir);

            if (Physics.Raycast(ray, out RaycastHit info))
            {
                rs = info;
                return true; 
            }
            else {
                rs = default;
                return false;
            }
            
        }

        public static bool RayCastToGround(float maxLength,LayerMask mask,out RaycastHit rs)
        {
            Vector3 origin = GetMouseOnWorldPose();
            Vector3 dir = origin - GetCam().transform.position;
            Ray ray = new Ray(origin, dir);

            if (Physics.Raycast(ray, out RaycastHit info, maxLength, mask))
            {
                rs = info;
                return true;
            }
            else
            {
                rs = default;
                return false;
            }

        }

        public Vector3 GetGroundPos() {
            return default;
        }
    }
}