using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;
namespace LGUVirtualOffice
{

    //This Script should be placed on root bones so that it could capture all bones. 
    public class avBearMeshStitcher : MonoBehaviour//SerializedMonoBehaviour
    {
        public Dictionary<string, Transform> boneDic = new Dictionary<string, Transform>();

       
        private void Awake()
        {
            AddBone(transform);
        }

        //This function will add all bones under current Transformation into Bone Dictionary
        
        private void AddBone(Transform target)
        {
            if (target == null) return;
            boneDic[target.name] = target;

            for (int i = 0; i < target.childCount; i++)
            {
                AddBone(target.GetChild(i));
            }
        }


        //Summary
        //this function will match mesh on current bone
        //!!!Notice the names in skinned mesh renderer should be a subset of bones in bone dictonary 
        public void Stitch(SkinnedMeshRenderer mesh)
        {
            mesh.rootBone = transform;
            Transform[] bones = mesh.bones;
            for (int i = 0; i < bones.Length; i++)
            {
                string name = bones[i].name;
                if (!boneDic.TryGetValue(name, out bones[i]))
                {
                    Debug.Log($"Unable to match bone {name}");

                }
            }
            mesh.bones = bones;
        }

        //this stitch use the name in input instead of SkinnedMeshRenderer
        public void Stitch(SkinnedMeshRenderer mesh,string[] boneNames)
        {
            mesh.rootBone = transform;
            Transform[] bones = new Transform[boneNames.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                string name = boneNames[i];
                if (!boneDic.TryGetValue(name, out bones[i]))
                {
                    Debug.Log($"Unable to match bone {name}");

                }
            }
            mesh.bones = bones;
        }
    }
}