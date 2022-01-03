#ifndef UNIVERSAL_SKINLIGHTING_INCLUDED
#define UNIVERSAL_SKINLIGHTING_INCLUDED

#define TRANSMISSION_ON
#define SKIN_IOR 1.36
#define SKIN_IETA 0.735 // 1.0 / 1.36

inline void InitializeBRDFDataDualLobe(half LobeMix, half SmoothnessB, inout BRDFData baseBRDFData, out BRDFData outBRDFData)
{
    outBRDFData = (BRDFData)0;
    outBRDFData.albedo = half(1.0);

    // Calculate Roughness of Dual Lobe layer
    outBRDFData.diffuse             = baseBRDFData.diffuse;// kDielectricSpec.aaa; // 1 - kDielectricSpec
    outBRDFData.specular            = baseBRDFData.specular; // kDielectricSpec.rgb;
    outBRDFData.reflectivity        = baseBRDFData.reflectivity;// kDielectricSpec.r;

    outBRDFData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(SmoothnessB);
    outBRDFData.roughness           = max(PerceptualRoughnessToRoughness(outBRDFData.perceptualRoughness), HALF_MIN_SQRT);
    outBRDFData.roughness2          = max(outBRDFData.roughness * outBRDFData.roughness, HALF_MIN);
    outBRDFData.normalizationTerm   = outBRDFData.roughness * 4.0h + 2.0h;
    outBRDFData.roughness2MinusOne  = outBRDFData.roughness2 - 1.0h;
    outBRDFData.grazingTerm         = saturate(SmoothnessB + kDielectricSpec.x);

// Relatively small effect, cut it for lower quality
#if !defined(SHADER_API_MOBILE)

    // Modify Roughness of base layer using Skin IOR
    half ieta                        = lerp(1.0h, SKIN_IETA, LobeMix);
    half skinRoughnessScale          = Sq(ieta);
    half sigma                       = RoughnessToVariance(PerceptualRoughnessToRoughness(baseBRDFData.perceptualRoughness));

    baseBRDFData.perceptualRoughness = RoughnessToPerceptualRoughness(VarianceToRoughness(sigma * skinRoughnessScale));

    // Recompute base material for new roughness, previous computation should be eliminated by the compiler (as it's unused)
    baseBRDFData.roughness          = max(PerceptualRoughnessToRoughness(baseBRDFData.perceptualRoughness), HALF_MIN_SQRT);
    baseBRDFData.roughness2         = max(baseBRDFData.roughness * baseBRDFData.roughness, HALF_MIN);
    baseBRDFData.normalizationTerm  = baseBRDFData.roughness * 4.0h + 2.0h;
    baseBRDFData.roughness2MinusOne = baseBRDFData.roughness2 - 1.0h;
#endif

#ifdef _DUAL_SPECULAR_LOBE
    // Darken/saturate base layer using coat to surface reflectance (vs. air to surface)
    //baseBRDFData.specular = lerp(baseBRDFData.specular, ConvertF0ForClearCoat15(baseBRDFData.specular), LobeMix);
    // TODO: what about diffuse? at least in specular workflow diffuse should be recalculated as it directly depends on it.
#endif
}

// https://game.watch.impress.co.jp/docs/news/575412.html
half3 ColorBleedAO(half occlusion, half3 colorBleed) {
    return pow(abs(occlusion), 1.0 - colorBleed);
}

half3 SkinGlobalIllumination(BRDFData brdfData, BRDFData brdfDataDualLobe, half lobeMix ,
    half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS )
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half NoV = saturate(dot(normalWS, viewDirectionWS));
    half fresnelTerm = Pow4(1.0 - NoV);

    half specOcc = GetSpecularOcclusionFromAmbientOcclusion(NoV, occlusion, brdfData.roughness);
    half3 colorBleedAO = ColorBleedAO(occlusion, brdfData.diffuse);

    half3 indirectDiffuse = bakedGI * colorBleedAO;
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness, specOcc);

    half3 color = EnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);

#if defined(_DUAL_SPECULAR_LOBE) || defined(_DUAL_SPECULAR_LOBEMAP)
    half3 skinIndirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfDataDualLobe.perceptualRoughness, specOcc); 
    half3 dualLobeColor = EnvironmentBRDF(brdfDataDualLobe, indirectDiffuse, skinIndirectSpecular, fresnelTerm);

    return lerp(color, dualLobeColor, lobeMix);
#else
    return color;
#endif
}

