#ifndef UNITYHAIR_BSDF_HLSL
#define UNITYHAIR_BSDF_HLSL

//-----------------------------------------------------------------------------
// conversion function for forward
//-----------------------------------------------------------------------------

real RoughnessToBlinnPhongSpecularExponent(real roughness)
{
    return clamp(2 * rcp(roughness * roughness) - 2, FLT_EPS, rcp(FLT_EPS));
}
//-----------------------------------------------------------------------------
// Unity Hair Lighting functions (based on HDRP hair lighting model)
//-----------------------------------------------------------------------------

#define DEFAULT_HAIR_SPECULAR_VALUE 0.0465 // Hair is IOR 1.55

struct BSDFData
{
    half secondaryGrazingTerm;
    half ambientOcclusion;
    half3 diffuseColor;
    half3 fresnel0;
    half3 specularTint;
    half3 normalWS;
    half3 geomNormalWS;
    half perceptualRoughness;
    half3 transmittance;
    half rimTransmissionIntensity;
    half3 hairStrandDirectionWS;
    half anisotropy;
    half secondaryPerceptualRoughness;
    half3 secondarySpecularTint;
    half specularExponent;
    half secondarySpecularExponent;
    half specularShift;
    half secondarySpecularShift;
};

// ref: HDRP ver.10 - High Definition RP/Runtime/Material/Hair/Hair.hlsl #130
BSDFData ConvertSurfaceDataToBSDFData(SurfaceData surfaceData, HairSurfaceData hairSurfaceData, half occlusion)
{
    BSDFData bsdfData;
    ZERO_INITIALIZE(BSDFData, bsdfData);

    bsdfData.ambientOcclusion               = occlusion;
    bsdfData.diffuseColor                   = surfaceData.albedo;

    bsdfData.normalWS                       = hairSurfaceData.normalWS;
    bsdfData.geomNormalWS                   = hairSurfaceData.geomNormalWS;

    half secondaryReflectivity              = ReflectivitySpecular(hairSurfaceData.secondarySpecularTint);
    bsdfData.secondaryGrazingTerm           = saturate(hairSurfaceData.secondarySmoothness + secondaryReflectivity);
    // Diffuse has no energy conservation, Balancing energy is left to artist
    bsdfData.specularTint                   = hairSurfaceData.specularTint;
    bsdfData.secondarySpecularTint          = hairSurfaceData.secondarySpecularTint;

    bsdfData.perceptualRoughness            = PerceptualSmoothnessToPerceptualRoughness(surfaceData.smoothness);
    bsdfData.secondaryPerceptualRoughness   = PerceptualSmoothnessToPerceptualRoughness(hairSurfaceData.secondarySmoothness);
    real roughness1                         = PerceptualRoughnessToRoughness(bsdfData.perceptualRoughness);
    real roughness2                         = PerceptualRoughnessToRoughness(bsdfData.secondaryPerceptualRoughness);

    bsdfData.specularExponent               = RoughnessToBlinnPhongSpecularExponent(roughness1);
    bsdfData.secondarySpecularExponent      = RoughnessToBlinnPhongSpecularExponent(roughness2);
    bsdfData.specularShift                  = hairSurfaceData.specularShift;
    bsdfData.secondarySpecularShift         = hairSurfaceData.secondarySpecularShift;

    bsdfData.fresnel0                       = DEFAULT_HAIR_SPECULAR_VALUE;
    bsdfData.transmittance                  = hairSurfaceData.transmittance;
    bsdfData.rimTransmissionIntensity       = hairSurfaceData.rimTransmissionIntensity;

    // This is the hair tangent (which represents the hair strand direction, root to tip).
    bsdfData.hairStrandDirectionWS          = hairSurfaceData.hairStrandDirection;

    bsdfData.anisotropy                     = 0.8; // For hair we fix the anisotropy

    return bsdfData;
}

