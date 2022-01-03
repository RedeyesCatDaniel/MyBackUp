using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avUIArray : MonoBehaviour
    {
        public avUIArrayElement instance;
        public int activecount;
        public List<avUIArrayElement> kids;



        //this function will enlarge the kids array
        public bool Expend(int count) {
            if (count > kids.Count)
            {
                for (int i = kids.Count; i < count; i++)
                {
                    avUIArrayElement kid = Instantiate<avUIArrayElement>(instance, transform);
                    kids.Add(kid);
                }
                return true;
            }
            else {
                return false;
            }
        }


        //This function will activate kids to target number
        public void ActivateUpTo(int index) {
            //activate kids
            for (int i = 0; i < index; i++)
            {
                avUIArrayElement kid = kids[i];
                kid.Index = i;
                if (!kid.gameObject.activeSelf) {
                    kid.gameObject.SetActive(true);
                }
            }

            //deactivate kids
            for (int i = index; i < kids.Count; i++)
            {
                avUIArrayElement kid = kids[i];
                kid.Index = -1;
                if (kid.gameObject.activeSelf)
                {
                    kid.gameObject.SetActive(false);
                }
            }

            activecount = index;
        }
    }
}