using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SubsurfaceScatteringRenderFeatureComputeShader : ScriptableRendererFeature
{
    //--------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------- Variables -------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------

    // Public Properties

    // GUI Interface
    [Serializable]
    public class RenderObjectsSettings
    {
        [SerializeField] public SubsurfaceScatteringProfile[] profiles = new SubsurfaceScatteringProfile[7];
        public LayerMask layer;
    }

    [HideInInspector]
    public RenderObjectsSettings settings = new RenderObjectsSettings();

    // GUI Signals
    public bool changeProfileFromGUI = false;
    public static bool changeGUIFromProfilelist = false;
    public static bool needToUpdateProfile = true;

    public SubsurfaceScatteringProfileManager profileManager;
    public SubsurfaceScatteringProfile[] m_profiles = new SubsurfaceScatteringProfile[7];

    //Public Setting Property
    public static LayerMask m_SubsurfaceLayer;

    [HideInInspector]
    public LayerMask SubsurfaceLayer;

    [Reload("SubsurfaceScattering")]
    public ComputeShader sssComputeShader;
    public static ComputeShader m_sssComputeShader;

    public static class Uniforms
    {
        //buffers
        internal static readonly int _SSSParams = Shader.PropertyToID("_SSSParams");
        internal static readonly int _SSSDiffColor = Shader.PropertyToID("_SSSDiffColor");
        internal static readonly int _SSSDiffuse = Shader.PropertyToID("_SSSDiffuse");
        internal static readonly int _SSSSpecular = Shader.PropertyToID("_SSSSpecular");
        internal static readonly int _SSSFilter = Shader.PropertyToID("_SSSFilter");

        internal static readonly int _EncodedLighting = Shader.PropertyToID("_EncodedLighting");
        internal static readonly int _IrradianceSource = Shader.PropertyToID("_IrradianceSource");

        //matrices
        internal static readonly int _ViewMatrix = Shader.PropertyToID("_ViewMatrix");
        internal static readonly int _ProjMatrix = Shader.PropertyToID("_ProjMatrix");
        internal static readonly int _InvProjMatrix = Shader.PropertyToID("_InvProjMatrix");

        //subsurface data
        internal static readonly int _TexturingModeFlags = Shader.PropertyToID("_TexturingModeFlags");
        internal static readonly int _TransmissionFlags = Shader.PropertyToID("_TransmissionFlags");
        internal static readonly int _ThicknessRemaps = Shader.PropertyToID("_ThicknessRemaps");
        internal static readonly int _ShapeParams = Shader.PropertyToID("_ShapeParams");
        internal static readonly int _TransmissionTints = Shader.PropertyToID("_TransmissionTints");
        internal static readonly int _ColorBleedAOs = Shader.PropertyToID("_ColorBleedAOs");
        internal static readonly int _WorldScales = Shader.PropertyToID("_WorldScales");
        internal static readonly int _FilterKernelsNearField = Shader.PropertyToID("_FilterKernelsNearField");
        internal static readonly int _FilterKernelsFarField = Shader.PropertyToID("_FilterKernelsFarField");
        internal static readonly int _FilterKernelsBasic = Shader.PropertyToID("_FilterKernelsBasic");
        internal static readonly int _HalfRcpWeightedVariances = Shader.PropertyToID("_HalfRcpWeightedVariances");

    }

    //--------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------- Passes ----------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------- Pass 1 : Decode Lighting Pass -----------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------

    class SplitLightingRenderPass : ScriptableRenderPass
    {
        //Render seperate diffuse and specular lighting 
        RenderTexture diffuseRT;
        RenderTexture specularRT;
        RenderTexture paramRT;
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {

            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                //Update material properties per frame
                cmd.BeginSample("DirectSplitLighting");

                var settings = SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings;

                cmd.SetGlobalVectorArray(Uniforms._ShapeParams, settings.useDisneySSS ? settings.shapeParams : settings.halfRcpWeightedVariances);
                cmd.SetGlobalVectorArray(Uniforms._TransmissionTints, settings.transmissionTints);
                cmd.SetGlobalFloatArray(Uniforms._ThicknessRemaps, settings.thicknessRemaps);

                RenderTextureDescriptor opaqueDesc = cameraTextureDescriptor;
                opaqueDesc.colorFormat = RenderTextureFormat.ARGB2101010;
                //opaqueDesc.colorFormat = RenderTextureFormat.Default;

                opaqueDesc.vrUsage = cameraTextureDescriptor.vrUsage;
                opaqueDesc.depthBufferBits = cameraTextureDescriptor.depthBufferBits;

                diffuseRT = RenderTexture.GetTemporary(opaqueDesc);
                opaqueDesc.depthBufferBits = 0;
                specularRT = RenderTexture.GetTemporary(opaqueDesc);
                paramRT = RenderTexture.GetTemporary(opaqueDesc);
                paramRT.enableRandomWrite = true;
                paramRT.Create();

                ConfigureTarget(new RenderTargetIdentifier[] { diffuseRT, specularRT, paramRT }, Shader.PropertyToID("_CameraDepthTexture"));
                ConfigureClear(ClearFlag.Color, Color.black);
            }

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                CommandBuffer cmd = CommandBufferPool.Get("DecodeSplitLighting");
                cmd.Clear();

                SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
                ShaderTagId m_ShaderTagId = new ShaderTagId("SplitForward");
                DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortingCriteria);

                
                int overrideMaterialPassIndex = 0;

                drawingSettings.overrideMaterial = null;
                drawingSettings.overrideMaterialPassIndex = overrideMaterialPassIndex;

                FilteringSettings m_FilteringSettings;

                RenderQueueRange renderQueueRange = RenderQueueRange.opaque;

                int layerMask = m_SubsurfaceLayer;

                m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);

                //Call "SplitForward" pass in the shader
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                cmd.EndSample("DirectSplitLighting");

                if(m_sssComputeShader != null)
                {
                    m_sssComputeShader.SetTexture(0, Uniforms._IrradianceSource, diffuseRT);
                    m_sssComputeShader.SetTexture(0, Uniforms._SSSSpecular, specularRT);
                    m_sssComputeShader.SetTexture(0, Shader.PropertyToID("_SSSBufferTexture"), paramRT);
                    //m_ResolveStencilBufferComputeShader.SetTexture(0, Shader.PropertyToID("_StencilTexture"), paramRT);
                }  

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                RenderTexture.ReleaseTemporary(diffuseRT);
                RenderTexture.ReleaseTemporary(specularRT);
                RenderTexture.ReleaseTemporary(paramRT);
            }

        }
    }

    

    //--------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------- Pass 2 : Diffusion Pass  ---------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------

    class DiffusionRenderPass : ScriptableRenderPass
    {

        RenderTexture colorTexture;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {

                var settings = SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings;

                if (m_sssComputeShader != null)
                {

                    if(colorTexture != null)
                    {
                        if(colorTexture.width != cameraTextureDescriptor.width || colorTexture.height != cameraTextureDescriptor.height)
                        {
                            colorTexture = RenderTexture.GetTemporary(cameraTextureDescriptor.width, cameraTextureDescriptor.height, 0, RenderTextureFormat.ARGB2101010, RenderTextureReadWrite.Linear);
                            //colorTexture = RenderTexture.GetTemporary(cameraTextureDescriptor.width, cameraTextureDescriptor.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                            colorTexture.enableRandomWrite = true;
                            colorTexture.name = "Diffusion Color Texture";
                            colorTexture.Create();
                        }
                    }
                    else
                    {
                        colorTexture = RenderTexture.GetTemporary(cameraTextureDescriptor.width, cameraTextureDescriptor.height, 0, RenderTextureFormat.ARGB2101010, RenderTextureReadWrite.Linear);
                        //colorTexture = RenderTexture.GetTemporary(cameraTextureDescriptor.width, cameraTextureDescriptor.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                        colorTexture.enableRandomWrite = true;
                        colorTexture.name = "Diffusion Color Texture";
                        colorTexture.Create();
                    }
                    

                    m_sssComputeShader.SetInt(Uniforms._TexturingModeFlags, (int)settings.texturingModeFlags);
                    m_sssComputeShader.SetFloats(Uniforms._ThicknessRemaps, settings.thicknessRemaps);
                    m_sssComputeShader.SetVectorArray(Uniforms._TransmissionTints, settings.transmissionTints);
                    m_sssComputeShader.SetVectorArray(Uniforms._ShapeParams, settings.shapeParams);
                    m_sssComputeShader.SetFloats(Uniforms._FilterKernelsFarField, settings.filterKernelsFarField);
                    m_sssComputeShader.SetVector(Shader.PropertyToID("_SSScreenParams"), new Vector4(cameraTextureDescriptor.width, cameraTextureDescriptor.height, (float)1.0 / cameraTextureDescriptor.width, (float)1.0 / cameraTextureDescriptor.height));

                    // To align with the hlsl packing rules

                    float[] unpackedWorldScale = new float[8 * 4];
                    for(int i = 0; i < 8; i++)
                    {
                        unpackedWorldScale[i * 4] = settings.worldScales[i];
                    }
                    m_sssComputeShader.SetFloats(Uniforms._WorldScales, unpackedWorldScale);

                    ConfigureTarget(colorTexture);
                    ConfigureClear(ClearFlag.All, Color.black);

                }
                else
                {
                    Debug.LogWarning("Subsurface Scattering Render Feature is missing shader: ");
                }

            }
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            if (m_sssComputeShader != null && SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0 )
            {

                CommandBuffer cmd = CommandBufferPool.Get("Diffusion");
                cmd.BeginSample("Diffusion");

                Camera mCamera = renderingData.cameraData.camera;

                cmd.SetGlobalMatrix(Uniforms._InvProjMatrix, SSSSUtils.GetGPUProjInverse(mCamera));

                Matrix4x4 viewMatrix = mCamera.worldToCameraMatrix;
                viewMatrix.SetRow(2, -viewMatrix.GetRow(2));        // Make Z axis point forwards in the view space (left-handed CS)
                Matrix4x4 projMatrix = GL.GetGPUProjectionMatrix(mCamera.projectionMatrix, false);
                projMatrix.SetColumn(2, -projMatrix.GetColumn(2));  // Undo the view-space transformation

                m_sssComputeShader.SetMatrix(Uniforms._ViewMatrix, viewMatrix);
                m_sssComputeShader.SetMatrix(Uniforms._ProjMatrix, projMatrix);
                m_sssComputeShader.SetMatrix(Uniforms._InvProjMatrix, SSSSUtils.GetGPUProjInverse(mCamera));
                
                cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, colorTexture);
                
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                m_sssComputeShader.SetTexture(0, Shader.PropertyToID("_ColorTexture"), colorTexture);

                int numTilesX = ((int)renderingData.cameraData.cameraTargetDescriptor.width + 15) / 16;
                int numTilesY = ((int)renderingData.cameraData.cameraTargetDescriptor.height + 15) / 16;
                int numTilesZ = 1;

                cmd.DispatchCompute(m_sssComputeShader, 0, numTilesX, numTilesY, numTilesZ);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                cmd.Blit(colorTexture, renderingData.cameraData.renderer.cameraColorTarget);

                cmd.EndSample("Diffusion");
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }


    //--------------------------------------------------------------------------------------------------------------------------
    //-------------------------------    Configue Render Feature        --------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------


    SplitLightingRenderPass m_SplitLightingRenderPass;
    DiffusionRenderPass m_DiffusionRenderPass;
