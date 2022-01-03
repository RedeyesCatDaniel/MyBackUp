using System;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    static class CreateUniversalHairShaderGraph
    {
        [MenuItem("Assets/Create/Shader Graph/URP/Hair Shader Graph", priority = CoreUtils.Sections.section2 + CoreUtils.Priorities.assetsCreateShaderMenuPriority)]
        public static void CreateUniversalHairGraph()
        {
            var target = (UniversalTarget)Activator.CreateInstance(typeof(UniversalTarget));
            target.TrySetActiveSubTarget(typeof(UniversalHairSubTarget));

            var blockDescriptors = new [] 
            { 
                BlockFields.VertexDescription.Position,
                BlockFields.VertexDescription.Normal,
                BlockFields.VertexDescription.Tangent,                
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.NormalTS,
                CharacterShaderBlockFields.SurfaceDescription.HairStrandDirection,
                CharacterShaderBlockFields.SurfaceDescription.HairTransmittance,
                CharacterShaderBlockFields.SurfaceDescription.HairRimTransmissionIntensity,
                BlockFields.SurfaceDescription.Smoothness,
                BlockFields.SurfaceDescription.Occlusion,
                BlockFields.SurfaceDescription.Alpha,
                CharacterShaderBlockFields.SurfaceDescription.HairSpecularTint,
                CharacterShaderBlockFields.SurfaceDescription.HairSpecularShift,
                CharacterShaderBlockFields.SurfaceDescription.HairSecondarySpecularTint,
                CharacterShaderBlockFields.SurfaceDescription.HairSecondarySmoothness,
                CharacterShaderBlockFields.SurfaceDescription.HairSecondarySpecularShift,                
            };

            GraphUtil.CreateNewGraphWithOutputs(new [] {target}, blockDescriptors);
        }
    }
}
