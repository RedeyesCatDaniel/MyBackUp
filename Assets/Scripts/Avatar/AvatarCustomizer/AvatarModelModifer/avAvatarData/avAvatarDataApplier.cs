using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avAvatarDataApplier : MonoBehaviour
    {
        public avAvatarData[] data;
        public GlobalModelUIManager gmui;

        public void Apply() {
            int gender = avGlobalModifierManager.gender;
            foreach (var item in data[gender].modfiers.kpv)
            {
                gmui.ApplyModification(gender,item.key,item.value);
            }

            foreach (var item in data[gender].colorModifiers.kpv)
            {
                gmui.ChangeColor(gender,item.key, item.value);
            }
        }

        public void Apply(int choice)
        {
            int gender = choice;
            foreach (var item in data[gender].modfiers.kpv)
            {
                gmui.ApplyModification(gender, item.key, item.value);
            }

            foreach (var item in data[gender].colorModifiers.kpv)
            {
                gmui.ChangeColor(gender,item.key, item.value);
            }
        }
    }
}