using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [System.Serializable]
    public struct FeatureGroup
    {
        public FeatureType type;
        public override bool Equals(object obj)
        {
            if (obj is FeatureGroup ft)
            {
                return type == ft.type;
            }
            else {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)type;
        }
    }

    public struct FeatureSelection {
        public FeatureType type;
        public string featureName;
    }

    public enum FeatureType { 
        FaceShape,
        Hair,
        Eyebrow,
        Eyes,
        Nose,        
        Mouth,
        Eyewear,
        Beard,
        Clothing,
        Background,
        Gender
    }

   
}
