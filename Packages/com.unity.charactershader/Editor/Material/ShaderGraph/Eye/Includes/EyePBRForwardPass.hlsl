
void InitializeInputData(Varyings input, SurfaceDescription surfaceDescription, out InputData inputData)
{
    inputData = (InputData)0;

    inputData.positionWS = input.positionWS;

    #ifdef _NORMALMAP
        // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
        float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        float3 bitangent = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);

        inputData.tangentToWorld = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);
        #if _NORMAL_DROPOFF_TS
            inputData.normalWS = TransformTangentToWorld(surfaceDescription.NormalTS, inputData.tangentToWorld);
        #elif _NORMAL_DROPOFF_OS
            inputData.normalWS = TransformObjectToWorldNormal(surfaceDescription.NormalOS);
        #elif _NORMAL_DROPOFF_WS
            inputData.normalWS = surfaceDescription.NormalWS;
        #endif
    #else
        inputData.normalWS = input.normalWS;
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

    inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactorAndVertexLight.x);
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
#if defined(DYNAMICLIGHTMAP_ON)
    inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV.xy, input.sh, inputData.normalWS);
#else
    inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.sh, inputData.normalWS);
#endif
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);

    #if defined(DEBUG_DISPLAY)
    #if defined(DYNAMICLIGHTMAP_ON)
    inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
    #endif
    #if defined(LIGHTMAP_ON)
    inputData.staticLightmapUV = input.staticLightmapUV;
    #else
    inputData.vertexSH = input.sh;
    #endif
    #endif
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

void SplitGlobalIllumination(BRDFData brdfData, half3 bakedGI, half occlusion, half specOcc,
    half3 normalWS, half3 viewDirectionWS, out half3 diffuseGI, out half3 specularGI)
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half NoV = saturate(dot(normalWS, viewDirectionWS));
    half fresnelTerm = Pow4(1.0 - NoV);

    half3 indirectDiffuse = bakedGI * occlusion * brdfData.diffuse;
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness, occlusion) * specOcc;

    indirectSpecular *= EnvironmentBRDFSpecular(brdfData, fresnelTerm);

    diffuseGI = indirectDiffuse;
    specularGI = indirectSpecular;
}


half3 ColorBleedAO(half occlusion, half3 colorBleed) {
    return pow(abs(occlusion), 1.0 - colorBleed);
}

void ScleraLayerBase(InputData i, BRDFData brdfData, half3 scleraNormal, half3 indirectGI, Light light, half3 ao, inout half3 diffuse, inout half3 specular)
{
    half shiftAmount = dot(scleraNormal, light.direction);
    half3 normal = shiftAmount < 0.0f ? scleraNormal + light.direction * (-shiftAmount + 1e-5f) : scleraNormal;

    half3 lightColor = light.color;
    half3 lightDirectionWS = light.direction;
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    half NdotL = dot(normal, lightDirectionWS);
    half3 radiance = lightColor * (lightAttenuation * saturate(NdotL));

    // SSS
    half _EyeScleraWrap = 0.05; // Change this to control the sss angle

    // This function @ com.unity.render-pipeline.core/ShaderLibrary/CommonLighting.hlsl
    float diffuseTerm = ComputeWrappedPowerDiffuseLighting (NdotL, _EyeScleraWrap, 1.0);
    diffuse = brdfData.diffuse * (indirectGI + lightColor * diffuseTerm * lightAttenuation);
    diffuse *= ao;

    //Specular
    half3 specColor = brdfData.specular * DirectBRDFSpecular(brdfData, scleraNormal, lightDirectionWS, i.viewDirectionWS);
    specular = specColor * radiance;

}

void IrisLayerBase(InputData i, BRDFData brdfData, half3 irisNormal, half3 indirectGI, Light light, half3 ao, inout half3 diffuse, inout half3 specular)
{
    half nv = saturate(dot(irisNormal, i.viewDirectionWS));
    half nl = saturate(dot(irisNormal, light.direction));
    half3 h = normalize(light.direction + i.viewDirectionWS);
    half lh = saturate(dot(light.direction, h));
    // This function @ com.unity.render-pipeline.core/ShaderLibrary/BSDF.hlsl
    half diffuseTerm = DisneyDiffuse(nv, nl, lh, brdfData.perceptualRoughness) * nl;
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;

    diffuse = brdfData.diffuse * (indirectGI + light.color * diffuseTerm * lightAttenuation);
    diffuse *= ao;
    //Specular should be intergrate to sclera

    nl = saturate(dot(i.normalWS.xyz, light.direction));

    half3 specColor = brdfData.specular * DirectBRDFSpecular(brdfData, i.normalWS.xyz, light.direction, i.viewDirectionWS);
    specular = specColor * light.color * lightAttenuation * nl;  
}

//--------------------------------------------------------------------------------------------------

half4 frag(PackedVaryings packedInput) : SV_TARGET
{
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);
    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);

    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

    #if _ALPHATEST_ON
        half alpha = surfaceDescription.Alpha;
        clip(alpha - surfaceDescription.AlphaClipThreshold);
    #elif _SURFACE_TYPE_TRANSPARENT
        half alpha = surfaceDescription.Alpha;
    #else
        half alpha = 1;
    #endif

    InputData inputData;
    InitializeInputData(unpacked, surfaceDescription, inputData);
    half3 irisNormal = normalize(TransformTangentToWorld(surfaceDescription.IrisNormal, inputData.tangentToWorld));

