using UnityEditor.ShaderGraph;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    static class SubsurfaceScatteringStructs
    {
        public struct SubsurfaceScatteringVaryings
        {
            public static string name = "Varyings";
            public static FieldDescriptor ShapeParams = new FieldDescriptor(Varyings.name, "ShapeParams", "", ShaderValueType.Float2,
                preprocessor: "defined(LIGHTMAP_ON)", subscriptOptions: StructFieldOptions.Optional);
        }

        public static StructDescriptor Varyings = new StructDescriptor()
        {
            name = "Varyings",
            packFields = true,
            fields = new FieldDescriptor[]
            {
                StructFields.Varyings.positionCS,
                StructFields.Varyings.positionWS,
                StructFields.Varyings.normalWS,
                StructFields.Varyings.tangentWS,
                StructFields.Varyings.texCoord0,
                StructFields.Varyings.texCoord1,
                StructFields.Varyings.texCoord2,
                StructFields.Varyings.texCoord3,
                StructFields.Varyings.color,
                StructFields.Varyings.viewDirectionWS,
                StructFields.Varyings.screenPosition,
                UniversalStructFields.Varyings.staticLightmapUV,
                UniversalStructFields.Varyings.dynamicLightmapUV,
                UniversalStructFields.Varyings.sh,
                UniversalStructFields.Varyings.fogFactorAndVertexLight,
                UniversalStructFields.Varyings.shadowCoord,
                StructFields.Varyings.instanceID,
                UniversalStructFields.Varyings.stereoTargetEyeIndexAsBlendIdx0,
                UniversalStructFields.Varyings.stereoTargetEyeIndexAsRTArrayIdx,
                StructFields.Varyings.cullFace,
            }
        };
    }
}