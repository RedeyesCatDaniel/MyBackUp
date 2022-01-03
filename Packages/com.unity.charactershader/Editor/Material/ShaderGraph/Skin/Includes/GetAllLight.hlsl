#ifndef UNIVERSAL_MAIN_AND_ADDITIONAL_LIGHTS_INCLUDED
#define UNIVERSAL_MAIN_AND_ADDITIONAL_LIGHTS_INCLUDED


void MainLight_float(float3 positionWS, half4 shadowMask, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction       = half3(0.5, 0.5, 0);
    Color           = half3(1, 1, 1);
    DistanceAtten   = 1;
    ShadowAtten     = 1;
#else
    half4 shadowCoord = TransformWorldToShadowCoord(positionWS);

    Light mainLight     = GetMainLight(shadowCoord, positionWS, shadowMask);
    Direction           = mainLight.direction;
    Color               = mainLight.color;
    DistanceAtten       = mainLight.distanceAttenuation;
    ShadowAtten         = mainLight.shadowAttenuation;
#endif
}

void AdditionalLights_float(float3 positionWS, half4 shadowMask, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction       = half3(0.5, 0.5, 0);
    Color           = half3(1,1,1);
    DistanceAtten   = 1;
    ShadowAtten     = 1;
#else

    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light     = GetAdditionalLight(lightIndex, positionWS, shadowMask);
        Direction       = light.direction;
        Color           = light.color;
        DistanceAtten   = light.distanceAttenuation;
        ShadowAtten     = light.shadowAttenuation;
    }
#endif
}

void AllLights_float(float3 positionWS, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction       = float3(0.5, 0.5, 0);
    Color           = half3(1, 1, 1);
    DistanceAtten   = 1;
    ShadowAtten     = 1;
#else

    #if !defined (LIGHTMAP_ON)
        half4 shadowMask = unity_ProbesOcclusion;
    #else
        half4 shadowMask = half4(1, 1, 1, 1);
    #endif

    //#ifndef _ADDITIONAL_LIGHTS
        // Main Light
        MainLight_float(positionWS, shadowMask, Direction, Color, DistanceAtten, ShadowAtten);
#ifdef _ADDITIONAL_LIGHTS
    //#else
        // Additional Lights
        AdditionalLights_float(positionWS, shadowMask, Direction, Color, DistanceAtten, ShadowAtten);
    #endif
#endif

}

#endif