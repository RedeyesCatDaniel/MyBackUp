using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "Toolkit/avImageToolkit")]
    public class avImageToolkit:ScriptableObject 
    {
        public int width;
        public int height;
        public Texture2D defaultTexture;
       

        public Texture2D GetTexture2D(Camera cam) {
            RenderTexture temp = new RenderTexture(width, height, 24);
           
            cam.targetTexture = temp;
            cam.Render();


            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            RenderTexture.active = temp;
            
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;


            return tex;
        }


        public Texture2D GetTexture2D(byte[] data) {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.LoadImage(data);
            return tex;
        }

        public void GetPhoto(string userid,System.Action<Texture2D> textureAction)
        {
            textureAction.Invoke(defaultTexture);
            memService.PullData<byte[]>(userid,avAvatarKeys.Avatar_Photo, (x)=> {
                Texture2D rs = GetTexture2D(x);
                textureAction.Invoke(rs);
            });

        }

        public void UpdatePhoto(string userid,Texture2D photo,System.Action onFinish) {
            byte[] data = photo.EncodeToPNG();
            memService.PushData<byte[]>(userid, avAvatarKeys.Avatar_Photo, data, (x) => {
                onFinish?.Invoke();
                Debug.Log($"finished writing {data.Length} byte data");
            });
        }

        


        

        






    }
}
