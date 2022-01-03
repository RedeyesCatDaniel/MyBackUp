using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    //you should attach this scripts on the parent of all skinned mesh renderers
    public class avMaterialManager : MonoBehaviour
    {
        public avLocalMaterialManager[] managers;
        private Dictionary<string, avLocalMaterialManager> dic = new Dictionary<string, avLocalMaterialManager>();
        public avBearDictionary<FeatureGroup, MAttributeIndex> f2mMap;
        private void Awake()
        {
            f2mMap.Init();
            foreach (var item in managers)
            {
                dic[item.name] = item;
                item.RefreshMat();
            }
        }
        public void ChangeColor(FeatureGroup group, Color color) {

            Debug.Log($"I changed {group.type}'s color to {color}");
            if (f2mMap.TryGetValue(group, out MAttributeIndex value)) {
                if (dic.TryGetValue(value.localManagerName, out avLocalMaterialManager amm))
                {
                    Debug.Log("I find the manager");
                    if (value.indexes.Length == 0)
                    {
                        //When the list is empty then it will modify all material 
                        amm.ModifyAll(value.attributename, color);
                    }
                    else {
                        //When there are spcific materials to modify
                        foreach (var index in value.indexes)
                        {
                            if (amm.TryGetMaterial(index, out Material mat))
                            {
                                if (mat == null)
                                {
                                    return;
                                }

                                if (mat.HasProperty(value.attributename))
                                {
                                    mat.SetColor(value.attributename, color);
                                }
                                else
                                {
                                    Debug.Log($"But I cannot find {value.attributename} in {mat.name}");
                                }
                            }

                        }
                    }

                    
                   
                    
                }
            }
            
        }

        public bool TryGetAvLocalMaterialManager(string code, out avLocalMaterialManager manager) {
            return dic.TryGetValue(code, out manager);
        }

        internal void ChangeTexture(FeatureGroup code, string name, Texture texture)
        {
            Debug.Log($"I changed {code.type}'s texure to {texture}");
            if (f2mMap.TryGetValue(code, out MAttributeIndex value))
            {
                if (dic.TryGetValue(value.localManagerName, out avLocalMaterialManager amm))
                {
                    Debug.Log("I find the manager");
                    if (value.indexes.Length == 0)
                    {
                        //When the list is empty then it will modify all material 
                        amm.ModifyAll(name, texture);
                    }
                    else {
                        //When there are spcific materials to modify
                        foreach (var index in value.indexes)
                        {
                            if (amm.TryGetMaterial(index, out Material mat))
                            {
                                if (mat == null)
                                {
                                    return;
                                }

                                if (mat.HasProperty(name))
                                {
                                    mat.SetTexture(name, texture);
                                }
                                else
                                {
                                    Debug.Log($"But I cannot find {name} in {mat.name}");
                                }
                            }

                        }
                    }
                  
                }
            }
        }
    }

    [System.Serializable]
    public struct MAttributeIndex {
        public string localManagerName;
        public int[] indexes;
        public string attributename;
    }
}