#if UNITY_EDITOR
    SerializedObject so;
#endif

    public override void Create()
    {
        //------------------------- Initialize custom passes ------------------------------------------------------------------

        m_SplitLightingRenderPass = new SplitLightingRenderPass();
        m_DiffusionRenderPass = new DiffusionRenderPass();

        m_SplitLightingRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        m_DiffusionRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

        //------------------------- Initialize SubsurfaceScatteringModel --------------------------------------------------

        if (profileManager == null)
        {
            profileManager = new SubsurfaceScatteringProfileManager();
            profileManager.Init();
        }

        if (SubsurfaceScatteringProfileManager.profiles == null)
        {
            SubsurfaceScatteringProfileManager.profiles = m_profiles;
            profileManager.Init();
        }
#if UNITY_EDITOR
        if (so == null)
            so = new SerializedObject(this);

        EditorUtility.SetDirty(so.targetObject);
#endif

        //------------------------ Update Material and Profile ------------------------------------------------------------------------


        profileManager.UpdateGUI();

        m_SubsurfaceLayer = settings.layer;

        m_profiles = SubsurfaceScatteringProfileManager.profiles;

        //----------------------------Compute Shader--------------------------------------------------------------------

        if (sssComputeShader == null)
        {
            //sssComputeShader = (ComputeShader)Resources.Load("SubsurfaceScattering");
        }

        if(m_sssComputeShader == null)
        {
            m_sssComputeShader = sssComputeShader;
        }


        CommandBuffer cmd = CommandBufferPool.Get("Set Up Material Key Words");
        if (isActive)
        {
            cmd.EnableShaderKeyword("_SUBSURFACE_PASS");
        }
        else
        {
            cmd.DisableShaderKeyword("_SUBSURFACE_PASS");
        }

        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        cmd.Release();
        
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_SplitLightingRenderPass);
        renderer.EnqueuePass(m_DiffusionRenderPass);

    }

}

