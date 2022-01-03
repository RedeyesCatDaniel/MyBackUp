using System;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    static class CreateSubsurfaceScatteringShaderGraph
    {
        [MenuItem("Assets/Create/Shader/Universal Render Pipeline/Subsurface Scattering Shader Graph", priority = CoreUtils.Priorities.assetsCreateShaderMenuPriority)]
        public static void CreateLitGraph()
        {
            var target = (UniversalTarget)Activator.CreateInstance(typeof(UniversalTarget));
            target.TrySetActiveSubTarget(typeof(SubsurfaceScatteringShaderGraphTarget));

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
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.DiffusionProfileHash,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.SubsurfaceMask,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.Thickness,
            };

            GraphUtil.CreateNewGraphWithOutputs(new[] { target }, blockDescriptors);
        }
    }
}
