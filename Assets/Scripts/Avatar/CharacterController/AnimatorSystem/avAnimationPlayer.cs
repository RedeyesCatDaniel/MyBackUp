using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avAnimationPlayer : MonoBehaviour
    {
        private avAnimatorController animController;
        public List<AnimationPlayerInfo> info;
       //public avMovementManager manager;
        private void Awake()
        {
            animController = GetComponent<avAnimatorController>();
        }

        public void Play(int index) {

            //if (manager.IsMoving())
            //{
            //    return;
            //}
            info[index].Play(animController);
           
        }

        [ContextMenu(itemName: "Test")]
        private void PlayRandom() {
            Play(Random.Range(0,info.Count));
        }
    }

    [System.Serializable]
    public class AnimationPlayerInfo {
        public string clipName;
        public bool isCrossingFading;
        public float normalizedTime;

        //public UnityEvent OnPlay;
        

        public void Play(avAnimatorController anim) {

            if (anim == null) return;
            anim.view.RPC("EnterAnimationState", Photon.Pun.RpcTarget.Others, clipName);
            if (isCrossingFading)
            {
                anim.anim.CrossFade(clipName, normalizedTime);
               
            }
            else {
               // Debug.Log($"I Played {clipName} :{isCrossingFading}");
                anim.anim.Play(clipName, 0, normalizedTime);
            }
        }


    }
}
