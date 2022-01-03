using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LGUVirtualOffice
{
    public class avMeshModifierContainer:MonoBehaviour {
        public avMeshModifierData data;
        [ContextMenu(itemName: "Pull")]
        public void DefaultPull()
        {
            data.Pull(() => { });
        }

        [ContextMenu(itemName: "Push")]
        public void DefaultPush()
        {
            data.Push(() => { });
        }

    }

    [System.Serializable]
    public class avMeshModifierData : avModifierData<avMeshModifier>
    {
        public const string key = "MeshModifierContainerData";
        public avBearDictionary<string, avMeshModifier> modifiers = new avBearDictionary<string, avMeshModifier>();

        

        protected override string GetJsonData()
        {
            return modifiers.GetJson();
        }

        protected override string GetKey()
        {
            return key;
        }

        protected override Dictionary<string, avMeshModifier> GetModifiers()
        {
            modifiers.Init();
            return modifiers.dic;
        }

        protected override void InitDic(string json)
        {
            modifiers.Init(avDictionarySerializer.DeSerializeDictionary<string, avMeshModifier>(json));
        }
    }

    [System.Serializable]
    public class avMeshModifier : IAvatarModifier
    {
        // public SkinnedMeshRenderer Data;
        public AssetReference reference;
     //   public FeatureGroup group;
        public string TargetSMR;

        private SkinnedMeshRenderer Renderer { get; set; }
        private bool Initialized { get; set; }
        public void Modify(avAvatarRenderer renderer)
        {
            if (!Initialized)
            {
                Addressables.LoadAssetAsync<GameObject>(reference).Completed += (x) =>
                {
                    if (x.Result.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer smr))
                    {
                        Renderer = smr;
                        Initialized = true;
                    }
                    //  Debug.Log(x.Result);
                    Modify(renderer, Renderer);
                };
            }
            else
            {
                Modify(renderer, Renderer);
            }



        }

        private void Modify(avAvatarRenderer renderer, SkinnedMeshRenderer smr)
        {
            if (renderer.ModifierByName.TryGetValue(TargetSMR, out avBearSkinnedMeshModifier smm))
            {
                //smm.ReplaceMeshAndStitch();
                smm.ReplaceMesh(smr);
                renderer.stitcher.Stitch(smm.myRenderer);
            }
        }
    }
}