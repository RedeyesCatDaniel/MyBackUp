using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice.Framework 
{
    public class UnSubscribeOnDestroyTrigger : MonoBehaviour
    {
        private HashSet<IUnSubscribe> mUnSubscribes = new HashSet<IUnSubscribe>();
        public void AddUnSubscribes(IUnSubscribe unSubscribe) 
        {
            mUnSubscribes.Add(unSubscribe);
        }
        private void OnDestroy()
        {
            foreach (var item in mUnSubscribes)
            {
                item.UnSubscribe();
            }
            mUnSubscribes.Clear();
        }
    }
}
