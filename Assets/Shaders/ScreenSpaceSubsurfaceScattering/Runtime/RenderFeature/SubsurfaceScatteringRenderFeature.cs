using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SubsurfaceScatteringRenderFeature : ScriptableRendererFeature
{
    //--------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------- Variables -------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------

    // Public Properties

    // GUI Interface
    [Serializable]
    public class RenderSettings
    {
        [SerializeField] public SubsurfaceScatteringProfile[] profiles = new SubsurfaceScatteringProfile[SubsurfaceScatteringProfileManager.MAX_PROFILES];
        public LayerMask layer;
        public DownSamplingMode downSamplingMode;
    }

    public enum DownSamplingMode
    {
        None,
        Half,
        Quarter
    }

    [HideInInspector]
    public RenderSettings settings = new RenderSettings();

    [SerializeField] public SubsurfaceScatteringProfileManager profileManager;

    //Private Setting Properties
    //private static LayerMask m_SubsurfaceLayer;
    //private static DownSamplingMode m_downSamplingMode;
    [SerializeField] private SubsurfaceScatteringProfile[] m_profiles = new SubsurfaceScatteringProfile[SubsurfaceScatteringProfileManager.MAX_PROFILES];

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
        internal static readonly int _TransmissionMode = Shader.PropertyToID("_TransmissionMode");
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
    //-------------------------------------- Pass 1 : Split Lighting Pass ------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------
    // Render the seperate diffuse and specular lighting RT and param RT
    // When downsampling is enabled, it will render diffuseRT and paramRT in downsampled formats. The specularRT will be rendered in full size in the following pass.

    class SplitLightingRenderPass : ScriptableRenderPass
    {
        //Render seperate diffuse and specular lighting 
        RenderTexture diffuseRT;
        RenderTexture specularRT;
        RenderTexture paramRT;                                                                      // paramRT: R: SubsurfaceRadius  G: Thickness B: SubsurfaceProfileIndex A: _

        LayerMask m_SubsurfaceLayer;
        DownSamplingMode m_downSamplingMode;

        public void Setup(RenderSettings settins)
        {
            m_SubsurfaceLayer = settins.layer;
            m_downSamplingMode = settins.downSamplingMode;
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {

            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                //Update material properties per frame
                cmd.BeginSample("DirectSplitLighting");

                var settings = SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings;

                cmd.SetGlobalVectorArray(Uniforms._ShapeParams, settings.shapeParams);
                cmd.SetGlobalVectorArray(Uniforms._TransmissionTints, settings.transmissionTints);
                cmd.SetGlobalFloatArray(Uniforms._ThicknessRemaps, settings.thicknessRemaps);
                cmd.SetGlobalFloatArray(Uniforms._TransmissionMode, settings.transmissionFlagsBuffer);

                RenderTextureDescriptor opaqueDesc = cameraTextureDescriptor;
                opaqueDesc.vrUsage = cameraTextureDescriptor.vrUsage;
                opaqueDesc.depthBufferBits = cameraTextureDescriptor.depthBufferBits;

                switch (m_downSamplingMode)
                {
                    case DownSamplingMode.None:
                        {
                            opaqueDesc.colorFormat = RenderTextureFormat.ARGB2101010;
                            diffuseRT = RenderTexture.GetTemporary(opaqueDesc);

                            opaqueDesc.depthBufferBits = 0;
                            opaqueDesc.colorFormat = RenderTextureFormat.ARGB2101010;
                            paramRT = RenderTexture.GetTemporary(opaqueDesc);
                            specularRT = RenderTexture.GetTemporary(opaqueDesc);

                            ConfigureTarget(new RenderTargetIdentifier[] { diffuseRT, specularRT, paramRT }, diffuseRT.depthBuffer);
                            ConfigureClear(ClearFlag.All, Color.black);
                            break;
                        }
                    case DownSamplingMode.Half:
                        {
                            opaqueDesc.width /= 2;
                            opaqueDesc.height /= 2;

                            opaqueDesc.colorFormat = RenderTextureFormat.ARGB32;
                            diffuseRT = RenderTexture.GetTemporary(opaqueDesc);

                            opaqueDesc.depthBufferBits = 0;

                            paramRT = RenderTexture.GetTemporary(opaqueDesc);

                            ConfigureTarget(new RenderTargetIdentifier[] { diffuseRT, paramRT }, diffuseRT.depthBuffer);
                            ConfigureClear(ClearFlag.All, Color.black);
                            break;
                        }
                    case DownSamplingMode.Quarter:
                        {
                            opaqueDesc.width /= 4;
                            opaqueDesc.height /= 4;
                            opaqueDesc.colorFormat = RenderTextureFormat.ARGB32;
                            diffuseRT = RenderTexture.GetTemporary(opaqueDesc);

                            opaqueDesc.depthBufferBits = 0;

                            paramRT = RenderTexture.GetTemporary(opaqueDesc);

                            ConfigureTarget(new RenderTargetIdentifier[] { diffuseRT, paramRT }, diffuseRT.depthBuffer);
                            ConfigureClear(ClearFlag.All, Color.black);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                if (m_downSamplingMode == DownSamplingMode.None)
                {
                    CommandBuffer cmd = CommandBufferPool.Get("SplitLighting");
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

                    cmd.SetGlobalTexture(Uniforms._IrradianceSource, diffuseRT);  // Cannot set a RT on a material
                    cmd.SetGlobalTexture(Uniforms._SSSSpecular, specularRT);  // Cannot set a RT on a material
                    cmd.SetGlobalTexture(Uniforms._SSSParams, paramRT);
                    cmd.SetGlobalTexture("_SSSAlbedo", Texture2D.whiteTexture);

                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                    CommandBufferPool.Release(cmd);
                }
                else
                {
                    CommandBuffer cmd = CommandBufferPool.Get("SplitLighting");
                    cmd.Clear();

                    SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
                    ShaderTagId m_ShaderTagId = new ShaderTagId("SplitForwardDownsampling");
                    DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortingCriteria);

                    //Material overrideMaterial = null;
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

                    cmd.SetGlobalTexture(Uniforms._IrradianceSource, diffuseRT);  // Cannot set a RT on a material
                    cmd.SetGlobalTexture(Uniforms._SSSParams, paramRT);

                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                    CommandBufferPool.Release(cmd);
                }

            }

            if (!renderingData.cameraData.requiresDepthTexture)
            {
                Debug.LogWarning("Screen Space Subsurface Scattering requires Depth Texture, please enable \"Depth Texture\" in the Universal Render Pipeline Asset.");
            }

        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                if (m_downSamplingMode == DownSamplingMode.None)
                {
                    RenderTexture.ReleaseTemporary(specularRT);
                }
                RenderTexture.ReleaseTemporary(diffuseRT);
                RenderTexture.ReleaseTemporary(paramRT);
            }

        }
    }
    //--------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------- SplitLightingAlbedoRenderPass ----------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------
    // This pass is only used when downsampling is enabled
    // The outputs are full resolution specular and albedo RT, preserving the high frequency info 

    class SplitLightingAlbedoRenderPass : ScriptableRenderPass
    {
        RenderTexture specularRT;
        RenderTexture albedoRT;

        LayerMask m_SubsurfaceLayer;
        DownSamplingMode m_downSamplingMode;

        public void Setup(RenderSettings settins)
        {
            m_SubsurfaceLayer = settins.layer;
            m_downSamplingMode = settins.downSamplingMode;
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {

            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                if (m_downSamplingMode != DownSamplingMode.None)
                {
                    //Update material properties per frame
                    cmd.BeginSample("DirectSplitLighting");

                    RenderTextureDescriptor opaqueDesc = cameraTextureDescriptor;

                    opaqueDesc.colorFormat = RenderTextureFormat.ARGB2101010;
                    opaqueDesc.vrUsage = cameraTextureDescriptor.vrUsage;
                    opaqueDesc.depthBufferBits = cameraTextureDescriptor.depthBufferBits;
                    specularRT = RenderTexture.GetTemporary(opaqueDesc);
                    opaqueDesc.depthBufferBits = 0;
                    albedoRT = RenderTexture.GetTemporary(opaqueDesc);

                    ConfigureTarget(new RenderTargetIdentifier[] { specularRT, albedoRT }, specularRT.depthBuffer);
                    ConfigureClear(ClearFlag.All, Color.black);
                }

            }

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                if (m_downSamplingMode != DownSamplingMode.None)
                {
                    CommandBuffer cmd = CommandBufferPool.Get("SplitLightingAlbedo");
                    cmd.Clear();
                    //Stencil only pass

                    SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
                    ShaderTagId m_ShaderTagId = new ShaderTagId("SplitForwardAlbedo");
                    DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortingCriteria);

                    //Material overrideMaterial = null;
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

                    cmd.SetGlobalTexture(Uniforms._SSSSpecular, specularRT);  // Cannot set a RT on a material
                    cmd.SetGlobalTexture("_SSSAlbedo", albedoRT);  // Cannot set a RT on a material

                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                    CommandBufferPool.Release(cmd);
                }

            }

        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {
                if (m_downSamplingMode != DownSamplingMode.None)
                {
                    RenderTexture.ReleaseTemporary(specularRT);
                    RenderTexture.ReleaseTemporary(albedoRT);
                }
            }
        }
    }



    //--------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------- Pass 2 : Diffusion Pass  ---------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------

    class DiffusionRenderPass : ScriptableRenderPass
    {

        Material m_SubsurfaceBlur;

        LayerMask m_SubsurfaceLayer;
        DownSamplingMode m_downSamplingMode;

        public void Setup(RenderSettings settins)
        {
            m_SubsurfaceLayer = settins.layer;
            m_downSamplingMode = settins.downSamplingMode;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {

                var settings = SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings;

                if (m_SubsurfaceBlur == null)
                {
                    if (Shader.Find("Hidden/Universal Render Pipeline/SSSDownsampling"))
                    {
                        m_SubsurfaceBlur = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Universal Render Pipeline/SSSDownsampling"));
                    }
                    else
                    {
                        Debug.LogWarning("Subsurface Scattering Render Feature is missing shader: ");
                    }
                }
                else
                {
                    if (m_downSamplingMode == DownSamplingMode.None)
                    {
                        m_SubsurfaceBlur.DisableKeyword("_PARTIALDOWNSAMPLE");

                    }
                    else
                    {
                        m_SubsurfaceBlur.EnableKeyword("_PARTIALDOWNSAMPLE");

                    }
                }
            }
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_SubsurfaceBlur && SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings.profiles.Length > 0)
            {

                var settings = SubsurfaceScatteringProfileManager.subsurfaceScatteringModel.settings;
                m_SubsurfaceBlur.SetVectorArray(Uniforms._ShapeParams, settings.shapeParams);
                m_SubsurfaceBlur.SetFloatArray(Uniforms._WorldScales, settings.worldScales);
                m_SubsurfaceBlur.SetFloatArray(Uniforms._FilterKernelsNearField, settings.filterKernelsNearField);
                m_SubsurfaceBlur.SetFloatArray(Uniforms._FilterKernelsFarField, settings.filterKernelsFarField);

                CommandBuffer cmd = CommandBufferPool.Get("Diffusion");
                cmd.BeginSample("Diffusion");

                Camera mCamera = renderingData.cameraData.camera;
            
                cmd.SetGlobalMatrix(Uniforms._InvProjMatrix, SSSSUtils.GetGPUProjInverse(mCamera));

                Matrix4x4 viewMatrix = mCamera.worldToCameraMatrix;
                viewMatrix.SetRow(2, -viewMatrix.GetRow(2));        // Make Z axis point forwards in the view space (left-handed CS)
                Matrix4x4 projMatrix = GL.GetGPUProjectionMatrix(mCamera.projectionMatrix, false);
                projMatrix.SetColumn(2, -projMatrix.GetColumn(2));  // Undo the view-space transformation
                m_SubsurfaceBlur.SetMatrix(Uniforms._ViewMatrix, viewMatrix);
                m_SubsurfaceBlur.SetMatrix(Uniforms._ProjMatrix, projMatrix);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                if (m_downSamplingMode == DownSamplingMode.None)
                {

                    cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_SubsurfaceBlur, 0, 0);
                }
                else
                {
                    cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_SubsurfaceBlur, 0, 1);

                }

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
    SplitLightingAlbedoRenderPass m_SplitLightingAlbedoRenderPass;
