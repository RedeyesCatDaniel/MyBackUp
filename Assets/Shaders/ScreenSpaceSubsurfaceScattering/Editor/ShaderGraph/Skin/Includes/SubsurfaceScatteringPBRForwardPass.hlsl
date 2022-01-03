void BuildInputData(Varyings input, SurfaceDescription surfaceDescription, SurfaceDescriptionInputs surfaceDescriptionInputs, out InputData inputData)
{
    inputData = (InputData)0;
    inputData.positionWS = input.positionWS;

    #ifdef _NORMALMAP
        #if _NORMAL_DROPOFF_TS
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
            float3 bitangent = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
            inputData.normalWS = TransformTangentToWorld(surfaceDescription.NormalTS, half3x3(input.tangentWS.xyz, bitangent, input.normalWS.xyz));
        #elif _NORMAL_DROPOFF_OS
            inputData.normalWS = TransformObjectToWorldNormal(surfaceDescription.NormalOS);
        #elif _NORMAL_DROPOFF_WS
            inputData.normalWS = surfaceDescription.NormalWS;
        #endif
    #else
        inputData.normalWS = input.normalWS;
    #endif

    #ifdef _NORMALMODE_NORMAL_FLIP
        if(surfaceDescriptionInputs.FaceSign == 0.0)
            inputData.normalWS = -inputData.normalWS;         
    #elif defined(_NORMALMODE_NORMAL_MIRROR)
        if (surfaceDescriptionInputs.FaceSign == 0.0)
        {
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
            float3 bitangent = crossSign * cross(inputData.normalWS.xyz, input.tangentWS.xyz);
            float3 halftangent = input.tangentWS.xyz + bitangent;
            halftangent *= dot(inputData.normalWS.xyz, halftangent.xyz);
            float3 t = inputData.normalWS.xyz - halftangent;
            float3 mirroredNormal = halftangent - t;
            inputData.normalWS = half4(mirroredNormal,1.0);
        }   
    #endif
        

    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = SafeNormalize(input.viewDirectionWS);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        inputData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif

    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.sh, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
}

PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = (PackedVaryings)0;
    packedOutput = PackVaryings(output);
    return packedOutput;
}



//--------------------------------------------------------------------------------------------------

half4 frag(PackedVaryings packedInput) : SV_TARGET
{
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);

    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

    #if _AlphaClip
        half alpha = surfaceDescription.Alpha;
        clip(alpha - surfaceDescription.AlphaClipThreshold);
    #elif _SURFACE_TYPE_TRANSPARENT
        half alpha = surfaceDescription.Alpha;
    #else
        half alpha = 1;
    #endif

    InputData inputData;

    BuildInputData(unpacked, surfaceDescription, surfaceDescriptionInputs, inputData);

    #ifdef _SPECULAR_SETUP
        float3 specular = surfaceDescription.Specular;
        float metallic = 1;
    #else
        float3 specular = 0;
        float metallic = surfaceDescription.Metallic;
    #endif

    SurfaceData surface         = (SurfaceData)0;
    surface.albedo              = surfaceDescription.BaseColor;
    surface.metallic            = saturate(metallic);
    surface.specular            = specular;
    surface.smoothness          = saturate(surfaceDescription.Smoothness),
    surface.occlusion           = surfaceDescription.Occlusion,
    surface.emission            = surfaceDescription.Emission,
    surface.alpha               = saturate(alpha);

    #ifdef _DUALSPECULARLOBE
        half secondLobeInterpolation = surfaceDescription.LobeInterpolation;
        half secondLobeDerivation = surfaceDescription.LobeDerivation;
    #else
        half secondLobeInterpolation = 0;
        half secondLobeDerivation = 0;
    #endif


    half3 tangentWS = normalize(unpacked.tangentWS.xyz);
    half3 bitangentWS = normalize(cross(inputData.normalWS.xyz, unpacked.tangentWS.xyz));
    half thickness = surfaceDescription.Thickness;
    half subsurfaceRabius = surfaceDescription.SubsurfaceMask;
    uint profileIndex = uint(surfaceDescription.DiffusionProfileHashValue);
    
    //-----------------------------------------------------------------------------------------

    BRDFData brdfData;

    // NOTE: can modify alpha
    InitializeBRDFData(surface.albedo, surface.metallic, surface.specular, surface.smoothness, surface.alpha, brdfData);

    BRDFData brdfDataClearCoat = (BRDFData)0;

    // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    half4 shadowMask = inputData.shadowMask;
#elif !defined (LIGHTMAP_ON)
    half4 shadowMask = unity_ProbesOcclusion;
#else
    half4 shadowMask = half4(1, 1, 1, 1);
#endif

#ifdef _SPECULARHIGHLIGHTS_OFF
    bool specularHighlightsOff = true;
#else
    bool specularHighlightsOff = false;
#endif

    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);

#if defined(_SCREEN_SPACE_OCCLUSION)
    AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
    mainLight.color *= aoFactor.directAmbientOcclusion;
    surface.occlusion = min(surface.occlusion, aoFactor.indirectAmbientOcclusion);
#endif


    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    half3 diffuseGIContribution;
    half3 specularGIContribution;
    SplitGlobalIllumination(brdfData, brdfDataClearCoat, surface.clearCoatMask,
        inputData.bakedGI, surface.occlusion, inputData.normalWS, inputData.viewDirectionWS,
        diffuseGIContribution, specularGIContribution);
    half3 diffuseDirectContribution;
    half3 specularDirectContribution;
    SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, mainLight, secondLobeInterpolation, secondLobeDerivation,
        inputData.normalWS, inputData.viewDirectionWS, surface.clearCoatMask, specularHighlightsOff,
        diffuseDirectContribution, specularDirectContribution);

    half atten = (mainLight.distanceAttenuation);

    half NdotL = dot(inputData.normalWS, mainLight.direction);
    half3 color;

    Transmission(diffuseDirectContribution, GetThickness(thickness, profileIndex), subsurfaceRabius,
        mainLight.shadowAttenuation, atten, NdotL, mainLight.color, brdfData.diffuse, profileIndex);


#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
#if defined(_SCREEN_SPACE_OCCLUSION)
        light.color *= aoFactor.directAmbientOcclusion;
#endif
        half3 diffuseAdditionalLightContribution;
        half3 specularAdditionalLightContribution;
        SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, light, secondLobeInterpolation, secondLobeDerivation,
            inputData.normalWS, inputData.viewDirectionWS, surface.clearCoatMask, specularHighlightsOff,
            diffuseAdditionalLightContribution, specularAdditionalLightContribution);

        atten = (light.distanceAttenuation);
        NdotL = dot(inputData.normalWS, light.direction);

        Transmission(diffuseAdditionalLightContribution, GetThickness(thickness, profileIndex), subsurfaceRabius,
            light.shadowAttenuation, atten, NdotL, light.color, brdfData.diffuse, profileIndex);

        diffuseDirectContribution += diffuseAdditionalLightContribution;
        specularDirectContribution += specularAdditionalLightContribution;

    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    color += inputData.vertexLighting * brdfData.diffuse;
#endif

    color += surface.emission;

    half3 diffuseContribution = diffuseGIContribution + diffuseDirectContribution;
    half3 specularContribution = specularGIContribution + specularDirectContribution;

#ifdef _SUBSURFACE_PASS
    color = specularContribution;
#else
    color = diffuseContribution + specularContribution;
#endif

#ifdef _EMISSION
    color += _EmissionColor;
#endif

    return half4(color, surface.alpha);
}
