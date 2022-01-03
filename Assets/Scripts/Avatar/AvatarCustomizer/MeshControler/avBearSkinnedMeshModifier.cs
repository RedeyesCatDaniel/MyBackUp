using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{

    //Warning when change the bone during play, if there is a error that cause the stoppage of the game
    //it may lead to mesh deformations
    public class avBearSkinnedMeshModifier : MonoBehaviour
    {
        public Dictionary<string, int> bones = new Dictionary<string, int>();
        public SkinnedMeshRenderer myRenderer;
        private Matrix4x4[] originalBindPoses;
 
        public void Awake()
        {
            Init();

        }

        public void Init() {
            bones.Clear();

            // Get All bones in skinned mesh renderer
            for (int i = 0; i < myRenderer.bones.Length; i++)
            {
                Transform bone = myRenderer.bones[i];
                bones[bone.name] = i;
            }

            // Remember the original Bind Poses
            originalBindPoses = myRenderer.sharedMesh.bindposes;
        }

        //This Function will Transform target bone with a Matrix4x4 transformation 
        //For example: if transformation is a scale matrix, it will scale target bone with same amount 
        public void Transform(string boneName, Matrix4x4 transformation)
        {
            //Get Bone
            int index = bones[boneName];
            Transform bone = GetBoneAt(boneName);

            //Apply Transformation to target bone
            Matrix4x4 result = transformation * originalBindPoses[index];
          
            // write new bindposes back to skinned mesh renderer
            Matrix4x4[] bindposes = myRenderer.sharedMesh.bindposes;
            bindposes[index] = result;
            myRenderer.sharedMesh.bindposes = bindposes;

        }


        //Find bind pose by bone name
        public Matrix4x4 GetBindPose(string boneName)
        {
            return myRenderer.sharedMesh.bindposes[bones[boneName]];
        }

        //Get complete bind pose infomation at this moment
        public Matrix4x4[] GetBindPoses()
        {
            return myRenderer.sharedMesh.bindposes;
        }

        //Find bone transformation by bone name
        public Transform GetBoneAt(string boneName)
        {
            int index = bones[boneName];
            return myRenderer.bones[index];
        }


        //This function could replace meshes of current skinned mesh renderer
        //remember to rebind bones after replacement
        //Using Stitch in Bear Mesh Stitcher
        public void ReplaceMesh(SkinnedMeshRenderer replacableMesh) {
            ReplaceMesh(replacableMesh.bones, replacableMesh.sharedMesh);
        }

        public void ReplaceMesh(Transform[] bones, Mesh newMesh)
        {
            myRenderer.enabled = false;
            myRenderer.bones = bones;
            myRenderer.sharedMesh = newMesh;
            Init();
            myRenderer.enabled = true;
        }

        public void ReplaceMesh(string[] bones, Mesh newMesh)
        {
            myRenderer.enabled = false;
           // myRenderer.bones = bones;

            myRenderer.sharedMesh = newMesh;
            Init();
            myRenderer.enabled = true;
        }


        public void ReplaceMeshAndStitch(Mesh replacableMesh, string[] boneNames, avBearMeshStitcher stitcher) {
            

            myRenderer.enabled = false;
            Transform[] newBones = new Transform[boneNames.Length];
            for (int i = 0; i < newBones.Length; i++)
            {
                string boneName = boneNames[i];
                if (stitcher.boneDic.TryGetValue(boneName, out Transform bone))
                {
                    newBones[i] = bone;
                }
                else
                {
                    Debug.LogWarning($"Bone {boneName} cannot be found");
                    return;
                }
            }

            myRenderer.bones = newBones;
            myRenderer.sharedMesh = replacableMesh;


            Init();
            myRenderer.enabled = true;

            stitcher.Stitch(myRenderer);

        }

        public int GetBlendShapeIndex(string name) {
            return myRenderer.sharedMesh.GetBlendShapeIndex(name);
        }

        public void SetBlendShape(string name, float value) {
            int index = GetBlendShapeIndex(name);
            if (index >=0) {
                myRenderer.SetBlendShapeWeight(index, value);
            }
           
        }

        

        //When Disable, it will write original bind poses back
        //!!!!Warning: the change to bind poses are permenant!!!
        // ^^^It means bindposes will not restore automatically after you end the game
        // Because bindposes are stored in mesh.
        //public void OnDisable()
        //{
        //    myRenderer.sharedMesh.bindposes = originalBindPoses;
            
        //}
    }
}