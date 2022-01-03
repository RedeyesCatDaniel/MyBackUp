using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    public class MapMoveView : AbstractController
    {
        public TMP_InputField search;
        public Button close, set;
        public TextMeshProUGUI location;
        public Transform item_p;
        public List<MapMoveitemview> mapMoveitemviews;
        public TextMeshProUGUI title;
        private MapMoveitem clickitem;
        public MapMoveitem Clickitem
        {
            get { return clickitem; }
            set
            {
                if (clickitem!=null)
                {
                    clickitem.NoSelect();
                    clickitem = value;
                    clickitem.Select();
                   
                }
                else
                {
                    clickitem = value;
                    clickitem.Select();
                } 
                FavoriteWorkSpaceList.Instance.index=MapMove.Instance.itemlist.IndexOf(clickitem)+1;
            }
        }
    }


}