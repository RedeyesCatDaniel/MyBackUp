using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace LGUVirtualOffice
{
    public class avDualCameraManager : MonoBehaviour
    {
        public Cinemachine.CinemachineMixingCamera mixer;
        public float multiplier;

        //0 indicate it is camera A and 1 means camera1
        private float currentmix = 0;
        public void UpdateMix(float delta)
        {
            Debug.Log($"I moved weight by {delta}");

            currentmix += delta;
            currentmix = Mathf.Clamp01(currentmix);
            float weightA = currentmix;
            float weightB = 1 - currentmix;
            mixer.SetWeight(0, weightA);
            mixer.SetWeight(1, weightB);
            //float weight = mixer.GetWeight(0);
            //weight += delta;
            //weight = Mathf.Clamp(weight,0,max);
            //mixer.SetWeight(0, weight);
        }

        public void UpdateMix(InputAction.CallbackContext input)
        {
            UpdateMix(input.ReadValue<float>()* multiplier);
            
        }
    }
}
