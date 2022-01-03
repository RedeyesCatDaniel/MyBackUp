// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "SubsurfaceScattering/EyePBR"
{
	Properties
	{

		[MainTexture] _BaseMap("Albedo", 2D) = "white" {}
		[MainColor] _BaseColor("Color", Color) = (1,1,1,1)

		// Specular vs Metallic workflow
		[HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

		// _Glossiness -> _Smoothness
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5

		_Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		//_GlossyReflections ->_EnvironmentReflections
		[ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0


		_DetailAlbedoMapScale("Scale", Range(0.0, 2.0)) = 1.0

		// SRP batching compatibility for Clear Coat (Not used in Lit)
		[HideInInspector] _ClearCoatMask("_ClearCoatMask", Float) = 0.0
		[HideInInspector] _ClearCoatSmoothness("_ClearCoatSmoothness", Float) = 0.0

		//----------------------------------------------------------------------------------------------------

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_GlossMapScale("Smoothness Factor", Range(0.0, 1.0)) = 1.0
		[Enum(Specular Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

		_SpecColor("Specular", Color) = (0.2,0.2,0.2)
		_SpecGlossMap("Specular Map", 2D) = "white" {}

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		_ParallaxMap("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		[HDR] _EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		[Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0


		//------------------------------------Obsolete-----------------------------------------------------------------
		_DetailSmoothnessMap("Detail Smoothness", 2D) = "white" {}

		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

		//-------------------------------------------------------------------------------------------------------------

		// Blending state
		[HideInInspector] _Surface("__surface", Float) = 0.0
		[HideInInspector] _Blend("__blend", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cull("__cull", Float) = 2.0

		[HideInInspector] _Mode("__mode", Float) = 0.0

		_ReceiveShadows("Receive Shadows", Float) = 1.0
		// Editmode props
		[HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

		// Eye
		_EyeMask("IrisMask", 2D) = "white" {}
		_EyeScleraAlbedo("ScleraAlbedo", 2D) = "white" {}
		_EyeScleraSpecularOcclusionScale("ScleraSpecularOcclusionScale", Range(0.0, 1.0)) = 1.0
		_EyeScleraSpecularOcclusion("ScleraSpecularOcclusion", 2D) = "white" {}
		_EyeScleraBumpScale("ScleraBumpScale", Float) = 1.0
		_EyeScleraBumpMap("ScleraBump", 2D) = "bump" {}

		_EyeScleraDiffuseAdditiveIntensity("ScleraDiffuseAdditiveIntensity", Range(0.0, 1.0)) = 1.0
		_EyeIrisDiffuseAdditiveIntensity("IrisDiffuseAdditiveIntensity", Range(0.0, 1.0)) = 1.0

		_EyeColorBleed("ColorBleed", Color) = (1,1,1,1)
		_EyeScleraWrap("ScleraSSS", Range(0.0, 1.0)) = 1.0
		_EyeReflectionIntensity("ReflectionIntensity", Range(0.0, 20.0)) = 1.0
		_EyeRadius("EyeRadius", Range(0.0, 1.0)) = 0.5
		_EyeAnteriorChamberDepth("AnteriorChamberDepth", Range(0.0, 5.0)) = 0.5
		_EyeIOR("EyeIOR", Range(0.0, 2.0)) = 0.5
		_EyeLimbusShift("LimbusShift", Range(-1.0, 1.0)) = 1.0
		_EyeLimbusSlope("LimbusSlope", Range(-1.0, 1.0)) = 1.0

		[PerRenderData] _EyeLookVector("LookVector", Vector) = (0, 0, 1)
		[PerRenderData] _EyeFaceVector("FaceVector", Vector) = (0, 0, 1)
		[PerRenderData] _EyeFaceVectorUp("FaceVectorUp", Vector) = (0, 1, 0)
		[PerRenderData] _EyeFaceVectorRight("FaceVectorRight", Vector) = (1, 0, 0)

		[PerRenderData] _EyeFixedTexCoordX("FixedX", Range(-1,1)) = 0
		[PerRenderData] _EyeFixedTexCoordY("FixedX", Range(-1,1)) = 0
		[PerRenderData] _EyeFixedTexCoordZ("FixedZ", Range(-1,1)) = 0
	}

	HLSLINCLUDE
	#define SUBSURFACE_MATERIAL_SKIN
	#define UNITY_SETUP_BRDF_INPUT SpecularSetup
	ENDHLSL

	SubShader
	{

		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit"
				"IgnoreProjector" = "True" "ShaderModel" = "4.5" "Queue" = "Geometry"}
		LOD 300


		//----------------------------------------------- Forward Pass -------------------------------------------------------------------
		Pass
		{
			Name "EyeForwardLit"
			Tags { "LightMode" = "UniversalForward" }

			Blend[_SrcBlend][_DstBlend]
			ZWrite On

			HLSLPROGRAM
			#pragma target 4.5

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
			#pragma shader_feature_local_fragment _EMISSION
			#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature_local_fragment _OCCLUSIONMAP
			#pragma shader_feature_local _PARALLAXMAP
			#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#pragma shader_feature_local_fragment _SPECULAR_SETUP
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING 
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON


			#include "./ShaderLibrary/EyeUtility.hlsl"
			#include "./ShaderLibrary/EyeInput.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"


			#pragma vertex LitPassVertex
			//#pragma fragment LitPassFragment
			#pragma fragment frag

			half4 frag(Varyings input) : SV_Target
			{

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
			
				SurfaceData surfaceData;
				InitializeStandardLitSurfaceData(input.uv, surfaceData);

				surfaceData.clearCoatMask = 0.0;
				surfaceData.clearCoatSmoothness = 1.0;

				InputData inputData;
				InitializeInputData(input, surfaceData.normalTS, inputData);

			#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
				input.uv = Parallax(input.uv, input.viewDirTS);
			#endif



			#ifdef _SPECULARHIGHLIGHTS_OFF
				bool specularHighlightsOff = true;
			#else
				bool specularHighlightsOff = false;
			#endif

				BRDFData brdfData;

				// NOTE: can modify alpha
				InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

				BRDFData brdfDataClearCoat = (BRDFData)0;
			#if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
				// base brdfData is modified here, rely on the compiler to eliminate dead computation by InitializeBRDFData()
				InitializeBRDFDataClearCoat(surfaceData.clearCoatMask, surfaceData.clearCoatSmoothness, brdfData, brdfDataClearCoat);
			#endif

				// To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
			#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
				half4 shadowMask = inputData.shadowMask;
			#elif !defined (LIGHTMAP_ON)
				half4 shadowMask = unity_ProbesOcclusion;
			#else
				half4 shadowMask = half4(1, 1, 1, 1);
			#endif

				Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);

			#if defined(_SCREEN_SPACE_OCCLUSION)
				AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
				mainLight.color *= aoFactor.directAmbientOcclusion;
				surfaceData.occlusion = min(surfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
			#endif

				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
				half3 diffuseGIContribution;
				half3 specularGIContribution;
					
				half3 diffuseDirectContribution;
				half3 specularDirectContribution;
					
				//------------------------------ScleraLayerBase----------------------------------------------------------

				float2 occlusionCoords = FixedTexcoords(input.uv.xy);

				half3 scleraBaseColor = SAMPLE_TEXTURE2D(_EyeScleraAlbedo, smp, input.uv.xy).rgb;

				half4 n = SAMPLE_TEXTURE2D(_EyeScleraBumpMap, smp, input.uv.xy);
				half3 scleraNormalTS = UnpackNormalScale(n, _EyeScleraBumpScale);
					
			#if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
				float sgn = input.tangentWS.w;
				float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
				half3 scleraNormal = normalize(TransformTangentToWorld(scleraNormalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz)));
			#else
				half3 scleraNormal = input.normalWS.xyz;
			#endif
					
				brdfData.diffuse = scleraBaseColor;
				SplitGlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
					inputData.bakedGI, 1.0, scleraNormal, inputData.viewDirectionWS,
					diffuseGIContribution, specularGIContribution);
					
				half3 scleraDiffuse;
				half3 scleraSpecular;

				ScleraLayerBase(inputData, brdfData, scleraBaseColor, scleraNormal, diffuseGIContribution, mainLight, occlusionCoords, scleraDiffuse, scleraSpecular);


				//---------------------------------IrisLayer-----------------------------------------------------------------

				//Apply Phycially Based Reftaction
				input.uv.xy += PhysicallyBasedRefraction(input.uv.xy, inputData.viewDirectionWS.xyz, input.normalWS.xyz);

				half irisMask = SAMPLE_TEXTURE2D(_EyeMask, smp, input.uv.xy).r;

				half3 irisBaseColor = (SAMPLE_TEXTURE2D(_BaseMap, smp, input.uv.xy).rgb) * _BaseColor.rgb;

				n = SAMPLE_TEXTURE2D(_BumpMap, smp, input.uv.xy);
				half3 irisNormalTS = UnpackNormalScale(n, _BumpScale);

			#if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
				half3 irisNormal = normalize(TransformTangentToWorld(irisNormalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz)));
			#else
				half3 irisNormal = input.normalWS.xyz;
			#endif

				//GI
				half3 irisSpecularGIContribution;

				brdfData.diffuse = irisBaseColor;
				SplitGlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
					inputData.bakedGI, 1.0, irisNormal, inputData.viewDirectionWS,
					diffuseGIContribution, irisSpecularGIContribution);

				half nv = saturate(dot(irisNormal, inputData.viewDirectionWS));
				half nl = saturate(dot(irisNormal, mainLight.direction));
				half3 h = normalize(mainLight.direction + inputData.viewDirectionWS);
				half lh = saturate(dot(mainLight.direction, h));
				half diffuseTerm = EyeDisneyDiffuse(nv, nl, lh, brdfData.perceptualRoughness) * nl;

				//Modulate the cos
				float dot1 = (nl + 0.7);
				float dot2 = (saturate(dot(mainLight.direction, _EyeLookVector)) + 0.7);
				float bump = max(dot1 / max(dot2, 1e-1), 0.0);

				diffuseGIContribution *= bump * bump;

				half3 irisDiffuse = irisBaseColor.rgb * (diffuseGIContribution + mainLight.color * diffuseTerm * mainLight.shadowAttenuation);

				half3 additionalScleraDiffuse = 0;
				half3 additionalScleraSpecular = 0;

				//Addtional light
			#ifdef _ADDITIONAL_LIGHTS
				uint pixelLightCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
				{
					Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
			#if defined(_SCREEN_SPACE_OCCLUSION)
					light.color *= aoFactor.directAmbientOcclusion;
			#endif

					ScleraLayerBase(inputData, brdfData, scleraBaseColor, scleraNormal,
						0, light, occlusionCoords, additionalScleraDiffuse, additionalScleraSpecular);

					scleraDiffuse += additionalScleraDiffuse;
					scleraSpecular += additionalScleraSpecular;

					half3 additionalIrisDiffuse;

					nv = saturate(dot(irisNormal, inputData.viewDirectionWS));
					nl = saturate(dot(irisNormal, light.direction));
					h = normalize(light.direction + inputData.viewDirectionWS);
					lh = saturate(dot(light.direction, h));
					diffuseTerm = EyeDisneyDiffuse(nv, nl, lh, brdfData.perceptualRoughness) * nl;
						
					additionalIrisDiffuse = irisBaseColor.rgb * (light.color * diffuseTerm * light.shadowAttenuation * light.distanceAttenuation);
				
					irisDiffuse += additionalIrisDiffuse;

					additionalIrisDiffuse = 0;
					additionalScleraDiffuse = 0;
					additionalScleraSpecular = 0;

				}
			#endif

			#ifndef _SPECULARHIGHLIGHTS_OFF
				//GI
				scleraSpecular += specularGIContribution;
			#else 
				scleraSpecular = specularGIContribution;
			#endif

				half3 diffuse = lerp(scleraDiffuse, irisDiffuse, Sigmoid(irisMask));
				half3 specular = scleraSpecular * (SpecularOcclusion(occlusionCoords, -inputData.viewDirectionWS));
				half4 c = half4((diffuse + specular), 1);
				//c = half4((diffuse), 1);
				//*************************************************************************************

				half3 normal = irisNormal;

				float height = _EyeAnteriorChamberDepth * saturate(1.0 - 18.4 * _EyeRadius * _EyeRadius);

				//Calculate Refraction
				float w = _EyeIOR * dot(normal, inputData.viewDirectionWS);
				float k = sqrt(1.0 + (w - _EyeIOR) * (w + _EyeIOR));
				float3 refractedW = (w - k) * normal - _EyeIOR * inputData.viewDirectionWS;

				float cosAlpha = dot(_EyeLookVector, -refractedW);
				float dist = height / cosAlpha;
				float3 offsetW = dist * refractedW;

				float2 offsetT = mul(offsetW, (float3x3)unity_ObjectToWorld).xy;

				float mask = 1 - SAMPLE_TEXTURE2D(_ParallaxMap, smp, input.uv.xy).r;

				//return  offsetT.xyxy;
				//*************************************************************************************
				return c;
					
			}

			ENDHLSL
		}

	}
	//UsePass "Universal Render Pipeline/Lit/SHADOWCASTER"
		//UsePass "Universal Render Pipeline/Lit/DEPTHNORMALS"
		//UsePass "Universal Render Pipeline/Lit/DEPTHONLY"

		FallBack "Universal Render Pipeline/Lit"
		CustomEditor "EyeShaderGUI"

}
