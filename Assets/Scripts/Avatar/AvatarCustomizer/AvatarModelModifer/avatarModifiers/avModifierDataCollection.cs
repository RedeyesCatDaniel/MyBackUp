using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "Data/avModifierDataCollection")]
    public class avModifierDataCollection : ScriptableObject
    {
        public avBlendShapeModifierData blendShapeModifiers;
        public avColorModifierData colorModifiers;
        public avMeshModifierData meshModifiers;
        public avMeshModifierData lowMeshModifiers;
        public avSkinnedMeshRendererModifierData smrModifiers;
        public avBackgroundModifierData backgroundModifiers;
        public avTextureModifierData textureModifier;

        public void InjectData(Dictionary<string,IAvatarModifier> modifiers) {
            blendShapeModifiers.InjectData(modifiers);
            colorModifiers.InjectData(modifiers);
            meshModifiers.InjectData(modifiers);
            smrModifiers.InjectData(modifiers);
            backgroundModifiers.InjectData(modifiers);
            textureModifier.InjectData(modifiers);
        }

        public void InjectLowPolyData(Dictionary<string, IAvatarModifier> modifiers) {
            lowMeshModifiers.InjectData(modifiers);
        }

        public void Make() {
            this.GenerateBlendShapeData();
            
        }
    }
}