using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LGUVirtualOffice
{
    public class avSkinnedMeshRendererModifierContainer : MonoBehaviour
    {
        
    }

    [System.Serializable]
    public class avSkinnedMeshRendererModifierData : avModifierData<avSkinnedMeshRendererModifier>
    {
        public const string key = "avBlendShapeModifierData";
        public avBearDictionary<string, avSkinnedMeshRendererModifier> modifiers;

        protected override string GetJsonData()
        {
            return modifiers.GetJson();
        }

        protected override string GetKey()
        {
            return key;
        }

        protected override Dictionary<string, avSkinnedMeshRendererModifier> GetModifiers()
        {
            modifiers.Init();
            return modifiers.dic;
        }

        protected override void InitDic(string json)
        {
            modifiers.Init(avDictionarySerializer.DeSerializeDictionary<string, avSkinnedMeshRendererModifier>(json));
        }
    }

    [System.Serializable]
    public class avSkinnedMeshRendererModifier : IAvatarModifier
    {
        public AssetReference[] allRenderer;
        public string optionId;
        //public Shader shaderForMesh;

        private AsyncHandler handler = new AsyncHandler();
        public IAsyncHandler Handler => handler;
        
        public void Modify(avAvatarRenderer renderer)
        {

            List<GameObject> created = new List<GameObject>();

            avAttachmentPoint point = renderer.attachPoint;
            avBearMeshStitcher stitcher = renderer.stitcher;
            List<Transform> made = new List<Transform>();

            //find point to attach clothing
            
            //var almm = trans.gameObject.AddComponent<avLocalMaterialManager>();
            //almm.localShader = shaderForMesh;

            //Register action => Initialize skinned mesh renderers after they are all generated
            AsyncClicker clicker = new AsyncClicker(allRenderer.Length, () => {
                //almm.RefreshMat();
                if (point != null)
                {
                    point.ClearPoint(optionId);
                    Transform trans = point.GetPoint(optionId);
                    foreach (var item in made)
                    {
                        item.SetParent(trans);
                        item.localPosition = Vector3.zero;
                        renderer.OnSMRMade(optionId);
                    }
                }
            });


            //Generate skinned mesh renderers;
            foreach (var item in allRenderer)
            {
                if (stitcher == null)
                    break;

                //When target is on memory
                if (item.IsDone) {
                    item.InstantiateAsync().Completed += (x) => {
                        if (x.Result.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer smr))
                        {
                            string[] names = GetBoneNames(smr);
                            stitcher.Stitch(smr, names);
                            //almm.myRenderers.Add(smr);
                            made.Add(smr.transform);
                            clicker.Click();
                           
                        }
                    };
                }
                //When target is not on memory
                else
                {
                    item.LoadAssetAsync<GameObject>().Completed += (x) => {
                        if (x.Result.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer smr))
                        {
                            string[] names = GetBoneNames(smr);
                            var mesh = MonoBehaviour.Instantiate<SkinnedMeshRenderer>(smr);
                            mesh.transform.localPosition = Vector3.zero;
                            stitcher.Stitch(mesh, names);
                            //almm.myRenderers.Add(smr);
                            made.Add(smr.transform);
                            clicker.Click();
                            
                        }
                    };
                }
            }
        }

        public static string[] GetBoneNames(SkinnedMeshRenderer renderer)
        {
            int length = renderer.bones.Length;
            string[] names = new string[length];
            for (int i = 0; i < length; i++)
            {
                
                string bname = renderer.bones[i].name;
                Debug.Log(bname);
                names[i] = bname;
            }
            return names;

        }
    }

    
}