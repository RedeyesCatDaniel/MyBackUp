using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public abstract class avModifierData<T> : netISymchronizable
        where T:IAvatarModifier
    {
        public void Pull(System.Action OnPull)
        {
            avJsonToolkit.Read(GetKey(), (v) => {
                InitDic(v);
                OnPull();
            });
        }

        public void Push(System.Action OnPush)
        {
            string json = GetJsonData();
            avJsonToolkit.Write(GetKey(), json, (x) => {
                if (x)
                {
                    OnPush();
                }
            });
        }

        public void InjectData(Dictionary<string,IAvatarModifier> dic) {
            foreach (var item in GetModifiers())
            {
                dic[item.Key] = item.Value;
            }
        }

        protected abstract string GetKey();
        protected abstract void InitDic(string json);
        protected abstract string GetJsonData();

        protected abstract Dictionary<string,T> GetModifiers();
    }
}
