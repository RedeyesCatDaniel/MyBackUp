using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avAnimationSync : MonoBehaviour
    {
        public Animator anim;
        //public avStateManager manager;
        [PunRPC]
        void EnterAnimationState(string stateName)
        {
            anim.Play(stateName,0,0);
        }

        public void EnterState(int state) {
            if (avPlayerStateManager.TryGetAvPlayerStateManager(out avPlayerStateManager instance)) {
                instance.stateManager.EnterState(state);
            }
        }
    }
}