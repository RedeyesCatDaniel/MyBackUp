using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    //[CreateAssetMenu(menuName = "Toolkit/avAvatarModifierToolkit")]
    //public class avAvatarModifierToolkit : ScriptableObject
    //{
    //    public void PushMyModifiers() {
    //        string key = "AvatarData" + UserInfo.Instance.UserId;
    //        string data = JsonUtility.ToJson(avGlobalModifierManager.GetAvatarData());
    //     //   Debug.Log(avGlobalModifierManager.GetSingleModifiers().Count);
    //      //  Debug.Log(data);
    //        avJsonToolkit.Write(key,data,(x)=> { 
    //            if (x) {
    //                Debug.Log("I have write json into modifiers");
    //            } });



    //    }

    //    public void PullMyModifiers() {
    //        string key = "AvatarData_" + UserInfo.Instance.UserId;
    //        avJsonToolkit.Read(key,(rs)=> {
    //            AvatarData data = JsonUtility.FromJson<AvatarData>(rs);
    //            avGlobalModifierManager.UpdateAvatarData(data);
                
    //        });
            
    //    }

    //    public void PushAllModifiers() {
    //        ModifierTable table = new ModifierTable(avGlobalModifierManager.modifiers);
    //        string data = JsonUtility.ToJson(table);
    //        avJsonToolkit.Write("AllModifiers", data, (x) => {
    //            if (x)
    //            {
    //                Debug.Log("I have write modifiers into jsons");
    //            }
    //        });
    //    }

    //    public void PullAllModifiers() {
    //        avJsonToolkit.Read("AllModifiers", (rs) => {
    //            ModifierTable data = JsonUtility.FromJson<ModifierTable>(rs);
    //            avGlobalModifierManager.UpdateModifierData(data);

    //        });
    //    }
    //}
}