// CBSDF Struct in: com.unity.render-pipeline.core/ShaderLibrary/BSDF.hlsl
// ref: HDRP ver.10.1.x - High Definition RP/Runtime/Material/Hair/Hair.hlsl #359
// (BSDF) Bidirectional Scattering Distribution Function
CBSDF EvaluateBSDF(half3 V, half3 L, BSDFData bsdfData)
{
    CBSDF cbsdf;
    ZERO_INITIALIZE(CBSDF, cbsdf);

    half3 T = bsdfData.hairStrandDirectionWS;
    half3 N = bsdfData.normalWS; // it is view facing normal

#if _USE_LIGHT_FACING_NORMAL
    // The Kajiya-Kay model has a "built-in" transmission, and the 'NdotL' is always positive.
    half cosTL = dot(T, L);
    half sinTL = sqrt(saturate(1.0 - cosTL * cosTL));
    half NdotL = sinTL; // Corresponds to the cosine w.r.t. the light-facing normal
#else
    // Double-sided Lambert.
    float NdotL = dot(N, L);
#endif

    half NdotV = dot(N, V);// preLightData.NdotV;
    half clampedNdotV = ClampNdotV(NdotV);
    half clampedNdotL = saturate(NdotL);

    half LdotV, NdotH, LdotH, invLenLV;
    GetBSDFAngle(V, L, NdotL, NdotV, LdotV, NdotH, LdotH, invLenLV);

    half3 t1 = ShiftTangent(T, N, bsdfData.specularShift);
    half3 t2 = ShiftTangent(T, N, bsdfData.secondarySpecularShift);

    half3 H = (L + V) * invLenLV;

    // Balancing energy between lobes, as well as between diffuse and specular is left to artists.
    half3 hairSpec1 = bsdfData.specularTint          * D_KajiyaKay(t1, H, bsdfData.specularExponent);
    half3 hairSpec2 = bsdfData.secondarySpecularTint * D_KajiyaKay(t2, H, bsdfData.secondarySpecularExponent);

    half3 F = F_Schlick(bsdfData.fresnel0, LdotH);

#if _USE_LIGHT_FACING_NORMAL
    // See "Analytic Tangent Irradiance Environment Maps for Anisotropic Surfaces".
    //cbsdf.diffR = rcp(PI * PI)* clampedNdotL;
    cbsdf.diffR = bsdfData.anisotropy * clampedNdotL; // Michael: modified multiplication to get correct brigthness in URP
    // Transmission is built into the model, and it's not exactly clear how to split it.
    cbsdf.diffT = 0;
#else
    // Double-sided Lambert.
    //cbsdf.diffR = Lambert() * clampedNdotL;
    cbsdf.diffR = clampedNdotL;
#endif
    // Bypass the normal map...
    half geomNdotV = dot(bsdfData.geomNormalWS, V);

    // G = NdotL * NdotV. // Michael: modified multiplication from 0.25 to 1 that match the brightness closer to the HDRP's hair specular result
    cbsdf.specR = F * (hairSpec1 + hairSpec2) * clampedNdotL * saturate(geomNdotV * FLT_MAX);

    // Yibing's and Morten's hybrid scatter model hack.
    half scatterFresnel1 = pow(saturate(-LdotV), 9.0) * pow(saturate(1.0 - geomNdotV * geomNdotV), 12.0);
    half scatterFresnel2 = saturate(PositivePow((1.0 - geomNdotV), 20.0));

    cbsdf.specT = scatterFresnel1 + bsdfData.rimTransmissionIntensity * scatterFresnel2;

    return cbsdf;
}

