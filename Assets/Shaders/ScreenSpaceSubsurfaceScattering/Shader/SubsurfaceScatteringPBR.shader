// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "SubsurfaceScattering/SubsurfaceScatteringPBR"
{
	Properties
	{

		[MainTexture] _BaseMap("Albedo", 2D) = "white" {}
		[MainColor] _BaseColor("Color", Color) = (1,1,1,1)

		// Specular vs Metallic workflow
		[HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5

		_Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}
		_GlossMapScale("Smoothness Factor", Range(0.0, 1.0)) = 1.0
		[Enum(Specular Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

		_SpecColor("Specular", Color) = (0.2,0.2,0.2)
		_SpecGlossMap("Specular Map", 2D) = "white" {}

		//----------------------------------------------------------------------------------------------------
		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		_ParallaxMap("Height Map", 2D) = "black" {}

		[ToggleOff] _SpecularOcculsion("Use AO as SO", Float) = 0.0
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}
		_OcclusionColorBleeding("Color Bleeding AO", Color) = (0,0,0)

		[HDR] _EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_DetailMask("Detail Mask", 2D) = "white" {}
		[Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "linearGrey" {}
		_DetailAlbedoMapScale("Scale", Range(0.0, 2.0)) = 1.0
		_DetailNormalMapScale("Scale", Range(0.0, 2.0)) = 1.0
		[Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

		// SSS excusetive
		_SubsurfaceProfile("Subsurface Profile", Int) = 0
		_SubsurfaceRadius("Subsurface Radius", Range(0.0, 1.0)) = 1.0
		_SubsurfaceRadiusMap("Subsurface Radius Map", 2D) = "white" {}

		[HideInInspector] _TransmissionFlags("Transmission Flags", Int) = 0
		_Thickness("Thickness", Range(0.0, 1.0)) = 1.0
		_ThicknessMap("Thickness Map", 2D) = "white" {}

		//Skin rendering
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _DualSpecularLobe("Dual Specular Lobe", Float) = 1.0
		_SpecularLobeInterpolation("Lobe Interpolation", Range(0.0, 1.0)) = 0.15
		_SecondLobeRoughnessDerivation("2nd Lobe Roughness Derivation", Range(0.0, 2.0)) = 0.65

		//------------------------------------Obsolete-----------------------------------------------------------------
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_DetailSmoothnessMap("Detail Smoothness", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

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

	}

	HLSLINCLUDE
		#define SUBSURFACE_MATERIAL_SKIN
		#define UNITY_SETUP_BRDF_INPUT SpecularSetup
	ENDHLSL

	SubShader
	{

		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit"
				"IgnoreProjector" = "True" "Queue" = "Alphatest+50"}
		LOD 300

		//----------------------------------------------- Forward Pass -------------------------------------------------------------------
		Pass
		{
			Name "SubsurfaceNormalForwardLit"
			Tags { "LightMode" = "UniversalForward" }

			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			Stencil {
					Ref 1
					Comp Always
					Pass Replace
					ZFail Keep
				}


			HLSLPROGRAM
			#pragma target 4.0

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
			#pragma shader_feature_local _SPECGLOSSMAP
			#pragma shader_feature_local _SPECULAROCCLUSION
			#pragma shader_feature_local _DUOSPECULARLOBE

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

			#pragma shader_feature _SUBSURFACE_PASS


			#include "./ShaderLibrary/SubsurfaceScatteringUtility.hlsl"
			#include "./ShaderLibrary/SubsurfaceScatteringInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"


			#pragma vertex LitPassVertex
			#pragma fragment frag

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(_PARALLAXMAP)
				#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
					half3 viewDirTS = input.viewDirTS;
				#else
					half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
				#endif
					ApplyPerPixelDisplacement(viewDirTS, input.uv);
				#endif

				SurfaceData surfaceData;
				InitializeStandardLitSurfaceData(input.uv, surfaceData);

				surfaceData.clearCoatMask = 0.0;
				surfaceData.clearCoatSmoothness = 1.0;

				InputData inputData;
				InitializeInputData(input, surfaceData.normalTS, inputData);


			#ifdef _SPECULARHIGHLIGHTS_OFF
				bool specularHighlightsOff = true;
			#else
				bool specularHighlightsOff = false;
			#endif

				BRDFData brdfData;

				// NOTE: can modify alpha
				InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

				BRDFData brdfDataClearCoat ;

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
				SplitGlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
					inputData.bakedGI, surfaceData.occlusion, inputData.normalWS, inputData.viewDirectionWS,
					diffuseGIContribution, specularGIContribution);
				half3 diffuseDirectContribution;
				half3 specularDirectContribution;
				SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, mainLight,
					inputData.normalWS, inputData.viewDirectionWS, surfaceData.clearCoatMask, specularHighlightsOff,
					diffuseDirectContribution, specularDirectContribution);

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
					SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, light,
						inputData.normalWS, inputData.viewDirectionWS, surfaceData.clearCoatMask, specularHighlightsOff,
						diffuseAdditionalLightContribution, specularAdditionalLightContribution);

					diffuseDirectContribution += diffuseAdditionalLightContribution;
					specularDirectContribution += specularAdditionalLightContribution;

				}
			#endif


				half3 color = 0;

			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += inputData.vertexLighting * brdfData.diffuse;
			#endif

				color += surfaceData.emission;

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

				return half4(color, surfaceData.alpha);
			}

			ENDHLSL
		}

		Pass
		{

			Name "SubsurfaceScatteringSupportPass"
			Tags { "LightMode" = "SplitForward" "RenderType" = "Opaque" }

			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			HLSLPROGRAM
			#pragma target 4.0

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
			#pragma shader_feature_local _SPECGLOSSMAP
			#pragma shader_feature_local _SPECULAROCCLUSION
			#pragma shader_feature_local _DUOSPECULARLOBE

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

			// -------------------------------------
			// Subsurface Scattering keywords
			#pragma shader_feature _THICKNESSMAP
			#pragma shader_feature _SUBSURFACE_RADIUS_MAP
			#pragma shader_feature _POST_SCATTER
			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "./ShaderLibrary/SubsurfaceScatteringUtility.hlsl"
			#include "./ShaderLibrary/SubsurfaceScatteringInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"


			#pragma vertex LitPassVertex
			#pragma fragment frag

			void frag(Varyings input, out half4 outBuffer0 : SV_Target0, out half4 outBuffer1 : SV_Target1, out half4 outParams : SV_Target2)
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(_PARALLAXMAP)
				#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
					half3 viewDirTS = input.viewDirTS;
				#else
					half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
				#endif
					ApplyPerPixelDisplacement(viewDirTS, input.uv);
				#endif

				SurfaceData surfaceData;
				InitializeStandardLitSurfaceData(input.uv, surfaceData);

				surfaceData.clearCoatMask = 0.0;
				surfaceData.clearCoatSmoothness = 1.0;

				InputData inputData;
				InitializeInputData(input, surfaceData.normalTS, inputData);


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
				SplitGlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
					inputData.bakedGI, surfaceData.occlusion, inputData.normalWS, inputData.viewDirectionWS,
					diffuseGIContribution, specularGIContribution);
				half3 diffuseDirectContribution;
				half3 specularDirectContribution;
				SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, mainLight,
					inputData.normalWS, inputData.viewDirectionWS, surfaceData.clearCoatMask, specularHighlightsOff,
					diffuseDirectContribution, specularDirectContribution);

				half disAtten = mainLight.distanceAttenuation;
				half shadow = mainLight.shadowAttenuation;
				half NdotL = dot(inputData.normalWS, mainLight.direction);

				Transmission(diffuseDirectContribution, GetThickness(input.uv), GetSubsurfaceRadius(input.uv),
					shadow, disAtten, NdotL, mainLight.color, brdfData.diffuse);

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
					SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, light,
						inputData.normalWS, inputData.viewDirectionWS, surfaceData.clearCoatMask, specularHighlightsOff,
						diffuseAdditionalLightContribution, specularAdditionalLightContribution);

					disAtten = light.distanceAttenuation;
					NdotL = dot(inputData.normalWS, light.direction);

					Transmission(diffuseAdditionalLightContribution, GetThickness(input.uv), GetSubsurfaceRadius(input.uv),
						light.shadowAttenuation, disAtten, NdotL, light.color, brdfData.diffuse);

					diffuseDirectContribution += diffuseAdditionalLightContribution;
					specularDirectContribution += specularAdditionalLightContribution;

				}
			#endif

				half3 diffuseContribution = diffuseGIContribution + diffuseDirectContribution;
				half3 specularContribution = specularGIContribution + specularDirectContribution;

				#ifdef _EMISSION
					diffuseContribution += _EmissionColor;
				#endif
				
				
				outBuffer0 = half4(diffuseContribution, input.positionCS.z);
				outBuffer1 = half4(specularContribution, 1);
				outParams = half4(GetSubsurfaceRadius(input.uv), GetThickness(input.uv), PackByte(_SubsurfaceProfile), 0.0);

			}

			ENDHLSL
		}

		Pass
		{

			Name "SubsurfaceScatteringSupportPassDownsampling"
			Tags { "LightMode" = "SplitForwardDownsampling" "RenderType" = "Opaque" }

			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			HLSLPROGRAM
			#pragma target 4.0

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
			#pragma shader_feature_local _SPECGLOSSMAP
			#pragma shader_feature_local _SPECULAROCCLUSION
			#pragma shader_feature_local _DUOSPECULARLOBE

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

			// -------------------------------------
			// Subsurface Scattering keywords
			#pragma shader_feature _THICKNESSMAP
			#pragma shader_feature _SUBSURFACE_RADIUS_MAP
			#pragma shader_feature _POST_SCATTER
			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "./ShaderLibrary/SubsurfaceScatteringUtility.hlsl"
			#include "./ShaderLibrary/SubsurfaceScatteringInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"


			#pragma vertex LitPassVertex
			//#pragma fragment LitPassFragment
			#pragma fragment frag

			void frag(Varyings input, out half4 outBuffer0 : SV_Target0, out half4 outParams : SV_Target1)
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(_PARALLAXMAP)
				#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
					half3 viewDirTS = input.viewDirTS;
				#else
					half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
				#endif
					ApplyPerPixelDisplacement(viewDirTS, input.uv);
				#endif

				SurfaceData surfaceData;
				InitializeStandardLitSurfaceData(input.uv, surfaceData);
				surfaceData.albedo = 1;

				surfaceData.clearCoatMask = 0.0;
				surfaceData.clearCoatSmoothness = 1.0;

				InputData inputData;
				InitializeInputData(input, surfaceData.normalTS, inputData);


			
				bool specularHighlightsOff = true;

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
				SplitGlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
					inputData.bakedGI, surfaceData.occlusion, inputData.normalWS, inputData.viewDirectionWS,
					diffuseGIContribution, specularGIContribution);
				half3 diffuseDirectContribution;
				half3 specularDirectContribution;
				SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, mainLight,
					inputData.normalWS, inputData.viewDirectionWS, surfaceData.clearCoatMask, specularHighlightsOff,
					diffuseDirectContribution, specularDirectContribution);

				half disAtten = mainLight.distanceAttenuation;
				half NdotL = dot(inputData.normalWS, mainLight.direction);
				half shadow = mainLight.shadowAttenuation;

				Transmission(diffuseDirectContribution, GetThickness(input.uv), GetSubsurfaceRadius(input.uv),
					shadow, disAtten, NdotL, mainLight.color, brdfData.diffuse);

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
					SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, light,
						inputData.normalWS, inputData.viewDirectionWS, surfaceData.clearCoatMask, specularHighlightsOff,
						diffuseAdditionalLightContribution, specularAdditionalLightContribution);

					disAtten = light.distanceAttenuation;
					NdotL = dot(inputData.normalWS, light.direction);

					Transmission(diffuseAdditionalLightContribution, GetThickness(input.uv), GetSubsurfaceRadius(input.uv),
						light.shadowAttenuation, disAtten, NdotL, light.color, brdfData.diffuse);


					diffuseDirectContribution += diffuseAdditionalLightContribution;
					
				}
			#endif

				half3 diffuseContribution = diffuseGIContribution + diffuseDirectContribution;
			
				#ifdef _EMISSION
					diffuseContribution += _EmissionColor;
				#endif

				outBuffer0 = half4(diffuseContribution, input.positionCS.z);
				outParams = half4(GetSubsurfaceRadius(input.uv), GetThickness(input.uv), PackByte(_SubsurfaceProfile), 0.0);

			}

			ENDHLSL
		}

		Pass
		{

			Name "SubsurfaceScatteringSupportPassAlbedo"
			Tags { "LightMode" = "SplitForwardAlbedo" "RenderType" = "Opaque" }

			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			HLSLPROGRAM
			#pragma target 4.0

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
			#pragma shader_feature_local _SPECGLOSSMAP
			#pragma shader_feature_local _SPECULAROCCLUSION
			#pragma shader_feature_local _DUOSPECULARLOBE

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

			// -------------------------------------
			// Subsurface Scattering keywords
			#pragma shader_feature _THICKNESSMAP
			#pragma shader_feature _SUBSURFACE_RADIUS_MAP
			#pragma shader_feature _POST_SCATTER
			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "./ShaderLibrary/SubsurfaceScatteringUtility.hlsl"
			#include "./ShaderLibrary/SubsurfaceScatteringInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"


			#pragma vertex LitPassVertex
			//#pragma fragment LitPassFragment
			#pragma fragment frag

			void frag(Varyings input, out half4 outBuffer0 : SV_Target0, out half4 outParams : SV_Target1)
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(_PARALLAXMAP)
				#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
					half3 viewDirTS = input.viewDirTS;
				#else
					half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
				#endif
					ApplyPerPixelDisplacement(viewDirTS, input.uv);
				#endif

				SurfaceData surfaceData;
				InitializeStandardLitSurfaceData(input.uv, surfaceData);

				surfaceData.clearCoatMask = 0.0;
				surfaceData.clearCoatSmoothness = 1.0;

				InputData inputData;
				InitializeInputData(input, surfaceData.normalTS, inputData);


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
				SplitGlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
					inputData.bakedGI, surfaceData.occlusion, inputData.normalWS, inputData.viewDirectionWS,
					diffuseGIContribution, specularGIContribution);
				half3 diffuseDirectContribution;
				half3 specularDirectContribution;
				SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, mainLight,
					inputData.normalWS, inputData.viewDirectionWS, surfaceData.clearCoatMask, specularHighlightsOff,
					diffuseDirectContribution, specularDirectContribution);

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
					SplitLightingPhysicallyBased(brdfData, brdfDataClearCoat, light,
						inputData.normalWS, inputData.viewDirectionWS, surfaceData.clearCoatMask, specularHighlightsOff,
						diffuseAdditionalLightContribution, specularAdditionalLightContribution);

					specularDirectContribution += specularAdditionalLightContribution;

				}
			#endif

				half3 specularContribution = specularGIContribution + specularDirectContribution;

				outBuffer0 = half4(specularContribution, 1);
				outParams = half4(surfaceData.albedo, 1);

			}

			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags{"LightMode" = "ShadowCaster"}

			ZWrite On
			ZTest LEqual
			ColorMask 0
			Cull[_Cull]

			HLSLPROGRAM
			//#pragma exclude_renderers gles gles3 glcore
			#pragma target 4.0

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			#include "./ShaderLibrary/SubsurfaceScatteringInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags{"LightMode" = "DepthOnly"}

			ZWrite On
			ColorMask 0
			Cull[_Cull]

			HLSLPROGRAM
			//#pragma exclude_renderers gles gles3 glcore
			#pragma target 4.0

			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "./ShaderLibrary/SubsurfaceScatteringInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
			ENDHLSL
		}

		// This pass is used when drawing to a _CameraNormalsTexture texture
		Pass
		{
			Name "DepthNormals"
			Tags{"LightMode" = "DepthNormals"}

			ZWrite On
			Cull[_Cull]

			HLSLPROGRAM
			//#pragma exclude_renderers gles gles3 glcore
			#pragma target 4.0

			#pragma vertex DepthNormalsVertex
			#pragma fragment DepthNormalsFragment

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "./ShaderLibrary/SubsurfaceScatteringInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
			ENDHLSL
		}

	}

	CustomEditor "SubsurfaceScatteringPBRGUI"

}
