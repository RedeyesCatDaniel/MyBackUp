using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class AssetLoader : MonoBehaviour,IAsyncAction
    {
        public AssetReference[] assets;
        public Action DONfinish;
        public UnityEvent onFinish;

        public void Act()
        {
            AsyncClicker clicker = new AsyncClicker(assets.Length,()=> {
                DONfinish?.Invoke();
                DONfinish = null;
                onFinish.Invoke();
            });
            foreach (var item in assets)
            {
                item.LoadAssetAsync<GameObject>().Completed+=(_)=> {
                    clicker.Click();
                };
            }
        }

        public void OnFinish(Action action)
        {
            DONfinish = action;
        }
    }
}
