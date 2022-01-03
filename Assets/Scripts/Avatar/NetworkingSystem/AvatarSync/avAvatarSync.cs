using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avAvatarSync : MonoBehaviour
    {
        public avAvatarMemberInfoManager infoMan;
        public avAvatarBodySwitcher switcher;
        public void ReloadAvatar() {
            if (infoMan == null)
                return;
            infoMan.FetchID((id)=> {
                infoMan.FetchGender((gender) => {
                    if (switcher == null) return;
                    switcher.Switch(gender);
                    memService.PullData<string>(id, avAvatarKeys.Avatar_Data,(x) => {
                        if (switcher == null) return;

                        var choices = avDictionarySerializer.DeSerializeDictionary<FeatureGroup, string>(x);
                        avGlobalModifierManager.Modify(switcher.body, choices.Values);
                        memService.PullData<string>(id, avAvatarKeys.Avatar_Color_Data, (colordic) => {
                            if (switcher == null) return;
                            var choices = avDictionarySerializer.DeSerializeDictionary<FeatureGroup, Color>(colordic);
                            foreach (var choice in choices)
                            {
                                switcher.body.ChangeColor(choice.Key,choice.Value);
                            }
                        });
                    });
                });
            });
            
        }
    }
}