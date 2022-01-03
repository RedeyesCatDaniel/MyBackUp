using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avAvatarBlinkManager : MonoBehaviour
    {
        public SkinnedMeshRenderer smr;
        public List<float> blendShapes;
        public List<int> targets;
       // public int smallest;
        private void Awake()
        {
            for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
            {
                float value = smr.GetBlendShapeWeight(i);
                blendShapes.Add(value);
            } 
        }

        public void SetToDefault() {
            Debug.Log("I have set everything to default");
            for (int i = 0; i < targets.Count; i++)
            {
                smr.SetBlendShapeWeight(targets[i], 0);
            }
          //  smr.SetBlendShapeWeight(smallest, 100);
        }

        public void SetToDefault(float percentage)
        {
           // Debug.Log("I have set everything to default");
            
            for (int i = 0; i < targets.Count; i++)
            {
                int index = targets[i];
                smr.SetBlendShapeWeight(index, blendShapes[index] * percentage);
            }
            //  smr.SetBlendShapeWeight(smallest, 100);
        }

        public void ResetShapes()
        {
           
            for (int i = 0; i < targets.Count; i++)
            {
               // Debug.Log($"I reset {smr.sharedMesh.GetBlendShapeName(targets[i])} to original {blendShapes[targets[i]]}");
                smr.SetBlendShapeWeight(targets[i], blendShapes[targets[i]]);
            }

        }
    }
}
