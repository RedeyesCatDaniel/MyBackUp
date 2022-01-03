using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{

    public class avNodeUIManager : MonoBehaviour
    {
        public avUIArray array;
        public float degreeDiff;
        public List<avNodeOnCirlceUI> kids;
        public UnityEvent OnNoColor;
        public UnityEvent OnShowColor;
        //public int kidCount;


        private void OnEnable()
        {
            Relocate();
        }

        private void Start()
        {
            //
            Init();
            GlobalModelUIManager.instance.DOnGroupSelected.AddListener((x) => Init());
        }


        [ContextMenu(itemName: "Init")]
        public void Init() {
            //Init(kidCount);
            // List<Color> colors = GlobalModelUIManager.instance.GetCurrentColors();
            List<Color> colors = GlobalModelUIManager.instance.GetCurrentColorsModifier();
            Init(colors.Count);
            for (int i = 0; i < colors.Count; i++)
            {
                UIImageManager uimgman = kids[i].Uimgman;
                uimgman.ChangeColorTo(colors[i]);
            }
        }

        public void Init(int count)
        {
            if (count == 0)
            {
                OnNoColor.Invoke();

            }
            else {
                OnShowColor.Invoke();
            }
            Expend(count);
            Relocate();
        }

        public void Expend(int index) {
            if (array.Expend(index)) {
                for (int i = kids.Count; i < index; i++)
                {
                    avUIArrayElement element = array.kids[i];
                    element.OnNotify.AddListener(GlobalModelUIManager.instance.OnChangeColorTo);
                    avNodeOnCirlceUI kid = element.GetComponent<avNodeOnCirlceUI>();
                    
                    kids.Add(kid);

                }
            }

            array.ActivateUpTo(index);
            
        }

        //Relocate all kids to their position
        public void Relocate() {
            int activeKids = array.activecount;
            float high = GetHighDegree(activeKids);
            for (int i = 0; i < activeKids; i++)
            {
                kids[i].RelocateOnce(90);
                //Debug.Log($"I will allocate node to {high}");
                kids[i].Relocate(high);

                high = NextDegree(high);
            }
        
        }

        

        private float NextDegree(float degree) {
            //Debug.Log($"{degree}+{degreeDiff}");
            return degree + degreeDiff;
        }

        private float GetHighDegree(int count) {
            
            float multiplier = (count-1);
            float rs = multiplier * degreeDiff/2;
            //Debug.Log($"high point is {180 - rs}");
            return 180-rs;
        }

    }
}