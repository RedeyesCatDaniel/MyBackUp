#ifndef UNIVERSAL_EYEUTILITY_INCLUDED
#define UNIVERSAL_EYEUTILITY_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

//#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#include "./ShaderLibrary/EyeInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"


#if (_DETAIL_MULX2 || _DETAIL_MUL || _DETAIL_ADD || _DETAIL_LERP)
#define _DETAIL 1
#endif

#define UNITY_PI            3.14159265359f
#define UNITY_INV_PI        0.31830988618f

#define smp SamplerState_Linear_Repeat
SAMPLER(smp);

half4 _Color;
half _Glossiness;
half4 _SpecularColor;

half _GlossMapScale;
float4 _SpecGlossMap_ST;
float4 _BumpMap_ST;
float4 _OcclusionMap_ST;


TEXTURE2D(_ThicknessMap);
float4 _ThicknessMap_ST;
half _Thickness;

//------------------------------------------------------------eye-------------------------------------------------------------------------------

TEXTURE2D (_EyeMask);

TEXTURE2D  (_EyeScleraAlbedo);

TEXTURE2D   (_EyeScleraSpecularOcclusion);

TEXTURE2D   (_EyeScleraBumpMap);




//------------------------------------------------------------Encode Lighting--------------------------------------------------------------------

float  EncodeLightChannel(float2 f) {
    return (dot(round((f) * 255), float2(256, 1.0)));
}

float  EncodeFullLight(float3 f) {
    uint lo = UnpackInt(f.r, 4);
    uint mi = UnpackInt(f.g, 4);
    uint hi = UnpackInt(f.b, 4);
    uint cb = (hi << 8) + (mi << 4) + lo;
    return PackShort(cb);
}

#define OVERFLOW_COMPRESSION    0.25
#define OVERFLOW_THRESHOLD      0.5
half4 EncodeSplitLighting(float3 diffuse, float3 specular) {
    specular = min(float3(0.9999, 0.9999, 0.9999), specular);
    diffuse = min(float3(0.9999, 0.9999, 0.9999), diffuse);

    //Written to 16161616 buffer during forward opaque pass.
    //(RGB) - encoded specular and diffuse, 8bits each
    //(A)   - full diffuse, 4bits for each channel channel
    return half4(EncodeLightChannel(float2(specular.r, diffuse.r)),
        EncodeLightChannel(float2(specular.g, diffuse.g)),
        EncodeLightChannel(float2(specular.b, diffuse.b)),
        EncodeFullLight(OVERFLOW_COMPRESSION * diffuse));
}



void SplitGlobalIllumination(BRDFData brdfData, BRDFData brdfDataClearCoat, float clearCoatMask,
    half3 bakedGI, half occlusion,
    half3 normalWS, half3 viewDirectionWS, out half3 diffuseGI, out half3 specularGI)
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half NoV = saturate(dot(normalWS, viewDirectionWS));
    half fresnelTerm = Pow4(1.0 - NoV);

    half3 indirectDiffuse = bakedGI * occlusion;
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness, occlusion);

    //half3 color = EnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);

    //indirectDiffuse *= brdfData.diffuse;
    indirectSpecular *= EnvironmentBRDFSpecular(brdfData, fresnelTerm) * _EyeReflectionIntensity;

    diffuseGI = indirectDiffuse ;
    specularGI = indirectSpecular;
}

