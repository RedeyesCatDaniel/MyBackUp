using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    //this class is for updating the UI to display feature sprite
    public class avUIAvatarSingleFeatureSelector : MonoBehaviour
    {
        public avPageManager pageMan;
        public UIImageManager[] imageManagers;

        

        private void Start()
        {
            int size = pageMan.gridman.defaultGridData.GetPageSize();
            imageManagers = new UIImageManager[size];
            for (int i = 0; i < size; i++)
            {
                //Get UI Image Manager So I could Change the sprite on each grid
                avGridElementManager gem = pageMan.gridman.allElems[i];
                if (gem.TryGetComponent<UIImageManager>(out UIImageManager uimageManager))
                {
                    imageManagers[i] = uimageManager;

                }

                //Add Event on Each Grid So that it will notify a Global manager when it is clicked
                gem.OnClickEvent.AddListener(GlobalModelUIManager.instance.OnSingleFeatureSelected);
            }

            GlobalModelUIManager.instance.DOnGroupSelected.AddListener(Init);
            Init(0);
           
        }


        public void Init(int featureGroup)
        {
            

            int count = GlobalModelUIManager.instance.GetFeatureGroupCount(featureGroup);
            pageMan.Init(count);
            //imageManagers = new UIImageManager[pageMan.pageCount];
            
        }

        public void ChangeSprite(int index) {
           
            Sprite sprite = GlobalModelUIManager.instance.GetFeatureSprite(index);
           
            imageManagers[index%pageMan.pageSize].ChangeImageTo(sprite);
        }

        
    }
}
