using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
   // [RequireComponent(typeof(Animator))]
    public class avAnimatorController : MonoBehaviour
    {
        public UnityEvent OnMove;
        public Animator anim;
       // [HideInInspector]
        public PhotonView view;
        public void SetAnim(Animator anim) {
           

            this.anim = anim;
            anim.transform.parent = transform;
            anim.transform.localPosition = Vector3.zero;

            view = anim.GetComponent<PhotonView>();

            
        }
        public void UpdateSpeed(Vector3 speed)
        {
            //Debug.Log(speed);

            
            anim?.SetFloat("Speed",speed.magnitude);
            if (speed.sqrMagnitude > 0)
            {

                OnMove.Invoke();

            }
        }

        public void UpdateSpeed(float speed) {
            
            anim?.SetFloat("Speed", speed);
            
        }

        public bool TryGetAnimator(out Animator animator) {
            if (anim != null)
            {
                animator = anim;
                return true;
            }
            else {
                animator = null;
                return false;
            }
        }
            
    }
}