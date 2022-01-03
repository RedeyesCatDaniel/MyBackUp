#ifndef SUBSURFACE_SCATTERING_LIGHTING_INCLUDED
#define SUBSURFACE_SCATTERING_LIGHTING_INCLUDED

#define profileElementCnt 6

int CALC_MIP_COUNT(float2 texTexelSize) {
    return floor(log2(max(texTexelSize.x, texTexelSize.y)));
}

inline half4 PSS_SAMPLE_BLURRED_IMPL(Texture2D tex, SamplerState Sampler, float2 uv, half blur, int mipLevels) {

    // NB: decided not to use GetDimensions in SM>=4 because it's a pain AND it's inefficent because of an
    // additional memory access.
    // If you're looking for things to optimize, you may want to use only one dimension like in SM or
    // precalculate it to get rid of log2 entirely.
//			int mipLevels = floor(log2(max(texTexelSize.z, texTexelSize.w)));

    // Use derivatives of texture coordinates to get approximate mipmap lod to use.
    // Can only be used in a fragment shader.
    // TODO use CalculateLevelOfDetail/CalculateLevelOfDetailUnclamped in SM5?
    float2 dx_vtc = ddx(uv);
    float2 dy_vtc = ddy(uv);
    float delta_max_sqr = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
    float mipLod = -0.5 * log2(delta_max_sqr);

    float selLod = min(blur * mipLevels, mipLod);

    return SAMPLE_TEXTURE2D_LOD(tex, Sampler, uv.xy,  mipLevels - selLod);
}

// lightDir should be normalized
void SubsurfaceScattering_float(half3x3 tangentSpaceTransform, float2 texelSize, half3 lightDir, half3 normalWS, half3 SSSFalloff,
    half radius, half blur, half shadow, Texture2D lut, SamplerState Sampler, float2 uv, out half3 Out)
{
    half3 weights = 0.0;
    half scattering = 0.0;
    half NdotL = 0.0;
    half diffLookup = 0.0;
    half diffuseDirect = 0.0;
    half3 directLight = 0.0;
    half penumbraMip = (1.0 - shadow) * 6.0;
    half3 worldNormal = normalWS;

    /////////////////////////////////////////////////////////////////////
    //	Skin Profile
    /////////////////////////////////////////////////////////////////////

    half3 c = SSSFalloff.xyz;

    // Modified using Color Tint with weight from the highest color value of the human skin profile
    half3 profileWeights[6] = {
    (1 - c) * 0.649,
    (1 - c) * 0.366,
    c * 0.198,
    c * 0.113,
    c * 0.358,
    c * 0.078 };

    const half profileVariance[6] = {
    0.0064,
    0.0484,
    0.187,
    0.567,
    1.99,
    7.41 };

    const half profileVarianceSqrt[6] = {
    0.08,	    // sqrt(0.0064)
    0.219,	    // sqrt(0.0484)
    0.432,	    // sqrt(0.187)
    0.753,	    // sqrt(0.567)
    1.410,	    // sqrt(1.99)
    2.722 };	// sqrt(7.41)

    // Originally calculates texture mip in vertex program, 
    // However, the mip count can be calculate in the shader editor and caches it in property
    int BumpTexMipCnts = CALC_MIP_COUNT(texelSize);

    half r = 1.0 / radius; // 1 / r
    half s = -r * r;
    for (int profileInd = 0; profileInd < profileElementCnt; profileInd++)
    {
        weights = profileWeights[profileInd];
        scattering = exp(s / profileVarianceSqrt[profileInd]);

#ifdef _NORMALMAP
        // blur normal map
        worldNormal = UnpackNormal(PSS_SAMPLE_BLURRED_IMPL(_BumpMap, Sampler, uv, lerp(blur, 0.0, profileVariance[profileInd]), BumpTexMipCnts));
        worldNormal = TransformTangentToWorld(worldNormal, tangentSpaceTransform);
#endif
        // diffuse
        NdotL = dot(worldNormal, lightDir);
        diffLookup = NdotL * 0.5 + 0.5;

        diffuseDirect = SAMPLE_TEXTURE2D_LOD(lut, Sampler, float2(diffLookup, scattering), penumbraMip).r;

        directLight += weights * diffuseDirect;
    }
    /// SSS 
    Out = directLight;
}

half3 SubsurfaceScattering(half3x3 tangentSpaceTransform, half3 lightDir, half3 normalWS, half3 SSSFalloff,
    half radius, half blur, half shadowAtten, half distanceAtten, float2 uv)
{
    
#ifdef _NORMALMAP
    float2 texelSize = _BumpMap_TexelSize.zw;
#else
    float2 texelSize = float2(1024.0, 1024.0);
#endif

    half3 OutColor;

    SubsurfaceScattering_float( tangentSpaceTransform, texelSize, lightDir, normalWS, SSSFalloff,
        radius, blur, shadowAtten,
    // hard coded for texture variable name        
        _SurfaceDescriptionLookUpTexture_LookUpTexture_0, sampler_SurfaceDescriptionLookUpTexture_LookUpTexture_0, uv, 
        OutColor);

    return OutColor * distanceAtten;
   
}

// Reference based on <<Real-Time Realistic Skin Translucency>> paper,
// See http://www.iryoku.com/translucency/
// Also Next-Generation-Character-Rendering-v6.ppt #182
half3 Transmittance(float thickness, half shadowAtten, half distanceAtten, float NdotL,
    half3 transmissionColor, half3 scatteringColor)
{
    half3 c = scatteringColor;
    half s = exp(-thickness * thickness);

    // Simplified version of Profile
    half3 translucencyProfile = s * (transmissionColor * scatteringColor);

    half irradiance = max(0.0, 0.3 - NdotL);

    // Allow some light bleeding through the shadow 
    half shadow = saturate(0.02 + shadowAtten);
    return  translucencyProfile * (irradiance * distanceAtten * shadow);
}

#endif