void SplitLightingPhysicallyBased(BRDFData brdfData, BRDFData brdfDataClearCoat, Light light,
    half3 normalWS, half3 viewDirectionWS, half clearCoatMask, bool specularHighlightsOff, out half3 directDiffuse, out half3 directSpecular)
{
    half3 lightColor = light.color;
    half3 lightDirectionWS = light.direction;
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    half NdotL = saturate(dot(normalWS, lightDirectionWS));
    half3 radiance = lightColor * (lightAttenuation * NdotL);

    half3 brdf;
#ifndef _SPECULARHIGHLIGHTS_OFF
    [branch] if (!specularHighlightsOff)
    {
        brdf = brdfData.specular * DirectBRDFSpecular(brdfData, normalWS, lightDirectionWS, viewDirectionWS);

#if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
        // Clear coat evaluates the specular a second timw and has some common terms with the base specular.
        // We rely on the compiler to merge these and compute them only once.
        half brdfCoat = kDielectricSpec.r * DirectBRDFSpecular(brdfDataClearCoat, normalWS, lightDirectionWS, viewDirectionWS);

        // Mix clear coat and base layer using khronos glTF recommended formula
        // https://github.com/KhronosGroup/glTF/blob/master/extensions/2.0/Khronos/KHR_materials_clearcoat/README.md
        // Use NoV for direct too instead of LoH as an optimization (NoV is light invariant).
        half NoV = saturate(dot(normalWS, viewDirectionWS));
        // Use slightly simpler fresnelTerm (Pow4 vs Pow5) as a small optimization.
        // It is matching fresnel used in the GI/Env, so should produce a consistent clear coat blend (env vs. direct)
        half coatFresnel = kDielectricSpec.x + kDielectricSpec.a * Pow4(1.0 - NoV);

        brdf = brdf * (1.0 - clearCoatMask * coatFresnel) + brdfCoat * clearCoatMask;
#endif // _CLEARCOAT
    }
#endif // _SPECULARHIGHLIGHTS_OFF
    directDiffuse = brdfData.diffuse * radiance;
    directSpecular = brdf * radiance ;
    //return brdf * radiance;
}


float2 FixedTexcoords(float2 texcoords) {
    //Fixed texture coordinates
    float3 headRightOS = mul(unity_WorldToObject, float4(_EyeFaceVectorRight, 0)).xyz;
    float3 headUpOS = mul(unity_WorldToObject, float4(_EyeFaceVectorUp, 0)).xyz;
    float UpDotRight = dot(headRightOS, float3(0, 1, 0));

    float ac = cos(-UpDotRight * UNITY_PI * _EyeFixedTexCoordZ);
    float as = sin(-UpDotRight * UNITY_PI * _EyeFixedTexCoordZ);
    float2x2 rot = { ac, -as,
                     as,  ac };
    texcoords.xy = mul(rot, texcoords.xy - 0.5) + 0.5;

    float3 headDirectionOS = mul(unity_WorldToObject, float4(_EyeFaceVector, 0)).xyz;
    float FaceDotX = dot(headDirectionOS, float3(1, 0, 0));
    float FaceDotY = dot(headDirectionOS, float3(0, 1, 0));
    texcoords.xy += float2(FaceDotX * _EyeFixedTexCoordX, FaceDotY * _EyeFixedTexCoordY);

    return texcoords;
}

half3 ColorBleedAO(half occlusion) {
    return pow(abs(occlusion), 1.0 - _EyeColorBleed.rgb);
}

void ScleraLayerBase(InputData i, BRDFData brdfData, half3 scleraColor, half3 scleraNormal, half3 indirectGI, Light light, float2 occlusionCoords, inout half3 diffuse, inout half3 specular) {

    half shiftAmount = dot(scleraNormal, light.direction);
    half3 normal = shiftAmount < 0.0f ? scleraNormal + light.direction * (-shiftAmount + 1e-5f) : scleraNormal;

    half3 lightColor = light.color * light.shadowAttenuation;
    half3 lightDirectionWS = light.direction;
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    half NdotL = saturate(dot(normal, lightDirectionWS));
    half3 radiance = lightColor * (lightAttenuation * NdotL);

    //Diffuse
    float n = 1;
    float diffuseTerm = pow(saturate((dot(normal, lightDirectionWS) + _EyeScleraWrap) / (1.0f + _EyeScleraWrap)), n) * (n + 1) / (2 * (1 + _EyeScleraWrap));
    diffuse = scleraColor * ( indirectGI + lightColor * diffuseTerm * lightAttenuation);
    float occ = SAMPLE_TEXTURE2D(_OcclusionMap, smp, occlusionCoords).r;
    diffuse *= ColorBleedAO(lerp(1, occ, _OcclusionStrength));

    //Specular
    half3 specColor = brdfData.specular * DirectBRDFSpecular(brdfData, scleraNormal, lightDirectionWS, i.viewDirectionWS);
    specular = specColor * radiance;

}