half3 SkinLightingPhysicallyBased(  BRDFData brdfData, BRDFData brdfDataDualLobe,
                                    Light light,
                                    half3 normalWS, half3 viewDirectionWS, SurfaceData surfaceData, SkinSurfaceData skinSurface,
                                    half3x3 tangentToWorld, bool specularHighlightsOff)
{
    half3 lightColor        = light.color;
    half3 lightDirectionWS  = light.direction;
    half lightAttenuation   = light.distanceAttenuation * light.shadowAttenuation;

    // Lambert Term
    half NdotL = dot(normalWS, lightDirectionWS);
    half3 radiance = lightColor * (lightAttenuation * saturate(NdotL));

    ///////////////////////////////////////////////////////////////////////////////
    //                             Transmission                                  //
    ///////////////////////////////////////////////////////////////////////////////
    half3 transmission = Transmittance(skinSurface.thickness, light.shadowAttenuation, light.distanceAttenuation, NdotL,
                                        skinSurface.transmission, skinSurface.scatteringColor);

    ///////////////////////////////////////////////////////////////////////////////
    //                          Subsurface Scattering                            //
    ///////////////////////////////////////////////////////////////////////////////
    // approx for Radius 
    half maxRadius = Luminance(skinSurface.scatteringColor);
    half scatterDist = maxRadius ;
    half blurriness = exp(-maxRadius * 0.25 );
    half3 subsurface = SubsurfaceScattering(tangentToWorld, lightDirectionWS, normalWS, skinSurface.scatteringColor,
                                            scatterDist, blurriness, light.shadowAttenuation, light.distanceAttenuation, skinSurface.uv);

    subsurface *= lightColor;
    subsurface += transmission * lightColor;

    // SubsurfaceMask to determine between skin surface (white) and regular PBR surface (Black)
    radiance = lerp(radiance, subsurface, skinSurface.subsurfaceMask);

    half3 brdf = brdfData.diffuse;
    half3 brdfLobe = half3(0,0,0);
#ifndef _SPECULARHIGHLIGHTS_OFF
    [branch] if (!specularHighlightsOff)
    {
        brdfLobe = brdfData.specular * DirectBRDFSpecular(brdfData, normalWS, lightDirectionWS, viewDirectionWS);

#if defined(_DUAL_SPECULAR_LOBE) || defined(_DUAL_SPECULAR_LOBEMAP)
        // Dual Lobe evaluates the specular a second time and has some common terms with the base specular.
        // We rely on the compiler to merge these and compute them only once.
        half3 brdfDualLobe = brdfData.specular * DirectBRDFSpecular(brdfDataDualLobe, normalWS, lightDirectionWS, viewDirectionWS);

        brdf += lerp(brdfLobe, brdfDualLobe, skinSurface.lobeMix);
#else
        brdf += brdfLobe;
#endif // _DUAL_SPECULAR_LOBE
    }
#endif // _SPECULARHIGHLIGHTS_OFF
 
    return brdf * radiance;
}

///////////////////////////////////////////////////////////////////////////////
//                      Fragment Functions                                   //
//       Used by ShaderGraph and others builtin renderers                    //
///////////////////////////////////////////////////////////////////////////////
half4 UniversalFragmentSkin(InputData inputData, SurfaceData surfaceData, SkinSurfaceData skinSurface)
{
    
#if defined(_SPECULARHIGHLIGHTS_OFF)
    bool specularHighlightsOff = true;
#else
    bool specularHighlightsOff = false;
#endif

    BRDFData brdfData;

    // NOTE: can modify alpha
    InitializeBRDFData(surfaceData, brdfData);
    
#if defined(DEBUG_DISPLAY)
    half4 debugColor;

    if (CanDebugOverrideOutputColor(inputData, surfaceData, brdfData, debugColor))
    {
        return debugColor;
    }
#endif

    // Dual Lobe calculation...
    BRDFData brdfDataDualLobe = (BRDFData)0;
#if defined(_DUAL_SPECULAR_LOBE) || defined(_DUAL_SPECULAR_LOBEMAP)
    // base brdfData is modified here, rely on the compiler to eliminate dead computation by InitializeBRDFData()
    InitializeBRDFDataDualLobe(skinSurface.lobeMix, skinSurface.smoothnessB, brdfData, brdfDataDualLobe);
#endif

    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);

    // NOTE: We don't apply AO to the GI here because it's done in the lighting calculation below...
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);

    LightingData lightingData = CreateLightingData(inputData, surfaceData);

    lightingData.giColor = SkinGlobalIllumination(brdfData, brdfDataDualLobe, skinSurface.lobeMix,
                                     inputData.bakedGI, aoFactor.indirectAmbientOcclusion,
                                     inputData.normalWS, inputData.viewDirectionWS);

    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
        lightingData.mainLightColor= SkinLightingPhysicallyBased(brdfData, brdfDataDualLobe,
                                     mainLight,
                                     inputData.normalWS, inputData.viewDirectionWS,
                                     surfaceData, skinSurface, inputData.tangentToWorld, specularHighlightsOff);
    }
   
#if defined(_ADDITIONAL_LIGHTS)
    uint pixelLightCount = GetAdditionalLightsCount();

    #if USE_CLUSTERED_LIGHTING
    for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);
        
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            lightingData.additionalLightsColor += SkinLightingPhysicallyBased(brdfData, brdfDataDualLobe, light,
                                                                                inputData.normalWS, inputData.viewDirectionWS,
                                                                                surfaceData, skinSurface, inputData.tangentToWorld, specularHighlightsOff);
        }
    }
    #endif
 
    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            lightingData.additionalLightsColor += SkinLightingPhysicallyBased(brdfData, brdfDataDualLobe, light,
                                                                                inputData.normalWS, inputData.viewDirectionWS,
                                                                                surfaceData, skinSurface, inputData.tangentToWorld, specularHighlightsOff);
        }
    LIGHT_LOOP_END
      
#endif

    #if defined(_ADDITIONAL_LIGHTS_VERTEX)
    lightingData.vertexLightingColor += inputData.vertexLighting * brdfData.diffuse;
#endif
    
    return CalculateFinalColor(lightingData, surfaceData.alpha);
}

#endif
