using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice {
    public class avAvatarSmoothBlink : StateMachineBehaviour
    {
        private avAvatarBlinkManager abm;

        public bool Reversed;
        public avAvatarBlinkManager GetABM(Animator animator) {
            if (abm == null) {
                abm = animator.GetComponent<avAvatarBlinkManager>();
            }
            return abm;
        }
        
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    if (ResetOnEnter)
        //        GetABM(animator).SetToDefault();
        //}

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float smooth = !Reversed ? 1 - stateInfo.normalizedTime : stateInfo.normalizedTime;
            GetABM(animator).SetToDefault(smooth);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Reversed)
            {
                GetABM(animator).ResetShapes();
            }
            else {
                GetABM(animator).SetToDefault();
            }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}