half3 EvaluateBSDF_Env(BSDFData bsdfData, InputData inputData)
{
    half3 reflectVector = reflect(-inputData.viewDirectionWS, bsdfData.normalWS); // iblR
    half NoV = saturate(dot(bsdfData.normalWS, inputData.viewDirectionWS));
    half fresnelTerm = Pow4(1.0 - NoV);

    half3 indirectDiffuse = inputData.bakedGI * bsdfData.ambientOcclusion;

    // ref: HDRP ver.10.1.x - High Definition RP/Runtime/Material/Hair/Hair.hlsl #286
    // Note: For Kajiya hair we currently rely on a single cubemap sample instead of two, as in practice smoothness of both lobe aren't too far from each other.
    // and we take smoothness of the secondary lobe as it is often more rough (it is the colored one).
    half iblPerceptualRoughness = bsdfData.secondaryPerceptualRoughness;

    // Michael: We do an approximation of roughness only here, skip the GetPreIntegratedFGDGGXAndDisneyDiffuse() function for URP
    iblPerceptualRoughness *= saturate(1.2 - bsdfData.anisotropy); // constant 0.4
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, iblPerceptualRoughness, bsdfData.ambientOcclusion); // function in Lighting.hlsl

    // ref: HDRP ver.10.1.x - High Definition RP/Runtime/Material/Hair/Hair.hlsl #578
    // We tint the HDRI with the secondary lob specular as it is more representatative of indirect lighting on hair.
    indirectSpecular *= bsdfData.secondarySpecularTint;

    // Specular Occulsion From AO
    indirectSpecular *= bsdfData.ambientOcclusion;

    half3 c = indirectDiffuse * bsdfData.diffuseColor;
    float surfaceReduction = 1.0 / (iblPerceptualRoughness * iblPerceptualRoughness + 1.0);
    c += surfaceReduction * indirectSpecular * lerp(bsdfData.fresnel0, bsdfData.secondaryGrazingTerm, fresnelTerm);

    return c;
}

half4 LightingUnityHair(InputData inputData, SurfaceData surfaceData, HairSurfaceData hairSurfaceData)
{
    
    #if defined(DEBUG_DISPLAY)
    half4 debugColor;

    if (CanDebugOverrideOutputColor(inputData, surfaceData, debugColor))
    {
        return debugColor;
    }
    #endif
    
    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);
    
    BSDFData bsdfData = ConvertSurfaceDataToBSDFData(surfaceData, hairSurfaceData, aoFactor.indirectAmbientOcclusion);

    // NOTE: We don't apply AO to the GI here because it's done in the lighting calculation below...
    // MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    
    LightingData lightingData = CreateLightingData(inputData, surfaceData);
    
    // Indirect lighting
    lightingData.giColor = EvaluateBSDF_Env(bsdfData, inputData);

    // Direct lighting
    // ref: HDRP ver.10 - High Definition RP/Runtime/Lighting/SurfaceShading.hlsl #39
    CBSDF cbsdf = EvaluateBSDF(inputData.viewDirectionWS, mainLight.direction, bsdfData);

    half3 lightColor = mainLight.color * mainLight.shadowAttenuation;

    half3 transmittance = bsdfData.transmittance;
    half3 diffuse = (cbsdf.diffR) * lightColor;
    half3 specular = (cbsdf.specR + cbsdf.specT * transmittance) * lightColor;
    
    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
        lightingData.mainLightColor = diffuse * bsdfData.diffuseColor + specular;
    }
    
#if defined(_ADDITIONAL_LIGHTS)
    uint pixelLightCount = GetAdditionalLightsCount();
    
    #if USE_CLUSTERED_LIGHTING
    for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            CBSDF cbsdf = EvaluateBSDF(inputData.viewDirectionWS, light.direction, bsdfData);

            lightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);

            diffuse += (cbsdf.diffR) * lightColor;
            specular += (cbsdf.specR + cbsdf.specT * transmittance) * lightColor;

            lightingData.additionalLightsColor += diffuse * bsdfData.diffuseColor + specular;
        }
    }
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
    Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    {
        CBSDF cbsdf = EvaluateBSDF(inputData.viewDirectionWS, light.direction, bsdfData);

        lightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);

        diffuse += (cbsdf.diffR) * lightColor;
        specular += (cbsdf.specR + cbsdf.specT * transmittance) * lightColor;

        lightingData.additionalLightsColor += diffuse * bsdfData.diffuseColor + specular;
    }
    LIGHT_LOOP_END
#endif

    #if defined(_ADDITIONAL_LIGHTS_VERTEX)
    lightingData.vertexLightingColor += inputData.vertexLighting * brdfData.diffuse;
    #endif
    
    return CalculateFinalColor(lightingData, surfaceData.alpha);
}

#endif
