using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avBearDictionaryContainer : MonoBehaviour {
        public avBearDictionary<string, avColorModifier> dic;
        [ContextMenu(itemName:"pull")]
        public void Pull() {
            avJsonToolkit.Read(avColorModifierData.key,(x)=>{
                dic = JsonUtility.FromJson<avBearDictionary<string, avColorModifier>>(x);
                print("read");
            });
            
        }
        [ContextMenu(itemName: "login")]
        public void Login() {
            UserToolkit.DefaultLogin();
        }
    }   
}