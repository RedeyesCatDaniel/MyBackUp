#ifndef UNIVERSAL_MYUTILITY1_INCLUDED
#define UNIVERSAL_MYUTILITY1_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

#if (_DETAIL_MULX2 || _DETAIL_MUL || _DETAIL_ADD || _DETAIL_LERP)
#define _DETAIL 1
#endif

#define UNITY_PI            3.14159265359f
#define UNITY_INV_PI        0.31830988618f


float4 _ShapeParams[8];        // RGB = S = 1 / D, A = filter radius
float4 _TransmissionTints[8];  // RGB = color, A = unused
float  _ThicknessRemaps[8][2]; // Remap: 0 = start, 1 = end - start


//------------------------------------------------------------ BRDFs --------------------------------------------------------------------


void SplitGlobalIllumination(BRDFData brdfData, BRDFData brdfDataClearCoat, float clearCoatMask,
    half3 bakedGI, half occlusion,
    half3 normalWS, half3 viewDirectionWS, out half3 diffuseGI, out half3 specularGI)
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half NoV = saturate(dot(normalWS, viewDirectionWS));
    half fresnelTerm = Pow4(1.0 - NoV);

    //Calculate AO Color Bleeding
    //TODO
    half3 colorBleedingAO = saturate(pow(saturate(occlusion.xxx), (half3(1, 1, 1))));

#if defined(_SPECULARAO)
    half s = saturate(NoV * NoV - 0.3);
    half SO = lerp(pow(occlusion, 8), 1.0, s);
#else
    half SO = 1;
#endif

    half3 indirectDiffuse = bakedGI * colorBleedingAO;
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness, SO);

    half3 color = EnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);

    indirectDiffuse *= brdfData.diffuse;
    indirectSpecular *= EnvironmentBRDFSpecular(brdfData, fresnelTerm);

#if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
    half3 coatIndirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfDataClearCoat.perceptualRoughness, occlusion);
    // TODO: "grazing term" causes problems on full roughness
    half3 coatColor = EnvironmentBRDFClearCoat(brdfDataClearCoat, clearCoatMask, coatIndirectSpecular, fresnelTerm);

    // Blend with base layer using khronos glTF recommended way using NoV
    // Smooth surface & "ambiguous" lighting
    // NOTE: fresnelTerm (above) is pow4 instead of pow5, but should be ok as blend weight.
    half coatFresnel = kDielectricSpec.x + kDielectricSpec.a * fresnelTerm;
    indirectDiffuse *= (1.0 - coatFresnel * clearCoatMask) + coatColor;
    indirectSpecular *= (1.0 - coatFresnel * clearCoatMask) + coatColor;

#endif

    diffuseGI = indirectDiffuse;
    specularGI = indirectSpecular;
}
// Computes the scalar specular term for Minimalist CookTorrance BRDF
// NOTE: needs to be multiplied with reflectance f0, i.e. specular color to complete
half DirectBRDFSpecularDualLobes(BRDFData brdfData, half lobeInterpolation, half lobeDerivation, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS)
{
    float3 halfDir = SafeNormalize(float3(lightDirectionWS)+float3(viewDirectionWS));

    float NoH = saturate(dot(normalWS, halfDir));
    half LoH = saturate(dot(lightDirectionWS, halfDir));

    // GGX Distribution multiplied by combined approximation of Visibility and Fresnel
    // BRDFspec = (D * V * F) / 4.0
    // D = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2
    // V * F = 1.0 / ( LoH^2 * (roughness + 0.5) )
    // See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
    // https://community.arm.com/events/1155

    // Final BRDFspec = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2 * (LoH^2 * (roughness + 0.5) * 4.0)
    // We further optimize a few light invariant terms
    // brdfData.normalizationTerm = (roughness + 0.5) * 4.0 rewritten as roughness * 4.0 + 2.0 to a fit a MAD.

    //TODO
    float secondRoughness = brdfData.roughness * (2 - lobeDerivation);
    
    float secondRoughness2MinusOne = secondRoughness * secondRoughness - 1;

    float d = NoH * NoH * brdfData.roughness2MinusOne + 1.00001f;
    float d2 = NoH * NoH * secondRoughness2MinusOne + 1.00001f;

    half normalizationTerm2 = secondRoughness * 4.0 + 2.0;

    half LoH2 = LoH * LoH;
    half specularTerm1 = brdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfData.normalizationTerm);
    half specularTerm2 = (secondRoughness * secondRoughness) / ((d2 * d2) * max(0.1h, LoH2) * normalizationTerm2);

    //TODO
    half specularTerm = lerp(specularTerm1, specularTerm2, lobeInterpolation);
    
    // On platforms where half actually means something, the denominator has a risk of overflow
    // clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
    // sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
    specularTerm = specularTerm - HALF_MIN;
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif

    return specularTerm;
}


void SplitLightingPhysicallyBased(BRDFData brdfData, BRDFData brdfDataClearCoat, Light light, half lobeInterpolation, half lobeDerivation,
    half3 normalWS, half3 viewDirectionWS, half clearCoatMask, bool specularHighlightsOff, out half3 diretDiffuse, out half3 directSpecular)
{
    half3 lightColor = light.color;
    half3 lightDirectionWS = light.direction;
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    half NdotL = saturate(dot(normalWS, lightDirectionWS));
    half3 radiance = lightColor * (lightAttenuation * NdotL);

    half3 brdf = 0;
#ifndef _SPECULARHIGHLIGHTS_OFF
    [branch] if (!specularHighlightsOff)
    {
#ifdef _DUALSPECULARLOBE
        brdf = brdfData.specular * DirectBRDFSpecularDualLobes(brdfData, lobeInterpolation, lobeDerivation, normalWS, lightDirectionWS, viewDirectionWS);
#else
        brdf = brdfData.specular * DirectBRDFSpecular(brdfData, normalWS, lightDirectionWS, viewDirectionWS);
#endif

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
    diretDiffuse = brdfData.diffuse * radiance;
    directSpecular = brdf * radiance;
    //return brdf * radiance;
}


//------------------------------------------------------------Transmission--------------------------------------------------------------------

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

float GetThickness(float thickness, uint profileIndex) {

    thickness = (_ThicknessRemaps[profileIndex][0] +
        _ThicknessRemaps[profileIndex][1] * thickness);

    return thickness;
}

#define SSS_TRSM_MODE_NONE       (0)
#define SSS_TRSM_MODE_THIN       (1)

void Transmission(inout half3 diffuse,
    float thickness, float radius,
    half shadow, half atten, float NdotL,
    half3 lightColor, half3 diffuseColor, uint profileId)
{
    int transmissionMode = 1; //TODO

    bool enableTransmission = transmissionMode != SSS_TRSM_MODE_NONE;

    if (enableTransmission) {
        bool useThinObjectMode = transmissionMode == SSS_TRSM_MODE_THIN;

        float illuminance = F_Transm_Schlick(0.028, saturate(-NdotL)) * atten;
        shadow = useThinObjectMode ? shadow : 1;
        illuminance *= shadow;

        float3 backLight = lightColor * illuminance;

        float3 transmittedLight = backLight * (diffuseColor * ComputeTransmittance(_ShapeParams[profileId].rgb,
            _TransmissionTints[profileId].rgb,
            thickness, radius));

        diffuse += transmittedLight;
    }
}

#endif