float2 PhysicallyBasedRefraction(float2 texcoords, float3 viewW, half3 normalWS)
{
    half3 normal = normalWS;

    float height = _EyeAnteriorChamberDepth * saturate(1.0 - 18.4 * _EyeRadius * _EyeRadius);

    //Calculate Refraction
    float w = _EyeIOR * dot(normal, viewW);
    float k = sqrt(1.0 + (w - _EyeIOR) * (w + _EyeIOR));
    float3 refractedW = (w - k) * normal - _EyeIOR * viewW;

    float cosAlpha = dot(_EyeLookVector, -refractedW);
    float dist = height / cosAlpha;
    float3 offsetW = dist * refractedW;

    //NOTE: Right now the effect is correct when multiplying by the model matrix, and negating the look vector.
    //      Mathematically, we should be transforming the offset into texture space.
    float2 offsetT = mul(offsetW, (float3x3)unity_ObjectToWorld).xy;

    float mask = 1 - SAMPLE_TEXTURE2D(_ParallaxMap, smp, texcoords.xy).r;
    //return offsetT;

    return float2(mask, mask) * offsetT;
}

half EyeDisneyDiffuse(half NdotV, half NdotL, half LdotH, half perceptualRoughness)
{
    half fd90 = 0.5 + 2 * LdotH * LdotH * perceptualRoughness;
    // Two schlick fresnel term
    half lightScatter = (1 + (fd90 - 1) * pow((1 - NdotL),5));
    half viewScatter = (1 + (fd90 - 1) * pow((1 - NdotV),5));

    return lightScatter * viewScatter;
}

float Sigmoid(float x) {
    return 1.0 / (1.0 + exp(-(x - _EyeLimbusShift) / _EyeLimbusSlope));
}

half3 NormalInTangentSpace(float2 texcoords, bool isSclera)
{
    half3 normalTangent;
    if (isSclera) {
        normalTangent = UnpackNormalScale(SAMPLE_TEXTURE2D(_EyeScleraBumpMap, smp, texcoords), _EyeScleraBumpScale);
    }
    else {
        normalTangent = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, smp, texcoords), _BumpScale);
    }

    return normalTangent;
}

half SpecularOcclusion(float2 texcoords, float3 viewW)
{
    //

    half3 normalT = NormalInTangentSpace(texcoords, true);
    viewW.y = -viewW.y;
    texcoords.xy -= 0.14 * viewW.xy;
    texcoords.xy -= 0.01 * normalT.xy;
    
    return lerp(1, SAMPLE_TEXTURE2D(_EyeScleraSpecularOcclusion, smp, texcoords).r, _EyeScleraSpecularOcclusionScale);

    
}


float2 Parallax(float2 texcoords, half3 viewDir)
{
    // D3D9/SM30 supports up to 16 samplers, skip the parallax map in case we exceed the limit
#define EXCEEDS_D3D9_SM3_MAX_SAMPLER_COUNT  (defined(LIGHTMAP_ON) && defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) && defined(_NORMALMAP) && \
                                             defined(_EMISSION) && defined(_DETAIL) && (defined(_METALLICGLOSSMAP) || defined(_SPECGLOSSMAP)))

#if !defined(_PARALLAXMAP) || (SHADER_TARGET < 30) || (defined(SHADER_API_D3D9) && EXCEEDS_D3D9_SM3_MAX_SAMPLER_COUNT)
    // SM20: instruction count limitation
    // SM20: no parallax
    return texcoords;
#else
    half h = SAMPLE_TEXTURE2D(_ParallaxMap,smp, texcoords).g;
    float2 offset = ParallaxOffset1Step(h, _Parallax, viewDir);
    return float2(texcoords.xy + offset);
#endif

#undef EXCEEDS_D3D9_SM3_MAX_SAMPLER_COUNT
}


#endif
