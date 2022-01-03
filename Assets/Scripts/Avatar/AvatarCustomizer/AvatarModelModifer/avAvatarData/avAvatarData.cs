using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "avAvatarData")]
    public class avAvatarData : ScriptableObject
    {
        public avBearDictionary<FeatureGroup, string> modfiers;
        public avBearDictionary<FeatureGroup, Color> colorModifiers;

       
    }
}