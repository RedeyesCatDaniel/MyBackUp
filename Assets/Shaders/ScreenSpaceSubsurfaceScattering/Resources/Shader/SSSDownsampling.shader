Shader "Hidden/Universal Render Pipeline/SSSDownsampling"
{
    HLSLINCLUDE

    //-------------------------------------------------------------------------------------
    // Include
    //-------------------------------------------------------------------------------------
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

    // ----------------------------------------------------
    //Defines

    //NOTE: These values should be 1-1 with what is defined in SssConstants.
    //      Typically done with code generation but for now, manually set and placed here.
    #define SSS_N_PROFILES           (8)
    #define SSS_NEUTRAL_PROFILE_ID   (7)
    #define SSS_LOD_THRESHOLD        (4)
    #define SSS_TRSM_MODE_NONE       (0)
    #define SSS_TRSM_MODE_THIN       (1)
    #define SSS_BASIC_N_SAMPLES      (25)
    #define SSS_BASIC_DISTANCE_SCALE (3)

    #define SSS_N_SAMPLES_NEAR_FIELD (55)
    #define SSS_N_SAMPLES_FAR_FIELD  (21)

    // Tweak parameters for the Disney SSS below.
    #define SSS_BILATERAL_FILTER  1
    #define SSS_CLAMP_ARTIFACT    0
    #define SSS_DEBUG_LOD         0
    #define SSS_DEBUG_NORMAL_VS   0

    // Do not modify these.
    #define SSS_USE_TANGENT_PLANE 0 //Currenty don't have access to WS normals for this to function.
    #define SSS_PASS              1
    #define MILLIMETERS_PER_METER 1000
    #define CENTIMETERS_PER_METER 100

    //-------------------------------------------------------------------------------------
    // Inputs & outputs
    //-------------------------------------------------------------------------------------


    //----------------------------------------------------
    //Inputs

    float4 _ShapeParams[SSS_N_PROFILES];        // RGB = S = 1 / D, A = filter radius
    float  _WorldScales[SSS_N_PROFILES];                                         // Size of the world unit in meters

#if SHADER_API_MOBILE
    half  _FilterKernelsFarField[SSS_N_PROFILES][SSS_N_SAMPLES_FAR_FIELD][2];   // 0 = radius, 1 = reciprocal of the PDF
#else
    half  _FilterKernelsNearField[SSS_N_PROFILES][SSS_N_SAMPLES_NEAR_FIELD][2]; // 0 = radius, 1 = reciprocal of the PDF
    half  _FilterKernelsFarField[SSS_N_PROFILES][SSS_N_SAMPLES_FAR_FIELD][2];   // 0 = radius, 1 = reciprocal of the PDF
#endif


    TEXTURE2D_X(_IrradianceSource);             // Includes transmitted light
    SAMPLER(sampler_IrradianceSource);

    TEXTURE2D_X(_SSSParams);
    SAMPLER(sampler_SSSParams);

    TEXTURE2D_X(_SSSSpecular);
    SAMPLER(sampler_SSSSpecular);

    TEXTURE2D_X(_SSSAlbedo);
    SAMPLER(sampler_SSSAlbedo);


    struct Attributes
    {
        float4 positionHCS   : POSITION;
        float2 uv           : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4  positionCS  : SV_POSITION;
        float2  uv          : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        // Note: The pass is setup with a mesh already in CS
        // Therefore, we can just output vertex position
        output.positionCS = float4(input.positionHCS.xyz, 1.0);

        #if UNITY_UV_STARTS_AT_TOP
                output.positionCS.y *= -1;
        #endif

        output.uv = input.uv;

        // Add a small epsilon to avoid artifacts when reconstructing the normals
        output.uv += 1.0e-6;

        return output;
    }


    //-------------------------------------------------------------------------------------
    // Implementation
    //-------------------------------------------------------------------------------------

    inline float LinearEyeDepth01(float z)
    {
        
#if UNITY_REVERSED_Z
        return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
#else
        return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
#endif
    }

    // Computes the value of the integrand over a disk: (2 * PI * r) * KernelVal().
    // N.b.: the returned value is multiplied by 4. It is irrelevant due to weight renormalization.
    float3 KernelValCircle(float r, float3 S)
    {
        float3 expOneThird = exp(((-1.0 / 3.0) * r) * S);
        return /* 0.25 * */ S * (expOneThird + expOneThird * expOneThird * expOneThird);
    }

    // Computes F(r)/P(r), s.t. r = sqrt(a^2 + b^2).
    // Rescaling of the PDF is handled by 'totalWeight'.
    float3 ComputeBilateralWeight(float a2, float b, float mmPerUnit, float3 S, float rcpPdf)
    {
#if (SSS_BILATERAL_FILTER == 0)
        b = 0;
#endif

#if SSS_USE_TANGENT_PLANE
        // Both 'a2' and 'b2' require unit conversion.
        float r = sqrt(a2 + b * b) * mmPerUnit;
#else
        // Only 'b2' requires unit conversion.
        float r = sqrt(a2 + (b * mmPerUnit) * (b * mmPerUnit));
#endif

#if SSS_CLAMP_ARTIFACT
        return saturate(KernelValCircle(r, S) * rcpPdf);
#else
        return KernelValCircle(r, S) * rcpPdf;
#endif
    }

    float RawToLinearDepth(float rawDepth)
    {
#if defined(_ORTHOGRAPHIC)

        return ((_ProjectionParams.z - _ProjectionParams.y) * (rawDepth)+_ProjectionParams.y);
#else
        return LinearEyeDepth(rawDepth, _ZBufferParams);
#endif
    }

    float SampleAndGetLinearDepth(float2 uv)
    {

        float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;;
#if UNITY_REVERSED_Z
        return LinearEyeDepth(rawDepth, _ZBufferParams);
#else
        return LinearEyeDepth((1 - rawDepth), _ZBufferParams);
#endif

    }

    float3 ViewSpacePosition(float2 positionNDC, float deviceDepth, float4x4 invProjMatrix)
    {
        float4 positionCS = ComputeClipSpacePosition(positionNDC, deviceDepth);
        #if !UNITY_UV_STARTS_AT_TOP
            positionCS.y =  - positionCS.y;
        #endif

        float4 positionVS = mul(invProjMatrix, positionCS);
        // The view space uses a right-handed coordinate system.
        positionVS.z = -positionVS.z;
        return positionVS.xyz / positionVS.w;
    }


    float4 Frag(Varyings input) : SV_Target
    {

        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        half2 SSPosition = input.uv;

        half4 params = SAMPLE_TEXTURE2D_X(_SSSParams, sampler_SSSParams, UnityStereoTransformScreenSpaceTex(SSPosition));

        int   profileID = UnpackByte(params.b);
        profileID = max(0, profileID);
        profileID = min(7, profileID);
        half  distScale = params.r;

        half3 shapeParam = _ShapeParams[profileID].rgb;
        half  maxDistance = _ShapeParams[profileID].a;

        // Take the first (central) sample.
        // TODO: copy its neighborhood into LDS.
        half2 centerPosition = SSPosition;

        half4 centerSamp = SAMPLE_TEXTURE2D_X(_IrradianceSource, sampler_IrradianceSource, UnityStereoTransformScreenSpaceTex(SSPosition));
        half3 centerIrradiance = centerSamp.rgb;
        //centerIrradiance = sqrt(centerIrradiance);

        // Reconstruct the view-space position.
        half2 centerPosSS = SSPosition;

        half2 cornerPosSS = centerPosSS + 0.5 * (_ScreenParams.zw - 1.0);
        half  centerDepth = SAMPLE_TEXTURE2D_X (_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(centerPosition)).r;

        half3 centerPosVS = ViewSpacePosition(centerPosSS, (centerDepth), _InvProjMatrix);
        half3 cornerPosVS = ViewSpacePosition(cornerPosSS, (centerDepth), _InvProjMatrix);

        // Rescaling the filter is equivalent to inversely scaling the world.
        half mmPerUnit = MILLIMETERS_PER_METER * (_WorldScales[profileID] / distScale);
        half unitsPerMm = rcp(mmPerUnit);
        
        // Compute the view-space dimensions of the pixel as a quad projected onto geometry.
        half2 unitsPerPixel = 2 * abs(cornerPosVS.xy - centerPosVS.xy);
        half2 pixelsPerMm = rcp(unitsPerPixel) * unitsPerMm;


        // We perform point sampling. Therefore, we can avoid the cost
        // of filtering if we stay within the bounds of the current pixel.
        // We use the value of 1 instead of 0.5 as an optimization.
        // N.b.: our LoD selection algorithm is the same regardless of
        // whether we integrate over the tangent plane or not, since we
        // don't want the orientation of the tangent plane to create
        // divergence of execution across the warp.
        half maxDistInPixels = maxDistance * max(pixelsPerMm.x, pixelsPerMm.y);

        clip(maxDistInPixels - 1.0);
        clip(distScale - 0.01);
        
        pixelsPerMm *= (_ScreenParams.zw - 1.0);
        #if SHADER_API_MOBILE
            pixelsPerMm *= 2.0;
        #endif
   
        //return half4(pixelsPerMm *10, 0, 1);
        const bool useTangentPlane = SSS_USE_TANGENT_PLANE != 0;

        // Compute the tangent frame in view space.
        half3 normalVS = float3(0,0,0);//mul((float3x3)_ViewMatrix, bsdfData.normalWS);
        half3 tangentX = float3(0,0,0);//GetLocalFrame(normalVS)[0] * unitsPerMm;
        half3 tangentY = float3(0,0,0);//GetLocalFrame(normalVS)[1] * unitsPerMm;

        #if SSS_DEBUG_NORMAL_VS
            // We expect the view-space normal to be front-facing.
            //if (normalVS.z >= 0) return float4(1, 0, 0, 1);
        #endif

        // Accumulate filtered irradiance and bilateral weights (for renormalization).
        float3 totalIrradiance, totalWeight = 0.0;

#if SHADER_API_MOBILE
        {

            half  centerRadius = _FilterKernelsFarField[profileID][0][0];
            half  centerRcpPdf = _FilterKernelsFarField[profileID][0][1];

            half3 centerWeight = KernelValCircle(centerRadius, shapeParam) * centerRcpPdf;

            totalIrradiance = centerWeight * centerIrradiance;
            totalWeight = centerWeight;

            /* Integrate over the screen-aligned or tangent plane in the view space. */
            uint n = SSS_N_SAMPLES_FAR_FIELD;

            [unroll]
            for (uint i = 0; i < n; i++)
            {
                half  r = _FilterKernelsFarField[profileID][i][0];

                /* The relative sample position is known at compile time. */
                half  phi = SampleDiskFibonacci(i, n).y;
                half2 vec = r * float2(cos(phi), sin(phi));

                /* Compute the screen-space position and the associated irradiance. */
                half2 position; float3 irradiance;
                /* Compute the squared distance (in mm) in the screen-aligned plane. */
                half dXY2;
                //return half4(1, 1, 0.5, 1);

                /* 'vec' is given directly in screen-space. */
                position = centerPosition + vec * pixelsPerMm;
                //return half4(position, 0, 1);
                half4 samp = SAMPLE_TEXTURE2D_X(_IrradianceSource, sampler_IrradianceSource, UnityStereoTransformScreenSpaceTex(position));
                irradiance = samp.rgb;
                dXY2 = r * r;

                /* TODO: see if making this a [branch] improves performance. */
                //[flatten]                                                                       
                if (any(irradiance))
                {
                    /* Apply bilateral weighting. */
                    half z = samp.a;
                    
                    half d = LinearEyeDepth01(z);
                    half t = d - LinearEyeDepth01(centerSamp.a);
                    half  p = _FilterKernelsFarField[profileID][i][1];

                    half3 w = ComputeBilateralWeight(dXY2, t, mmPerUnit, shapeParam, p);

                    w = max(w, 0.0001);

                    totalIrradiance += w * irradiance;
                    totalWeight += w;
                }
                else
                {
                    //return float4(1, 0, 0, 1);
                }

            }

        }

#else
        // Use fewer samples for SS regions smaller than 5x5 pixels (rotated by 45 degrees).
        [branch]
        if (maxDistInPixels < SSS_LOD_THRESHOLD)
        {
            #if SSS_DEBUG_LOD
                return float4(0.5, 0.5, 0, 1);
            #else
            {
                half  centerRadius = _FilterKernelsFarField[profileID][0][0];
                half  centerRcpPdf = _FilterKernelsFarField[profileID][0][1];

                half3 centerWeight = KernelValCircle(centerRadius, shapeParam) * centerRcpPdf;
                    
                totalIrradiance = centerWeight * centerIrradiance;                              
                totalWeight = centerWeight;                                                 
                    
                /* Integrate over the screen-aligned or tangent plane in the view space. */     
                uint n = SSS_N_SAMPLES_FAR_FIELD;

                [unroll]                                                                        
                for (uint i = 0; i <  n; i++)
                {                                                                                     
                    half  r = _FilterKernelsFarField[profileID][i][0];

                    /* The relative sample position is known at compile time. */                    
                    half  phi = SampleDiskFibonacci(i, n).y;
                    half2 vec = r * float2(cos(phi), sin(phi));
                                
                    /* Compute the screen-space position and the associated irradiance. */          
                    half2 position; float3 irradiance;
                    /* Compute the squared distance (in mm) in the screen-aligned plane. */         
                    half dXY2;
                    //return half4(1, 1, 0.5, 1);
                                                                                
                    /* 'vec' is given directly in screen-space. */                              
                    position = centerPosition + vec * pixelsPerMm;
                    half4 samp = SAMPLE_TEXTURE2D_X(_IrradianceSource, sampler_IrradianceSource, UnityStereoTransformScreenSpaceTex(position));
                    irradiance = samp.rgb;
                    dXY2 = r * r;      

                    /* TODO: see if making this a [branch] improves performance. */                 
                    //[flatten]                                                                       
                    if (any(irradiance))                                                            
                    {                                                                               
                        /* Apply bilateral weighting. */                                           
                        half z = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(position)).r;
                        //half z = samp.a;
                        half d = LinearEyeDepth01(z);
                        half t = d - centerPosVS.z;
                        half  p = _FilterKernelsFarField[profileID][i][1];
                        half3 w = ComputeBilateralWeight(dXY2, t, mmPerUnit, shapeParam, p);

                        w = max(w, 0.0001);

                        totalIrradiance += w * irradiance;                                          
                        totalWeight += w;                                                       
                    }                                                                               
                    else                                                                            
                    {                                                                               
                        //return float4(1, 0, 0, 1);
                    }                                                                               
                    
                }                   

            }
            #endif
        }
        else
        {
            #if SSS_DEBUG_LOD
                return float4(1, 0, 0, 1);
            #else
            half  centerRadius = _FilterKernelsNearField[profileID][0][0];
            half  centerRcpPdf = _FilterKernelsNearField[profileID][0][1];

            half3 centerWeight = KernelValCircle(centerRadius, shapeParam) * centerRcpPdf;

            totalIrradiance = centerWeight * centerIrradiance;
            totalWeight = centerWeight;
       
            /* Integrate over the screen-aligned or tangent plane in the view space. */
            uint n = SSS_N_SAMPLES_NEAR_FIELD;

            [unroll]
            for (uint i = 0; i < n ; i ++)
            {
                #if SHADER_API_MOBILE
                    half  r = _FilterKernelsFarField[profileID][i][0];
                #else
                    half  r = _FilterKernelsNearField[profileID][i][0];
                #endif

                /* The relative sample position is known at compile time. */
                half  phi = SampleDiskFibonacci(i, n).y;
                half2 vec = r * float2(cos(phi), sin(phi));

                /* Compute the screen-space position and the associated irradiance. */
                half2 position; float3 irradiance;
                /* Compute the squared distance (in mm) in the screen-aligned plane. */
                half dXY2;

                /* 'vec' is given directly in screen-space. */
                position = centerPosition + vec * pixelsPerMm;
                
                half4 samp = SAMPLE_TEXTURE2D_X(_IrradianceSource, sampler_IrradianceSource, UnityStereoTransformScreenSpaceTex(position));
                irradiance = samp.rgb;
                dXY2 = r * r;

                /* TODO: see if making this a [branch] improves performance. */
                    //[flatten]                                                                       
                if (any(irradiance))
                {
                    /* Apply bilateral weighting. */
                    half z = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(position)).r;
                    //half z = samp.a;
                    half d = LinearEyeDepth01(z);
                    half t = d - centerPosVS.z;
                    half  p = _FilterKernelsNearField[profileID][i][1];

                    half3 w = ComputeBilateralWeight(dXY2, t, mmPerUnit, shapeParam, p);
                    w = max(w, 0.0001);

                    totalIrradiance += w * irradiance;
                    totalWeight += w;
                }
                else
                {
                    //return float4(1, 0, 0, 1);
                }
            }
 
            #endif
            
        }
