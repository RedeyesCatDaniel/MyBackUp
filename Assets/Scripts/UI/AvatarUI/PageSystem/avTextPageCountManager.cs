using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    public class TextPageCountManager : MonoBehaviour
    {
        public Text myText;
        public avPageManager pman;

        public void UpdatePage() {
            myText.text = (pman.currentPage+1) + "/" + pman.pageCount;
        }
        
    }
}