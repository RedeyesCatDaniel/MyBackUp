using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    //[RequireComponent(typeof(CharacterController))]
    public class avMovementManager : MonoBehaviour
    {
        public CharacterController cc;
        private bool IsOnGround;
        public bool isUpdating;
        public UnityEvent<Vector3> OnNotify;
        public bool IsUpdating { get => isUpdating; set => isUpdating = value; }

        [Min(0)]
        public float gravity = 9.8f;
        public float moveSpeed = 4f;

       // [HideInInspector]
        public Vector3 moveDir;
       // [HideInInspector]
        public Vector3 playerVelocity;
        //[HideInInspector]
        public Vector3 force;

        public void SetUpdating(bool isUpdating) {
            if (isUpdating)
            {
                this.enabled = true;
                IsUpdating = true;
                
            }
            else {
                this.enabled = false;
                IsUpdating = false;
            }
        }
        

        public void AddForce(Vector3 force)
        {
            this.force = force;
        }

        public void Move(Vector3 dir)
        {
            // cc.Move(dir*Time.deltaTime);
          //  Debug.Log(dir);
            moveDir = dir*moveSpeed;

        }

        public void FreezePlayerVelocity()
        {
            playerVelocity = Vector3.zero;
        }

        public void Rotate(Vector3 dir)
        {
            if (dir.sqrMagnitude > 0)
            {
                cc.transform.forward = dir;
            }
        }

        public void PhysicalUpdate()
        {

            //add gravity
            bool groundedPlayer = cc.isGrounded;

            if (groundedPlayer)
            {
                if (!IsOnGround)
                {
                    IsOnGround = true;
                    //touchGround.Act();
                }
                playerVelocity.y = -20f;
            }
            else
            {
                if (IsOnGround)
                {
                    IsOnGround = false;
                    //onAir.Act();
                }
                playerVelocity.y -= gravity * Time.deltaTime;
            }


            //smoothing force
            force *= 0.9f;
            if (force.sqrMagnitude <= 0.1)
            {
                force = Vector3.zero;
            }


            //update movement
            Vector3 move = force + moveDir+ playerVelocity;
            cc.Move(move * Time.deltaTime);


            //update gravity
            

        }

        public void FreezeGravity()
        {
            FreezePlayerVelocity();
        }

        private void FixedUpdate()
        {

            PhysicalUpdate();
            
        }

        private void Update()
        {
            NotifySpeed();
        }

        private void NotifySpeed() {
            if (isUpdating) {
                
                OnNotify.Invoke(moveDir);
            }
        }
    }
}