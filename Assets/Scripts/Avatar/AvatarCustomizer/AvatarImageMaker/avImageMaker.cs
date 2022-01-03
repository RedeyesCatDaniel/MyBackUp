using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice { 
    
    public class avImageMaker : MonoBehaviour
    {
        private static Texture myPhoto;
        public avImageToolkit kit;
        public Camera cam;

        private void Awake()
        {
            if(myPhoto == null)
                myPhoto = kit.defaultTexture;
        }
        public void SnapShot()
        {
            Texture2D photo = kit.GetTexture2D(cam);
            myPhoto = photo;
            kit.UpdatePhoto(UserInfo.Instance.UserId, photo,()=> {});
        }

        public static bool TryGetPhoto(out Texture rs) {
            rs = avImageMaker.myPhoto;
            if (myPhoto != null) {
                return true; 
            }
            return false;
        }

        
    }
}