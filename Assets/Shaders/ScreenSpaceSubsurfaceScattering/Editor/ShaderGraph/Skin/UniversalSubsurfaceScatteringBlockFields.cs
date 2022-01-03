using UnityEngine;
using UnityEditor.ShaderGraph.Internal;
//using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEditor.ShaderGraph;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    static class UniversalSubsurfaceScatteringBlockFields
    {
        [GenerateBlocks("Universal Render Pipeline/Unity Subsurface Scattering")]
        public struct SurfaceDescription
        {
            public static string name = "SurfaceDescription";

            public static CustomSlotBlockFieldDescriptor DiffusionProfileHash = new CustomSlotBlockFieldDescriptor(name, "DiffusionProfileHashValue", "Diffusion Profile", "SURFACEDESCRIPTION_DIFFUSIONPROFILEHASH",
                () => { return new DiffusionProfileInputMaterialSlot(0, "Diffusion Profile", "DiffusionProfileHashValue", ShaderStageCapability.Fragment); });
            public static BlockFieldDescriptor SubsurfaceMask = new BlockFieldDescriptor(name, "SubsurfaceMask", "SURFACEDESCRIPTION_SUBSURFACEMASK",
                new FloatControl(1.0f), ShaderStage.Fragment);
            public static BlockFieldDescriptor Thickness = new BlockFieldDescriptor(name, "Thickness", "SURFACEDESCRIPTION_THICKNESS",
                new FloatControl(0.1f), ShaderStage.Fragment);
            public static BlockFieldDescriptor LobeInterpolation = new BlockFieldDescriptor(name, "LobeInterpolation", "SURFACEDESCRIPTION_LOBEINTERPOLATION",
                new FloatControl(0.5f), ShaderStage.Fragment);
            public static BlockFieldDescriptor LobeDerivation = new BlockFieldDescriptor(name, "LobeDerivation", "SURFACEDESCRIPTION_DERIVATION",
                new FloatControl(1.0f), ShaderStage.Fragment);

        }
    }


}
