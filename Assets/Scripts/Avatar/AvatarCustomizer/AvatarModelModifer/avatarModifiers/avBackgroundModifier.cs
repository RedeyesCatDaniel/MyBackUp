using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LGUVirtualOffice
{
    [System.Serializable]
    public class avBackgroundModifierData : avModifierData<avBackgroundModifier>
    {
        public const string key = "avBackgroundModifierData";
        public avBearDictionary<string, avBackgroundModifier> modifiers;
        protected override string GetJsonData()
        {
            return modifiers.GetJson();
        }

        protected override string GetKey()
        {
            return key;
        }

        protected override Dictionary<string, avBackgroundModifier> GetModifiers()
        {
            modifiers.Init();
            return modifiers.dic;
        }

        protected override void InitDic(string json)
        {
            modifiers.Init(avDictionarySerializer.DeSerializeDictionary<string, avBackgroundModifier>(json));
        }
    }

    [System.Serializable]
    public class avBackgroundModifier : IAvatarModifier
    {
        public Material skyBoxMaterial;
        public void Modify(avAvatarRenderer renderer)
        {
            if(SceneManager.GetActiveScene().name != "MainScene")
                RenderSettings.skybox = skyBoxMaterial;
        }
    }
}