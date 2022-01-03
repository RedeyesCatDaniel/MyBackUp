using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avOptionManager : MonoBehaviour
    {
        public avBearDictionary<string, Sprite> options;
    }

    [System.Serializable]
    public class avFeatureOptions
    {
        public string id;
        public Sprite sprite;
    }
}