using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avTextureModifierContainer : MonoBehaviour
    {
        public avTextureModifierData data;
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
    public class avTextureModifierData : avModifierData<avTextureModifier>
    {
        public const string key = "avTextureModifierData";
        public avBearDictionary<string, avTextureModifier> modifiers = new avBearDictionary<string, avTextureModifier>();

        protected override string GetJsonData()
        {
            return modifiers.GetJson();
        }

        protected override string GetKey()
        {
            return key;
        }

        protected override Dictionary<string, avTextureModifier> GetModifiers()
        {
            modifiers.Init();
            return modifiers.dic;
        }

        protected override void InitDic(string json)
        {
            modifiers.Init(avDictionarySerializer.DeSerializeDictionary<string, avTextureModifier>(json));
        }
    }

    [System.Serializable]
    public class avTextureModifier : IAvatarModifier
    {
        public string textureName;
        public Texture texture;
        public FeatureGroup group;
        public void Modify(avAvatarRenderer renderer)
        {
            renderer.ChangeTexture(group, textureName,texture);
        }
    }
}