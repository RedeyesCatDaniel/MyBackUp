using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public interface IAvatarModifier
    {
        public void Modify(avAvatarRenderer renderer);
    }

    public class avModifierID {
        FeatureGroup group;
        string name;
    }

}