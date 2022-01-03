Shader "Hidden/DrawTransmittanceGraph"
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
            #pragma target 4.5
            #pragma only_renderers d3d11 ps4 metal // TEMP: until we go further in dev

            #pragma vertex Vert
            #pragma fragment Frag

            //-------------------------------------------------------------------------------------
            // Include
            //-------------------------------------------------------------------------------------

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            //-------------------------------------------------------------------------------------
            // Inputs & outputs
            //-------------------------------------------------------------------------------------

            float4 _ShapeParam, _TransmissionTint, _ThicknessRemap;

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

           
            // Computes the fraction of light passing through the object.
            // Evaluate Int{0, inf}{2 * Pi * r * R(sqrt(r^2 + d^2))}, where R is the diffusion profile.
            // Ref: Approximate Reflectance Profiles for Efficient Subsurface Scattering by Pixar (BSSRDF only).
            float3 ComputeTransmittance(float3 S, float3 volumeAlbedo, float thickness, float radiusScale)
            {
                // Thickness and SSS radius are decoupled for artists.
                // In theory, we should modify the thickness by the inverse of the radius scale of the profile.
                // thickness /= radiusScale;

                float3 expOneThird = exp(((-1.0 / 3.0) * thickness) * S);

                return 0.25 * (expOneThird + 3 * expOneThird * expOneThird * expOneThird) * volumeAlbedo;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);

                output.texcoord = input.texcoord;

                output.vertex = vertexInput.positionCS;

                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float  d = (_ThicknessRemap.x + input.texcoord.x * (_ThicknessRemap.y - _ThicknessRemap.x));
                float3 T = ComputeTransmittance(_ShapeParam.rgb, float3(1, 1, 1), d, 1);

                // Apply gamma for visualization only. Do not apply gamma to the color.
                return float4(pow(T, 1.0 / 3) * _TransmissionTint.rgb, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}