using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avDictionarySerializer : MonoBehaviour
    {
        // Start is called before the first frame update
        public static string SerializeDictionary<K, V>(Dictionary<K, V> dic) {
            return JsonUtility.ToJson(new DictionaryData<K,V>(dic));
        }

        public static Dictionary<K, V> DeSerializeDictionary<K, V>(string json) {
            return JsonUtility.FromJson<DictionaryData<K, V>>(json).GetDictionary();
        }


    }

    [System.Serializable]
    public class DictionaryData<K, V>{
        public List<K> keys = new List<K>();
        public List<V> values = new List<V>();
        public DictionaryData(Dictionary<K, V> dic) {
            foreach (var item in dic)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
        }

        public Dictionary<K, V> GetDictionary() {
            Dictionary<K, V> rs = new Dictionary<K, V>();
            for (int i = 0; i < keys.Count; i++)
            {
                K key = keys[i];
                V value = values[i];
                rs[key] = value;
            }

            return rs;
        }

        
    }

    
}