#if UNITY_EDITOR
    SerializedObject so;
#endif

    public override void Create()
    {
        
        //------------------------- Initialize custom passes ------------------------------------------------------------------

        m_SplitLightingRenderPass = new SplitLightingRenderPass();
        m_DiffusionRenderPass = new DiffusionRenderPass();
        m_SplitLightingAlbedoRenderPass = new SplitLightingAlbedoRenderPass();

        m_SplitLightingRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        m_SplitLightingAlbedoRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        m_DiffusionRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

        m_SplitLightingRenderPass.Setup(settings);
        m_SplitLightingAlbedoRenderPass.Setup(settings);
        m_DiffusionRenderPass.Setup(settings);

        //------------------------- Initialize SubsurfaceScatteringModel --------------------------------------------------

        if (profileManager == null)
        {
            profileManager = new SubsurfaceScatteringProfileManager();
            profileManager.Init();
            SubsurfaceScatteringProfileManager.needToUpdateGUI = true;
        }

        if(SubsurfaceScatteringProfileManager.profiles == null)
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
        SubsurfaceScatteringProfileManager.ApplyProfilesToSSModel();

        m_profiles = SubsurfaceScatteringProfileManager.profiles;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        renderer.EnqueuePass(m_SplitLightingAlbedoRenderPass);
        renderer.EnqueuePass(m_SplitLightingRenderPass);
        renderer.EnqueuePass(m_DiffusionRenderPass);

    }

}
