using UnityEngine;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    static class CharacterShaderBlockFields
    {
        [GenerateBlocks("Universal Render Pipeline/Character Shader")]
        public struct SurfaceDescription
        {
            public static string name = "SurfaceDescription";
            
            // Eye
            public static BlockFieldDescriptor EyeSpecularOcclusion = new BlockFieldDescriptor(name, "SpecularOcclusion", "Specular Occlusion", "SURFACEDESCRIPTION_EYESPECULAROCCLUSION",
                new FloatControl(0.0f), ShaderStage.Fragment);
            public static BlockFieldDescriptor EyeMask = new BlockFieldDescriptor(name, "Mask", "Mask", "SURFACEDESCRIPTION_EYEMASK",
                new FloatControl(0.0f), ShaderStage.Fragment);
            public static BlockFieldDescriptor EyeAOColor = new BlockFieldDescriptor(name, "ColoredOcclusion", "Colored Occlusion", "SURFACEDESCRIPTION_EYECOLOREDOCCLUSION",
                new ColorControl(Color.black, true), ShaderStage.Fragment);
            public static BlockFieldDescriptor EyeIrisNormal = new BlockFieldDescriptor(name, "IrisNormal", "Iris Normal", "SURFACEDESCRIPTION_EYEIRISNORMAL",
                new NormalControl(CoordinateSpace.Tangent), ShaderStage.Fragment);
            
            // Hair
            public static BlockFieldDescriptor HairTransmittance = new BlockFieldDescriptor(SurfaceDescription.name, "Transmittance", "Transmittance", "SURFACEDESCRIPTION_HAIRTRANSMITTANCE",
                new Vector3Control(0.3f * new Vector3(1.0f, 0.65f, 0.3f)), ShaderStage.Fragment);
            public static BlockFieldDescriptor HairRimTransmissionIntensity = new BlockFieldDescriptor(SurfaceDescription.name, "RimTransmissionIntensity", "Rim Transmission Intensity", "SURFACEDESCRIPTION_HAIRRIMTRANSMISSIONINTENSITY",
                new FloatControl(0.2f), ShaderStage.Fragment);
            public static BlockFieldDescriptor HairStrandDirection = new BlockFieldDescriptor(SurfaceDescription.name, "HairStrandDirection", "Hair Strand Direction", "SURFACEDESCRIPTION_HAIRSTRANDDIRECTION",
                new Vector3Control(new Vector3(0, -1, 0)), ShaderStage.Fragment);
            public static BlockFieldDescriptor HairSpecularTint = new BlockFieldDescriptor(SurfaceDescription.name, "SpecularTint", "Specular Tint", "SURFACEDESCRIPTION_HAIRSPECULARTINT",
                new ColorControl(Color.white, false), ShaderStage.Fragment);
            public static BlockFieldDescriptor HairSpecularShift = new BlockFieldDescriptor(SurfaceDescription.name, "SpecularShift", "Specular Shift", "SURFACEDESCRIPTION_HAIRSPECULARSHIFT",
                new FloatControl(0.1f), ShaderStage.Fragment);
            public static BlockFieldDescriptor HairSecondarySpecularTint = new BlockFieldDescriptor(SurfaceDescription.name, "SecondarySpecularTint", "Secondary Specular Tint", "SURFACEDESCRIPTION_HAIRSECONDARYSPECULARTINT",
                new ColorControl(Color.gray, false), ShaderStage.Fragment);
            public static BlockFieldDescriptor HairSecondarySmoothness = new BlockFieldDescriptor(SurfaceDescription.name, "SecondarySmoothness", "Secondary Smoothness", "SURFACEDESCRIPTION_HAIRSECONDARYSMOOTHNESS",
                new FloatControl(0.5f), ShaderStage.Fragment);
            public static BlockFieldDescriptor HairSecondarySpecularShift = new BlockFieldDescriptor(SurfaceDescription.name, "SecondarySpecularShift", "Secondary Specular Shift", "SURFACEDESCRIPTION_HAIRSECONDARYSPECULARSHIFT",
                new FloatControl(-0.1f), ShaderStage.Fragment);

            // Skin
            public static BlockFieldDescriptor SkinScatteringColor = new BlockFieldDescriptor(SurfaceDescription.name, "ScatteringColor", "Scattering Color", "SURFACEDESCRIPTION_SKINSCATTERINGCOLOR",
                new ColorControl(new Color(0.76f, 0.3174f, 0.195f, 1.0f), true), ShaderStage.Fragment);
            public static BlockFieldDescriptor SkinSubsurfaceMask = new BlockFieldDescriptor(SurfaceDescription.name, "SubsurfaceMask", "Subsurface Mask", "SURFACEDESCRIPTION_SKINSUBSURFACEMASK",
                new FloatControl(1.0f), ShaderStage.Fragment);
            public static BlockFieldDescriptor SkinThickness = new BlockFieldDescriptor(SurfaceDescription.name, "Thickness", "Thickness", "SURFACEDESCRIPTION_SKINTHICKNESS",
                new FloatControl(1.0f), ShaderStage.Fragment);
            public static BlockFieldDescriptor SkinTransmission = new BlockFieldDescriptor(SurfaceDescription.name, "Transmission", "Transmission", "SURFACEDESCRIPTION_SKINTRANSMISSION",
                new ColorControl(new Color(0.76f, 0.3174f, 0.195f, 1.0f), true), ShaderStage.Fragment);
            public static BlockFieldDescriptor SkinLobeMix       = new BlockFieldDescriptor(SurfaceDescription.name, "LobeMix", "Lobe Mix", "SURFACEDESCRIPTION_SKINLOBEMIX",
                new FloatControl(0.15f), ShaderStage.Fragment);
            public static BlockFieldDescriptor SkinSmoothnessB = new BlockFieldDescriptor(SurfaceDescription.name, "SmoothnessB", "Smoothness B", "SURFACEDESCRIPTION_SKINLOBETWOSMOOTHNESS",
                new FloatControl(0.5f), ShaderStage.Fragment);

            public static CustomSlotBlockFieldDescriptor SkinNormalMap = new CustomSlotBlockFieldDescriptor(SurfaceDescription.name, "NormalMap", "Normal Map", "SURFACEDESCRIPTION_SKINNORMALMAP",
            () => { return new Texture2DInputMaterialSlot(0, "Normal Map", "NormalMap", ShaderStageCapability.Fragment, true); });
            public static CustomSlotBlockFieldDescriptor SkinNormalMapUV = new CustomSlotBlockFieldDescriptor(SurfaceDescription.name, "NormalMapUV", "Normal Map UV", "SURFACEDESCRIPTION_SKINNORMALMAPUV",
            () => { return new UVMaterialSlot(0, "Normal Map UV", "NormalMapUV", UnityEditor.ShaderGraph.Internal.UVChannel.UV0, ShaderStageCapability.Fragment, true); });

            public static CustomSlotBlockFieldDescriptor SkinLookUpTexture = new CustomSlotBlockFieldDescriptor(SurfaceDescription.name, "LookUpTexture", "Look Up Texture", "SURFACEDESCRIPTION_SKINLOOKUPTEXTURE",
            () => { return new Texture2DInputMaterialSlot(0, "Look Up Texture", "LookUpTexture", ShaderStageCapability.Fragment, true); });

        }
    }
}