#endif
    
            float3 specular = SAMPLE_TEXTURE2D_X(_SSSSpecular, sampler_SSSSpecular, UnityStereoTransformScreenSpaceTex(centerPosition)).rgb;
            float3 albedo = SAMPLE_TEXTURE2D_X(_SSSAlbedo, sampler_SSSAlbedo, UnityStereoTransformScreenSpaceTex(centerPosition)).rgb;

            return float4((totalIrradiance / totalWeight) * albedo + specular, 1);
    }

    ENDHLSL

    SubShader
    {

        Pass
        {
            Stencil
            {
                Ref  1 // StencilBits.SSS
                Comp Equal
                Pass Keep
            }

            Cull   Off
            ZTest  Always
            ZWrite Off
            Blend  One Zero

            HLSLPROGRAM
            #pragma target 4.0
            //#pragma only_renderers d3d11 ps4 metal // TEMP: until we go further in dev
            // #pragma enable_d3d11_debug_symbols
            
            #pragma vertex   Vert
            #pragma fragment Frag

            ENDHLSL
        }

        Pass
        {
            Stencil
            {
                Ref  1 // StencilBits.SSS
                Comp Equal
                Pass Keep
            }

            Cull   Off
            ZTest  Always
            ZWrite Off
            Blend  One Zero

            HLSLPROGRAM
            #pragma target 4.0
            //#pragma only_renderers d3d11 ps4 metal // TEMP: until we go further in dev
            // #pragma enable_d3d11_debug_symbols

            #pragma shader_feature_local _PARTIALDOWNSAMPLE

            #pragma vertex   Vert
            #pragma fragment Frag


            ENDHLSL
        }

    }
}
