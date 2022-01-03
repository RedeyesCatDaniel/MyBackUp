Shader "Hidden/DrawSssProfile"
{
    SubShader
    {
        Pass
        {
            Cull   Off
            ZTest  Off
            ZWrite Off
            Blend  Off

            HLSLPROGRAM
            #pragma target 4.0
            #pragma only_renderers d3d11 ps4 metal // TEMP: until we go further in dev

            #pragma vertex Vert
            #pragma fragment Frag

            #pragma multi_compile SSS_MODEL_BASIC SSS_MODEL_DISNEY

            //-------------------------------------------------------------------------------------
            // Include
            //-------------------------------------------------------------------------------------

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            //-------------------------------------------------------------------------------------
            // Inputs & outputs
            //-------------------------------------------------------------------------------------
        #define SSS_BASIC_DISTANCE_SCALE (3)

        #ifdef SSS_MODEL_DISNEY
            float4 _ShapeParam; float _MaxRadius; // See 'SubsurfaceScatteringProfile'
        #else
            float4 _StdDev1, _StdDev2;
            float _LerpWeight, _MaxRadius; // See 'SubsurfaceScatteringParameters'
        #endif

            //-------------------------------------------------------------------------------------
            // Implementation
            //-------------------------------------------------------------------------------------

            struct Attributes
            {
                float3 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);

                output.texcoord = input.texcoord;

                output.vertex = vertexInput.positionCS;

                return output;
            }

            float3 EvalBurleyDiffusionProfile(float r, float3 S)
            {
                float3 exp_13 = exp2(((LOG2_E * (-1.0 / 3.0)) * r) * S); // Exp[-S * r / 3]
                float3 expSum = exp_13 * (1 + exp_13 * exp_13);        // Exp[-S * r / 3] + Exp[-S * r]

                return (S * rcp(8 * PI)) * expSum; // S / (8 * Pi) * (Exp[-S * r / 3] + Exp[-S * r])
            }

            float4 Frag(Varyings input) : SV_Target
            {
            #ifdef SSS_MODEL_DISNEY
                // Profile display does not use premultiplied S.
                //float  r = min(min(_ShapeParam.x, _ShapeParam.y), _ShapeParam.z);
                float  r = _MaxRadius;
                r =  r * 0.5 * length(input.texcoord - 0.5) * 10; // (-0.25 * R, 0.25 * R)
                float3 S = _ShapeParam.rgb;
                float3 M;

                // Gamma in previews is weird...
                S = S * S;
                M = EvalBurleyDiffusionProfile(r, S) / r; // Divide by 'r' since we are not integrating in polar coords
                return float4(sqrt(M), 1);
            #else
                float  r    = (2 * length(input.texcoord - 0.5)) * _MaxRadius * SSS_BASIC_DISTANCE_SCALE;
                float3 var1 = _StdDev1.rgb * _StdDev1.rgb;
                float3 var2 = _StdDev2.rgb * _StdDev2.rgb;

                // Evaluate the linear combination of two 2D Gaussians instead of
                // product of the linear combination of two normalized 1D Gaussians
                // since we do not want to bother artists with the lack of radial symmetry.
                float3 magnitude = lerp(exp(-r * r / (2 * var1)) / (TWO_PI * var1),
                                        exp(-r * r / (2 * var2)) / (TWO_PI * var2), _LerpWeight);

                return float4(magnitude, 1);
            #endif
            }
            ENDHLSL
        }
    }
    Fallback Off
}