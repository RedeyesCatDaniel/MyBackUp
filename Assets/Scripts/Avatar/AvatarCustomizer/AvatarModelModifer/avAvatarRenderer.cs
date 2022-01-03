using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avAvatarRenderer : MonoBehaviour
    {

        // public List<FMesh> stitchers;
        public avBearMeshStitcher stitcher;

        //Clothing will be put on this point
        public avAttachmentPoint attachPoint;

        //This Component is controlling materials
        public avMaterialManager amm;
        
       // public avBearDictionary<FeatureGroup, avBearSkinnedMeshModifier> avBearSkinnedMeshModifiers;
        public Dictionary<string, avBearSkinnedMeshModifier> ModifierByName = new Dictionary<string, avBearSkinnedMeshModifier>();
        public List<avBearSkinnedMeshModifier> modifiers;
        private void Awake()
        {
            // avBearSkinnedMeshModifiers.Init();
            // avGlobalModifierManager.Modify(this);
            // Debug.Log("I am initialized");

            foreach (var item in modifiers)
            {
                ModifierByName[item.name] = item;
            }
        }

        [ContextMenu(itemName: "PrintModifiers")]
        public void PrintModifiers() {
            //Debug.Log("Hello I am printing?");
            foreach (var item in avGlobalModifierManager.modificationsOnCharacter.Keys)
            {
                Debug.Log($"{item.type}:{avGlobalModifierManager.modificationsOnCharacter[item]}");
            }
        }

        public bool TryGetAttachPoint(out avAttachmentPoint trans)
        {
            trans = attachPoint;
            if (attachPoint!=null) {
                return true;
            }

            return false;
        }

        public void OnSMRMade(string code) {
            if (amm == null) return;
            if (amm.TryGetAvLocalMaterialManager(code, out avLocalMaterialManager manager)) {
                manager.Init();
                FeatureGroup group = manager.group;
                if (avGlobalModifierManager.colorModifiers.TryGetValue(group, out Color color )) {
                    amm.ChangeColor(group,color);
                }
                //gameObject.SetActive(true);
                
            }
        }

        public void ChangeColor(FeatureGroup code,Color color) {
            amm.ChangeColor(code,color);
            
        }

        public void ChangeTexture(FeatureGroup code, string name, Texture texture) {
            amm.ChangeTexture(code,name,texture);
        }

        

        



    }

    



}