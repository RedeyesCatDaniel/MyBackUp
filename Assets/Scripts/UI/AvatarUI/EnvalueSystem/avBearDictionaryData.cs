using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public interface IBearDictionary<K, V> {
        public void Init();
        public bool TryGetValue(K key, out V value);
        public string GetJson();
        public void Init(Dictionary<K, V> data);
    }

    //[CreateAssetMenu(menuName = "Data/avBearDictionaryData")]
    //public class avBearDictionaryData<K, V> : ScriptableObject, IBearDictionary<K, V>
    //{
    //    public avBearDictionary<K, V> dic; 
    //    public string GetJson()
    //    {
    //        return dic.GetJson();
    //    }

    //    public void Init()
    //    {
    //        dic.Init();
    //    }

    //    public void Init(Dictionary<K, V> data)
    //    {
    //        dic.Init(data);
    //    }

    //    public bool TryGetValue(K key, out V value)
    //    {
    //        return dic.TryGetValue(key, out value);
    //    }
    //}


    [System.Serializable]
    public class avBearDictionary<K,V>: IBearDictionary<K,V>
    {
        public List<avPair<K, V>> kpv = new List<avPair<K, V>>();
        public Dictionary<K, V> dic = new Dictionary<K, V>();


        public void Init() {
            if (dic.Count==0) {
                foreach (var pair in kpv)
                {
                    dic[pair.key] = pair.value;
                }
            }
            

        }

        public bool TryGetValue(K key, out V value) {
            Init();
            return dic.TryGetValue(key, out value);
        }

        public string GetJson() {
            dic.Clear();
            Init();
            return avDictionarySerializer.SerializeDictionary<K, V>(dic);
        }

        public void Init(Dictionary<K,V> data) {
            kpv.Clear();
            dic.Clear();
            foreach (var item in data)
            {
                kpv.Add(new avPair<K, V>(item.Key,item.Value));
                dic[item.Key] = item.Value;
            }
        }

        


    }


    [System.Serializable]
    public class avPair<K, V> {
        public K key;
        public V value;

        public avPair(K key, V value) {
            this.key = key;
            this.value = value;
        }
    }
}