#ifdef _SPECULAR_SETUP
    float3 specular = surfaceDescription.Specular;
    float metallic = 1;
#else
    float3 specular = 0;
    float metallic = surfaceDescription.Metallic;
#endif

    SurfaceData surfaceData = (SurfaceData)0;
    surfaceData.albedo      = surfaceDescription.BaseColor;
    surfaceData.metallic    = saturate(metallic);
    surfaceData.specular    = specular;
    surfaceData.smoothness  = saturate(surfaceDescription.Smoothness),
    surfaceData.occlusion   = surfaceDescription.Occlusion,
    surfaceData.emission    = surfaceDescription.Emission,
    surfaceData.alpha       = saturate(alpha);
    half mask               = saturate(surfaceDescription.Mask); // iris / Sclera Mask
    half specOcc            = surfaceDescription.SpecularOcclusion;
    half3 coloredAO         = ColorBleedAO(surfaceData.occlusion, surfaceDescription.ColoredOcclusion);

#ifdef _DBUFFER
    ApplyDecalToSurfaceData(unpacked.positionCS, surfaceData, inputData);
#endif
    //--------------------------------------Sclera layer-----------------------------------------------------------------------------------

    half3 diffuseGIContribution;
    half3 specularGIContribution;
    
    half3 scleraDiffuse;
    half3 scleraSpecular;

    BRDFData brdfData;

    // NOTE: can modify "surfaceData"...
    InitializeBRDFData(surfaceData, brdfData);

    #if defined(DEBUG_DISPLAY)
    half4 debugColor;
    
    if (CanDebugOverrideOutputColor(inputData, surfaceData, brdfData, debugColor))
    {
        return debugColor;
    }
    #endif
    
    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);

    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    
    LightingData lightingData = CreateLightingData(inputData, surfaceData);
    
    half3 scleraNormal = inputData.normalWS.xyz;

    SplitGlobalIllumination(brdfData, inputData.bakedGI, aoFactor.indirectAmbientOcclusion, specOcc, scleraNormal, inputData.viewDirectionWS,
        diffuseGIContribution, specularGIContribution);

    ScleraLayerBase(inputData, brdfData, scleraNormal, diffuseGIContribution, mainLight, coloredAO, scleraDiffuse, scleraSpecular);
    
    lightingData.giColor = diffuseGIContribution + specularGIContribution;
    
    // half3 ScleraColor = scleraDiffuse + scleraSpecular;// + diffuseGIContribution + specularGIContribution;

    //---------------------------Iris layer--------------------------------------------------------------------------------------------------------------------------
    half3 irisDiffuse = 0;
    half3 irisSpecular = 0;
    
    IrisLayerBase(inputData, brdfData, irisNormal, diffuseGIContribution, mainLight, coloredAO, irisDiffuse, irisSpecular);

    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
        lightingData.mainLightColor = scleraSpecular * specOcc + lerp(irisDiffuse, scleraDiffuse, mask);
    }
    
    //---------------------------Additional Lighting-------------------------------------------------------------------------

#if defined(_ADDITIONAL_LIGHTS)
    half3 additionalScleraDiffuse = 0;
    half3 additionalScleraSpecular = 0;
    half3 additionalIrisDiffuse = 0;
    half3 additionalIrisSpecular = 0;
    
    uint pixelLightCount = GetAdditionalLightsCount();

    #if USE_CLUSTERED_LIGHTING
    for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
        #if defined(_SCREEN_SPACE_OCCLUSION)
            light.color *= aoFactor.directAmbientOcclusion;
        #endif
            ScleraLayerBase(inputData, brdfData, scleraNormal, diffuseGIContribution, light, coloredAO, additionalScleraDiffuse, additionalScleraSpecular);
            IrisLayerBase(inputData, brdfData, irisNormal, diffuseGIContribution, light, coloredAO, additionalIrisDiffuse, additionalIrisSpecular);

            lightingData.additionalLightsColor += lerp(additionalIrisDiffuse, additionalScleraDiffuse, mask) + additionalScleraSpecular * specOcc;
        }
    }
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
        #if defined(_SCREEN_SPACE_OCCLUSION)
            light.color *= aoFactor.directAmbientOcclusion;
        #endif
            ScleraLayerBase(inputData, brdfData, scleraNormal, diffuseGIContribution, light, coloredAO, additionalScleraDiffuse, additionalScleraSpecular);
            IrisLayerBase(inputData, brdfData, irisNormal, diffuseGIContribution, light, coloredAO, additionalIrisDiffuse, additionalIrisSpecular);

            lightingData.additionalLightsColor += lerp(additionalIrisDiffuse, additionalScleraDiffuse, mask) + additionalScleraSpecular * specOcc;
        }
    LIGHT_LOOP_END
#endif

    #if defined(_ADDITIONAL_LIGHTS_VERTEX)
    lightingData.vertexLightingColor += inputData.vertexLighting * brdfData.diffuse;
    #endif
    
    half4 color = CalculateFinalColor(lightingData, surfaceData.alpha);
    
    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    return color;
}
