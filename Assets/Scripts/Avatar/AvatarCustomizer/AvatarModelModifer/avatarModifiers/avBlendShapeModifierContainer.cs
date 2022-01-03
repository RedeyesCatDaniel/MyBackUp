using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avBlendShapeModifierContainer:MonoBehaviour {
        public avBlendShapeModifierData data;

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
    public class avBlendShapeModifierData : avModifierData<avBlendShapeModifier>
    {
        public const string key = "avBlendShapeModifierData";
        public avBearDictionary<string, avBlendShapeModifier> modifiers;

        

        protected override string GetJsonData()
        {
            return modifiers.GetJson();
        }

        protected override string GetKey()
        {
            return key;
        }

        protected override Dictionary<string, avBlendShapeModifier> GetModifiers()
        {
            modifiers.Init();
            return modifiers.dic;
        }

        protected override void InitDic(string json)
        {
            modifiers.Init(avDictionarySerializer.DeSerializeDictionary<string, avBlendShapeModifier>(json));
        }
    }

    [System.Serializable]
    public class avBlendShapeModifier : IAvatarModifier
    {
        public avBearDictionary<string, float> blendshapes = new avBearDictionary<string, float>();
      //  public FeatureGroup group;
        public string TargetModifier;


        public void Modify(avAvatarRenderer renderer)
        {
            if (renderer == null) {
                return;
            }
            //renderer.avBearSkinnedMeshModifiers.Init();
            if (renderer.ModifierByName.TryGetValue(TargetModifier, out avBearSkinnedMeshModifier smm))
            {
                //smm.ReplaceMeshAndStitch();
                foreach (var item in blendshapes.kpv)
                {
                    smm.SetBlendShape(item.key,item.value);
                }
            }

        }
    }
}