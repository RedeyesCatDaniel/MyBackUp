using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avNavigationManager : MonoBehaviour
    {

        public NavMeshAgent agent;
        public LayerMask walkableLayers;
        public avMovementManager avMovementManager;
       // public Transform mainTransform;
        private bool isInControl;

        public UnityEvent<Vector3> Notify;
        public UnityEvent<Vector3> OnSetDestination;
        public void SetDestination(Vector3 position) {
            agent.SetDestination(position);
        }

        private void Awake()
        {
            
            agent.updateRotation = false; 
        }

        public void SetDestination() {
            //  agent.enabled = true;
          //  agent.updatePosition = true;
            
            if (avMouseOnGround.RayCastToGround(1000,walkableLayers, out RaycastHit rs)) {
                isInControl = true;
                agent.isStopped = false;
                avMovementManager.SetUpdating(false);
               // Debug.Log($"I have set destination at {rs.point}");
               // Debug.DrawLine(transform.position, rs.point);
                SetDestination(rs.point);
                OnSetDestination.Invoke(rs.point);
              //  agent.transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
            }
        }

        public void StopNavigation() {
         //   agent.updatePosition = false;
            //agent.enabled = false;
            agent.isStopped = true;
         //   agent.velocity = Vector3.zero;
            isInControl = false;
            
        }
        public void SpeedObserve(Vector3 dir) {
            if (dir.sqrMagnitude > 0)
            {
                StopNavigation();
                avMovementManager.SetUpdating(true);
                
            }
            
        }

        private void Update()
        {
            if (isInControl)
            {
                Notify.Invoke(agent.velocity);
                if (agent.velocity.sqrMagnitude > 1f)
                {
                    Quaternion dir = Quaternion.LookRotation(agent.velocity.normalized);
                    dir.x = 0;
                    dir.z = 0;
                    agent.transform.rotation = dir;
                }
            }
        }



    }
}