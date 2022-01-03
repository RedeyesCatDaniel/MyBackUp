using System;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    static class CreateEyeShaderGraph
    {
        [MenuItem("Assets/Create/Shader Graph/URP/Eye Shader Graph", priority = CoreUtils.Sections.section2 + CoreUtils.Priorities.assetsCreateShaderMenuPriority)]
        public static void CreateLitGraph()
        {
            var target = (UniversalTarget)Activator.CreateInstance(typeof(UniversalTarget));
            target.TrySetActiveSubTarget(typeof(UniversalEyeSubTarget));

            var blockDescriptors = new[]
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
                CharacterShaderBlockFields.SurfaceDescription.EyeMask,
                CharacterShaderBlockFields.SurfaceDescription.EyeSpecularOcclusion,
                CharacterShaderBlockFields.SurfaceDescription.EyeAOColor,
                CharacterShaderBlockFields.SurfaceDescription.EyeIrisNormal,
            };

            GraphUtil.CreateNewGraphWithOutputs(new[] { target }, blockDescriptors);
        }
    }
}
