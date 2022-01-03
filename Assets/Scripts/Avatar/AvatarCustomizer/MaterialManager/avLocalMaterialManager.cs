using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avLocalMaterialManager : MonoBehaviour
    {
        public List<SkinnedMeshRenderer> myRenderers = new List<SkinnedMeshRenderer>();
        //public string ColorProperty;
        public FeatureGroup group;
        public Material[] localMaterials;
        
        private List<Material> mats;


 

        public List<Material> Mats {
            get{
                if (mats == null) {
                    mats = new List<Material>();
                    for (int i = 0; i < localMaterials.Length; i++)
                    {
                        Material mat = new Material(localMaterials[i]);
                        Mats.Add(mat);

                    }
                    
                }
                return mats;
            }

            set {
                mats = value;
            }
        }

        public void Init()
        {
            RebindRenderers();
            RefreshMat();
          //  RefreshColor();
        }
        public void RebindRenderers() {
            myRenderers = new List<SkinnedMeshRenderer>();
            foreach (Transform item in transform)
            {
                if (item.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer meshRenderer)) {
                    if(meshRenderer != null)
                        myRenderers.Add(meshRenderer);
                }
            }
        }

        //public void RefreshColor() {
        //    if (avGlobalModifierManager.colorModifiers.TryGetValue(group, out Color color)) { 
                
        //    }
        //}

        public void RefreshMat() {

            // mats = new Material[localMaterials.Length];

            if (localMaterials.Length == 0)
            {
                Mats = new List<Material>();
                foreach (var item in myRenderers)
                {
                    for (int i = 0; i < item.materials.Length; i++)
                    {
                        Material mat = item.materials[i];
                        //  Material newMat = new Material(mat);
                        Mats.Add(mat);
                    }
                }
            }
            else
            {
                foreach (var item in myRenderers)
                {
                    item.materials = Mats.ToArray();

                }

            }




        }

        public bool TryGetMaterial(int index,out Material material) {
            if (index >= 0 && index < Mats.Count)
            {
                material = Mats[index];
                return true;
            }
            else {
                material = null;
                return false;
            }
        }

        //public void Modify(Color newColor) {
        //    foreach (var mat in mats)
        //    {
        //        if (mat.HasColor(ColorProperty))
        //            mat.SetColor(ColorProperty, newColor);
        //    }
            
        //}

        public void Modify(string name,Color newColor)
        {
            foreach (var mat in mats)
            {
                if (mat.HasColor(name))
                    mat.SetColor(name, newColor);
            }

        }

        public void Modify(string name, Texture text) {
            foreach (var mat in mats)
            {
                if (mat.HasProperty(name))
                {
                    mat.SetTexture(name, text);
                }
            }
           
           
        }

        public void ModifyAll(string name, Color newColor) {
            foreach (var mat in Mats)
            {
                if (mat.HasColor(name))
                    mat.SetColor(name, newColor);
            }
        }

        public void ModifyAll(string name, Texture text)
        {
            foreach (var mat in Mats)
            {
                if (mat.HasProperty(name))
                    mat.SetTexture(name, text);
            }
        }
    }
}