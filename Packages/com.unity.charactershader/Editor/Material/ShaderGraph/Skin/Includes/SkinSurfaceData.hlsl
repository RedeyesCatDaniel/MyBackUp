#ifndef UNIVERSAL_SKINSURFACE_DATA_INCLUDED
#define UNIVERSAL_SKINSURFACE_DATA_INCLUDED

// Must match Universal ShaderGraph master node
struct SkinSurfaceData
{
    // half3 albedo;
    // half3 specular;
    // half  metallic;
    // half  smoothness;
    // half3 normalTS;
    // half3 emission;
    // half  occlusion;
    // half  alpha;
    half3 scatteringColor;
    half  subsurfaceMask;
    half  thickness;
    half3 transmission;
    half  lobeMix;
    half  smoothnessB;
    float2 uv;
};

#endif
