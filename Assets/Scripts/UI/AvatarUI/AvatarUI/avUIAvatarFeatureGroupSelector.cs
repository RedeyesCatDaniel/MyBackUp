using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    // This is the class designed to select feature group like face nose and hair
    // It will be attactched to a component that manage a list of feature group
    public class avUIAvatarFeatureGroupSelector : MonoBehaviour
    {
        public List<avUIAvatarFeatureGroupElementManager> kids = new List<avUIAvatarFeatureGroupElementManager>();
        public int selected;
        public avGridManager gm;


        public void Init() {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform kid = transform.GetChild(i);
                if (kid.TryGetComponent<avUIAvatarFeatureGroupElementManager>(out avUIAvatarFeatureGroupElementManager man))
                {
                    kids.Add(man);
                    man.OnSelected.AddListener(OnKidSelected);
                    man.index = i;
                }
            }
        }

        public void Init(avGridManager gm) {
            List<Sprite> icons = GlobalModelUIManager.instance.GetGroupICons();
            int row = icons.Count;

            gm.ConstructPage(row,0);
            for (int i = 0; i < row; i++)
            {
                avRowManager item = gm.GetRows()[i];
                if (item.TryGetComponent<avUIAvatarFeatureGroupElementManager>(out avUIAvatarFeatureGroupElementManager man))
                {
                    kids.Add(man);
                    man.OnSelected.AddListener(OnKidSelected);
                    man.index = i;
                    man.OnSelected.AddListener(GlobalModelUIManager.instance.OnGroupSelected);
                    man.imageMan.ChangeImageTo(icons[i]);
                }
            }
            
        }


        private void Start()
        {
            Init(gm);
            SelectUI(selected);
            ResetTarget(kids.Count-1);
            GlobalModelUIManager.instance.OnGroupSelected(selected);
        }

        public void OnKidSelected(int index) {
            if (index != selected) {
                OnUnSelectedUI(selected);
                selected = index;
                UpdateChoiceGroup(index);
                SelectUI(index);
            }
        }


        public void UpdateChoiceGroup(int index) { 
        
        }

        public void SelectUI(int index) {
            ResetTarget(index - 1);
            ResetTarget(index);
            ConnectTarget(selected);
        }

        private void OnUnSelectedUI(int index)
        {
            int last = index - 1;
           // ResetTarget(last);
            ResetTarget(index);
            Extend(last);
            Extend(index);
        }

        private void ResetTarget(int index) {
            if (TryGetKid(index,out avUIAvatarFeatureGroupElementManager kid)) {
                kid.Shrink();
            }
        }

        private void ConnectTarget(int index) {
           // Debug.Log($"I asked {index} connected to right");
            if (TryGetKid(index, out avUIAvatarFeatureGroupElementManager kid))
            {
                kid.ConnectToRight();
            }
        }

        public void Extend(int index) {
            if (TryGetKid(index, out avUIAvatarFeatureGroupElementManager kid) && index != kids.Count-1) {
                kid.ExtendDown();
            }
        }

       

        private bool TryGetKid(int index,out avUIAvatarFeatureGroupElementManager kid) {
            if (index< kids.Count  && index >= 0)
            {
                kid = kids[index];
                return true;
            }
            kid = default;
            return false;
        }
    }
}