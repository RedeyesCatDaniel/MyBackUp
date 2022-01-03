using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public static class BasicMemberInfoExtension 
    {
        public static void FetchData<T>(this avAvatarMemberInfoManager info,string dataName, System.Action<T> rs) {
            info.FetchID((x)=> {
                memService.PullData<T>(x, dataName).OnCompleted((dic)=> {
                    if (dic.TryGetValue(dataName, out T val)) {
                        rs?.Invoke(val);
                    }
                });
            });
        }

        public static void PushData<T>(this avAvatarMemberInfoManager info, string dataName, T val, System.Action OnPushDone)
        {
            info.FetchID((x) => {
                memService.PushData<T>(x, dataName,val).OnCompleted((x)=> {
                    if (x) {
                        OnPushDone?.Invoke();
                    }
                });
            });
        }
    }
}