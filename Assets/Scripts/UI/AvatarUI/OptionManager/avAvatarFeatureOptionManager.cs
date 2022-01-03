using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "UIData/avAvatarFeatureOptionManager")]
    public class avAvatarFeatureOptionManager : ScriptableObject
    {
        public avBearDictionary<FeatureGroup, List<avFeatureOptions>> options;


        public void Init() {
            options.Init();
        }

        public int GetFeatureCount(FeatureGroup group) {
            if (options.TryGetValue(group,out List<avFeatureOptions> realoptions)) {
                return realoptions.Count;
            }
            return 0;
        }

        public string GetID(FeatureGroup group, int index) {
            if (options.TryGetValue(group, out List<avFeatureOptions> realoptions))
            {
                return realoptions[index].id;
            }
            return "";
        }

        public Sprite GetSprite(FeatureGroup group, int index)
        {
            if (options.TryGetValue(group, out List<avFeatureOptions> realoptions))
            {
                return realoptions[index].sprite;
            }
            return default;
        }
    }
}