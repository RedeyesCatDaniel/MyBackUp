using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avAttachmentPoint : MonoBehaviour
    {
        public Dictionary<string, Transform> creation = new Dictionary<string, Transform>();

        public List<Transform> defaultAttachpoint;
        private void Awake()
        {
            foreach (var item in defaultAttachpoint)
            {
                creation[item.name] = item;
                
            }
        }
        //public Transform[] myrenderers;

        //private void Awake()
        //{
        //    foreach (var item in myrenderers)
        //    {
        //        creation[item.name] = item;
        //    }
        //}
        public Transform GetPoint(string id) {
            Debug.Log(id);
            Debug.Log(creation.Keys);
            if (creation.TryGetValue(id, out Transform rs)){
                return rs;
            }
            else {
                GameObject obj = new GameObject(id);
                obj.transform.SetParent(transform);
                Debug.Log($"I placed {id} into creation");
                //var created = Instantiate(obj, transform);
                creation[id] = obj.transform;
                return obj.transform;
            }
        }

        public void ClearPoint(string id) {
            if (creation.TryGetValue(id, out Transform rs))
            {
                foreach (Transform item in rs)
                {
                    Destroy(item.gameObject);
                }
                
                
            }
            //else {
            //    Debug.Log($"I want to clear {id}");
            //    foreach (var item in creation)
            //    {
            //        Debug.Log(item.Key);
            //    }
            //}
        }
        
    }
}
