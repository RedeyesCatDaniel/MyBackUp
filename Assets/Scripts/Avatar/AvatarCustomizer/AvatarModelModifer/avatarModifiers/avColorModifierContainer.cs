using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avColorModifierContainer:MonoBehaviour {
        public avColorModifierData data;
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
    public class avColorModifierData : avModifierData<avColorModifier>
    {
        public const string key = "avColorModifierData";
        public avBearDictionary<string, avColorModifier> modifiers = new avBearDictionary<string, avColorModifier>();

        private Dictionary<FeatureGroup, List<Color>> colors;

        public Dictionary<FeatureGroup, List<Color>> GetColorByFeatureGroup() {
            if (colors == null) {
                InitColors();
            }

            return colors;
        }

        private void InitColors() {
            colors = new Dictionary<FeatureGroup, List<Color>>();
            foreach (var item in modifiers.kpv)
            {
                avColorModifier modifier = item.value;
                if (colors.TryGetValue(modifier.group, out List<Color> colorlis))
                {
                    colorlis.Add(modifier.color);
                }
                else {
                    colors[modifier.group] = new List<Color>() { modifier.color };
                }
            }
        }
        protected override string GetJsonData()
        {
            return modifiers.GetJson();
        }

        protected override string GetKey()
        {
            return key;
        }

        protected override Dictionary<string, avColorModifier> GetModifiers()
        {
            modifiers.Init();
            return modifiers.dic;
        }

        protected override void InitDic(string json)
        {            
            modifiers.Init(avDictionarySerializer.DeSerializeDictionary<string, avColorModifier>(json));
        }
    }

    [System.Serializable]
    public class avColorModifier : IAvatarModifier
    {
        public FeatureGroup group;
    //    public string colorName;
        public Color color;
        public void Modify(avAvatarRenderer renderer)
        {
          //  string key = group.type.ToString();
            
        }
    }
}