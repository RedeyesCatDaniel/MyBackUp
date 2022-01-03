using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    public class avImagePrinter : MonoBehaviour
    {
        public Image img;
        public avImageToolkit kit;
        public void PrintIMG() {
            kit.GetPhoto(UserInfo.Instance.UserId,(tex)=> {
                img.material.mainTexture = tex;
               // Debug.Log("Finish Reading");
            });
        }
    }
}
