#ifndef UNIVERSAL_HAIR_SURFACE_DATA_INCLUDED
#define UNIVERSAL_HAIR_SURFACE_DATA_INCLUDED

// Must match Universal ShaderGraph master node
struct HairSurfaceData
{
	// half3   albedo;
	// half	occlusion;
	// half3	emission;
	// half    alpha;
	half3   normalWS;
	half3	geomNormalWS;
	// half    smoothness;
	half3   transmittance;
	half    rimTransmissionIntensity;
	half3	hairStrandDirection;
	half    secondarySmoothness;
	half3   specularTint;
	half3   secondarySpecularTint;
	half    specularShift;
	half    secondarySpecularShift;
};

void BuildSurfaceData(SurfaceDescription surfaceDescription, InputData inputData, half alpha, out HairSurfaceData hairSurfaceData)
{
	ZERO_INITIALIZE(HairSurfaceData, hairSurfaceData);

	// surfaceData.albedo						= surfaceDescription.BaseColor;
	// surfaceData.smoothness					= saturate(surfaceDescription.Smoothness),
	// surfaceData.occlusion					= surfaceDescription.Occlusion,
	// surfaceData.emission					= surfaceDescription.Emission,
	// surfaceData.alpha						= saturate(alpha),
	hairSurfaceData.transmittance				= surfaceDescription.Transmittance,
	hairSurfaceData.rimTransmissionIntensity	= surfaceDescription.RimTransmissionIntensity,

	hairSurfaceData.specularTint				= surfaceDescription.SpecularTint,
	hairSurfaceData.specularShift				= surfaceDescription.SpecularShift,

	hairSurfaceData.secondarySmoothness			= surfaceDescription.SecondarySmoothness,
	hairSurfaceData.secondarySpecularTint		= surfaceDescription.SecondarySpecularTint,
	hairSurfaceData.secondarySpecularShift		= surfaceDescription.SecondarySpecularShift,

#if _USE_LIGHT_FACING_NORMAL
	surfaceData.normalWS						= ComputeViewFacingNormal(inputData.viewDirectionWS, surfaceData.hairStrandDirection);
#else
	hairSurfaceData.normalWS					= inputData.normalWS; // with normal map
#endif
	hairSurfaceData.geomNormalWS				= inputData.tangentToWorld[2];
	hairSurfaceData.hairStrandDirection         = normalize(TransformTangentToWorld(surfaceDescription.HairStrandDirection, inputData.tangentToWorld));
}
#endif
