using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LGUVirtualOffice
{
    public class ImageSyncTester : MonoBehaviour
    {
        public Camera cam;
        public avImageToolkit kit;
        public Image img;

        public void UpdateImg() {
            if (avImageMaker.TryGetPhoto(out Texture tex))
            {
                Debug.Log("found to find img");
                img.material.mainTexture = tex;
            }
            else {
                Debug.Log("failed to find img");
            }
        }

    }
}