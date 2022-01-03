using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace LGUVirtualOffice
{
    [CreateAssetMenu()]
    public class avResourceManager : ScriptableObject
    {
        public List<avResourceTuple> resources;
        private static Dictionary<string, AssetReference> resourceDic = new Dictionary<string, AssetReference>();
        


        public static T GetResource<T>(string resourceName) {

            AssetReference reference = resourceDic[resourceName];
            var handle = Addressables.LoadAssetAsync<T>(reference);
            
            return default;
        }

       
    }

    [System.Serializable]
    public struct avResourceTuple {
        public string id;
        public AssetReference reference;
    }
}