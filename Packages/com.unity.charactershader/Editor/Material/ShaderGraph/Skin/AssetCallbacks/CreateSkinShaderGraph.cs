using System;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    static class CreateSkinShaderGraph
    {
        [MenuItem("Assets/Create/Shader Graph/URP/Skin Shader Graph", priority = CoreUtils.Sections.section2 + CoreUtils.Priorities.assetsCreateShaderMenuPriority)]
        public static void CreateSkinGraph()
        {
            var target = (UniversalTarget)Activator.CreateInstance(typeof(UniversalTarget));
            target.TrySetActiveSubTarget(typeof(UniversalSkinSubTarget));

            var blockDescriptors = new [] 
            { 
                BlockFields.VertexDescription.Position,
                BlockFields.VertexDescription.Normal,
                BlockFields.VertexDescription.Tangent,
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.NormalTS, 
                BlockFields.SurfaceDescription.Metallic,
                BlockFields.SurfaceDescription.Smoothness, 
                BlockFields.SurfaceDescription.Emission,
                BlockFields.SurfaceDescription.Occlusion,
                CharacterShaderBlockFields.SurfaceDescription.SkinScatteringColor,
                CharacterShaderBlockFields.SurfaceDescription.SkinSubsurfaceMask,
                CharacterShaderBlockFields.SurfaceDescription.SkinThickness,
                CharacterShaderBlockFields.SurfaceDescription.SkinTransmission,                
                CharacterShaderBlockFields.SurfaceDescription.SkinLookUpTexture,
            };

            GraphUtil.CreateNewGraphWithOutputs(new [] {target}, blockDescriptors);
        }
    }
}
