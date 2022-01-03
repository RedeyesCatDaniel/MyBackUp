using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Legacy;

using static UnityEditor.Rendering.Universal.ShaderGraph.SubShaderUtils;
using UnityEngine.Rendering.Universal;
using static Unity.Rendering.Universal.ShaderUtils;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    sealed class SubsurfaceScatteringShaderGraphTarget : UniversalSubTarget //SubTarget<UniversalTarget>
    {
        static readonly GUID kSourceCodeGuid = new GUID("f73ad065f129dac4ab92f6b1309bafa6"); // UniversalLitSubTarget.cs

        [SerializeField]
        WorkflowMode m_WorkflowMode = WorkflowMode.Metallic;

        [SerializeField]
        NormalDropOffSpace m_NormalDropOffSpace = NormalDropOffSpace.Tangent;

        [SerializeField]
        bool m_ClearCoat = false;

        [SerializeField]
        bool m_DualSpecularLobe = false;

        [SerializeField]
        bool m_SpecularOcclusion = false;

        public SubsurfaceScatteringShaderGraphTarget()
        {
            displayName = "Subsurface Scattering";
        }

        protected override ShaderID shaderID => ShaderID.SG_Lit;

        public WorkflowMode workflowMode
        {
            get => m_WorkflowMode;
            set => m_WorkflowMode = value;
        }

        public NormalDropOffSpace normalDropOffSpace
        {
            get => m_NormalDropOffSpace;
            set => m_NormalDropOffSpace = value;
        }

        public bool clearCoat
        {
            get => m_ClearCoat;
            set => m_ClearCoat = value;
        }

        private bool complexLit
        {
            get
            {
                // Rules for switching to ComplexLit with forward only pass
                return clearCoat; // && <complex feature>
            }
        }

        public override bool IsActive() => true;

        public override void Setup(ref TargetSetupContext context)
        {
            context.AddAssetDependency(kSourceCodeGuid, AssetCollection.Flags.SourceDependency);
            base.Setup(ref context);

            var universalRPType = typeof(UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset);
            if (!context.HasCustomEditorForRenderPipeline(universalRPType))
                context.AddCustomEditorForRenderPipeline(typeof(ShaderGraphLitGUI).FullName, universalRPType);

            // Process SubShaders
            context.AddSubShader(SubShaders.LitComputeDotsSubShader(target, workflowMode, target.renderType, target.renderQueue, complexLit));
            context.AddSubShader(SubShaders.LitGLESSubShader(target, workflowMode, target.renderType, target.renderQueue, complexLit));
        }

        public static FieldDescriptor DualSpecularLobe = new FieldDescriptor(string.Empty, "DualSpecularLobe", "_DUALSPECULARLOBE");
        public static FieldDescriptor SpecularOcclusion = new FieldDescriptor(string.Empty, "SpecularOcclusion", "_SPECULARAO");

        public override void ProcessPreviewMaterial(Material material)
        {
            if (target.allowMaterialOverride)
            {
                // copy our target's default settings into the material
                // (technically not necessary since we are always recreating the material from the shader each time,
                // which will pull over the defaults from the shader definition)
                // but if that ever changes, this will ensure the defaults are set
                material.SetFloat(Property.SpecularWorkflowMode, (float)workflowMode);
                material.SetFloat(Property.CastShadows, target.castShadows ? 1.0f : 0.0f);
                material.SetFloat(Property.ReceiveShadows, target.receiveShadows ? 1.0f : 0.0f);
                material.SetFloat(Property.SurfaceType, (float)target.surfaceType);
                material.SetFloat(Property.BlendMode, (float)target.alphaMode);
                material.SetFloat(Property.AlphaClip, target.alphaClip ? 1.0f : 0.0f);
                material.SetFloat(Property.CullMode, (int)target.renderFace);
                material.SetFloat(Property.ZWriteControl, (float)target.zWriteControl);
                material.SetFloat(Property.ZTest, (float)target.zTestMode);
            }

            // We always need these properties regardless of whether the material is allowed to override
            // Queue control & offset enable correct automatic render queue behavior
            // Control == 0 is automatic, 1 is user-specified render queue
            material.SetFloat(Property.QueueOffset, 0.0f);
            material.SetFloat(Property.QueueControl, (float)BaseShaderGUI.QueueControl.Auto);

            // call the full unlit material setup function
            ShaderGraphLitGUI.UpdateMaterial(material, MaterialUpdateType.CreatedNewMaterial);
        }

        public override void GetFields(ref TargetFieldContext context)
        {
            //FieldDescriptor myProperty = new FieldDescriptor("Eye", "SO", "SO");
            //var descs = context.blocks.Select(x => x.descriptor);
            //// Surface Type & Blend Mode
            //// These must be set per SubTarget as Sprite SubTargets override them
            //context.AddField(myProperty, true);

            var descs = context.blocks.Select(x => x.descriptor);

            context.AddField(UniversalFields.SurfaceOpaque, target.surfaceType == SurfaceType.Opaque);
            context.AddField(UniversalFields.SurfaceTransparent, target.surfaceType != SurfaceType.Opaque);
            context.AddField(UniversalFields.BlendAdd, target.surfaceType != SurfaceType.Opaque && target.alphaMode == AlphaMode.Additive);
            context.AddField(Fields.BlendAlpha, target.surfaceType != SurfaceType.Opaque && target.alphaMode == AlphaMode.Alpha);
            context.AddField(UniversalFields.BlendMultiply, target.surfaceType != SurfaceType.Opaque && target.alphaMode == AlphaMode.Multiply);
            context.AddField(UniversalFields.BlendPremultiply, target.surfaceType != SurfaceType.Opaque && target.alphaMode == AlphaMode.Premultiply);

            // Lit
            context.AddField(UniversalFields.NormalDropOffOS, normalDropOffSpace == NormalDropOffSpace.Object);
            context.AddField(UniversalFields.NormalDropOffTS, normalDropOffSpace == NormalDropOffSpace.Tangent);
            context.AddField(UniversalFields.NormalDropOffWS, normalDropOffSpace == NormalDropOffSpace.World);
            //context.AddField(UniversalFields.SpecularSetup, workflowMode == WorkflowMode.Specular);
            context.AddField(UniversalFields.Normal, descs.Contains(BlockFields.SurfaceDescription.NormalOS) ||
                                                                             descs.Contains(BlockFields.SurfaceDescription.NormalTS) ||
                                                                             descs.Contains(BlockFields.SurfaceDescription.NormalWS));
            // Complex Lit

            // Template Predicates
            //context.AddField(UniversalFields.PredicateClearCoat, clearCoat);
            context.AddField(DualSpecularLobe, m_DualSpecularLobe);
            context.AddField(SpecularOcclusion, m_SpecularOcclusion);
            
        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context)
        {
            context.AddBlock(BlockFields.SurfaceDescription.Smoothness);
            context.AddBlock(BlockFields.SurfaceDescription.NormalOS, normalDropOffSpace == NormalDropOffSpace.Object);
            context.AddBlock(BlockFields.SurfaceDescription.NormalTS, normalDropOffSpace == NormalDropOffSpace.Tangent);
            context.AddBlock(BlockFields.SurfaceDescription.NormalWS, normalDropOffSpace == NormalDropOffSpace.World);
            context.AddBlock(BlockFields.SurfaceDescription.Emission);
            context.AddBlock(BlockFields.SurfaceDescription.Occlusion);
            context.AddBlock(BlockFields.SurfaceDescription.Specular, workflowMode == WorkflowMode.Specular);
            context.AddBlock(BlockFields.SurfaceDescription.Metallic, workflowMode == WorkflowMode.Metallic);
            context.AddBlock(BlockFields.SurfaceDescription.Alpha, target.surfaceType == SurfaceType.Transparent || target.alphaClip);
            context.AddBlock(BlockFields.SurfaceDescription.AlphaClipThreshold, target.alphaClip);
            context.AddBlock(BlockFields.SurfaceDescription.CoatMask, clearCoat);
            context.AddBlock(BlockFields.SurfaceDescription.CoatSmoothness, clearCoat);
            context.AddBlock(UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.DiffusionProfileHash);
            context.AddBlock(UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.SubsurfaceMask);
            context.AddBlock(UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.Thickness);
            context.AddBlock(UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.LobeInterpolation, m_DualSpecularLobe);
            context.AddBlock(UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.LobeDerivation,    m_DualSpecularLobe);
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            // if using material control, add the material property to control workflow mode
            if (target.allowMaterialOverride)
            {
                collector.AddFloatProperty(Property.SpecularWorkflowMode, (float)workflowMode);
                collector.AddFloatProperty(Property.CastShadows, target.castShadows ? 1.0f : 0.0f);
                collector.AddFloatProperty(Property.ReceiveShadows, target.receiveShadows ? 1.0f : 0.0f);

                // setup properties using the defaults
                collector.AddFloatProperty(Property.SurfaceType, (float)target.surfaceType);
                collector.AddFloatProperty(Property.BlendMode, (float)target.alphaMode);
                collector.AddFloatProperty(Property.AlphaClip, target.alphaClip ? 1.0f : 0.0f);
                collector.AddFloatProperty(Property.SrcBlend, 1.0f);    // always set by material inspector, ok to have incorrect values here
                collector.AddFloatProperty(Property.DstBlend, 0.0f);    // always set by material inspector, ok to have incorrect values here
                collector.AddToggleProperty(Property.ZWrite, (target.surfaceType == SurfaceType.Opaque));
                collector.AddFloatProperty(Property.ZWriteControl, (float)target.zWriteControl);
                collector.AddFloatProperty(Property.ZTest, (float)target.zTestMode);    // ztest mode is designed to directly pass as ztest
                collector.AddFloatProperty(Property.CullMode, (float)target.renderFace);    // render face enum is designed to directly pass as a cull mode
            }

            // We always need these properties regardless of whether the material is allowed to override other shader properties.
            // Queue control & offset enable correct automatic render queue behavior.  Control == 0 is automatic, 1 is user-specified.
            // We initialize queue control to -1 to indicate to UpdateMaterial that it needs to initialize it properly on the material.
            collector.AddFloatProperty(Property.QueueOffset, 0.0f);
            collector.AddFloatProperty(Property.QueueControl, -1.0f);
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo)
        {
            var universalTarget = (target as UniversalTarget);
            universalTarget.AddDefaultMaterialOverrideGUI(ref context, onChange, registerUndo);

            context.AddProperty("Workflow Mode", new EnumField(WorkflowMode.Metallic) { value = workflowMode }, (evt) =>
            {
                if (Equals(workflowMode, evt.newValue))
                    return;

                registerUndo("Change Workflow");
                workflowMode = (WorkflowMode)evt.newValue;
                onChange();
            });

            universalTarget.AddDefaultSurfacePropertiesGUI(ref context, onChange, registerUndo, showReceiveShadows: true);

            context.AddProperty("Fragment Normal Space", new EnumField(NormalDropOffSpace.Tangent) { value = normalDropOffSpace }, (evt) =>
            {
                if (Equals(normalDropOffSpace, evt.newValue))
                    return;

                registerUndo("Change Fragment Normal Space");
                normalDropOffSpace = (NormalDropOffSpace)evt.newValue;
                onChange();
            });


            context.AddProperty("Dual Specular Lobe", new Toggle() { value = m_DualSpecularLobe }, (evt) =>
            {
                if (Equals(m_DualSpecularLobe, evt.newValue))
                    return;

                registerUndo("Change Dual Specular Lobe");
                m_DualSpecularLobe = evt.newValue;
                onChange();
            });

            context.AddProperty("Specular Occlusion", new Toggle() { value = m_SpecularOcclusion }, (evt) =>
            {
                if (Equals(m_SpecularOcclusion, evt.newValue))
                    return;

                registerUndo("Change Specular OcclusionS");
                m_SpecularOcclusion = evt.newValue;
                onChange();
            });
        }



        #region SubShader
        static class SubShaders
        {
            // SM 4.5, compute with dots instancing
            public static SubShaderDescriptor LitComputeDotsSubShader(UniversalTarget target, WorkflowMode workflowMode, string renderType, string renderQueue, bool complexLit)
            {
                SubShaderDescriptor result = new SubShaderDescriptor()
                {
                    pipelineTag = UniversalTarget.kPipelineTag,
                    customTags = UniversalTarget.kLitMaterialTypeTag,
                    renderType = renderType,
                    renderQueue = renderQueue,
                    generatesPreview = true,
                    passes = new PassCollection()
                };

                if (complexLit)
                    result.passes.Add(LitPasses.ForwardOnly(target, workflowMode, complexLit, CoreBlockMasks.Vertex, LitBlockMasks.FragmentComplexLit, CorePragmas.DOTSForward));
                else
                    result.passes.Add(LitPasses.Forward(target, workflowMode, CorePragmas.DOTSForward));

                if (!complexLit)
                    result.passes.Add(LitPasses.GBuffer(target, workflowMode));

                // cull the shadowcaster pass if we know it will never be used
                if (target.castShadows || target.allowMaterialOverride)
                    result.passes.Add(PassVariant(CorePasses.ShadowCaster(target), CorePragmas.DOTSInstanced));

                if (target.mayWriteDepth)
                    result.passes.Add(PassVariant(CorePasses.DepthOnly(target), CorePragmas.DOTSInstanced));

                if (complexLit)
                    result.passes.Add(PassVariant(LitPasses.DepthNormalOnly(target), CorePragmas.DOTSInstanced));
                else
                    result.passes.Add(PassVariant(LitPasses.DepthNormal(target), CorePragmas.DOTSInstanced));
                result.passes.Add(PassVariant(LitPasses.Meta(target), CorePragmas.DOTSDefault));
                result.passes.Add(PassVariant(LitPasses._2D(target), CorePragmas.DOTSDefault));
                result.passes.Add(PassVariant(CorePasses.SceneSelection(target), CorePragmas.DOTSDefault));
                result.passes.Add(PassVariant(CorePasses.ScenePicking(target), CorePragmas.DOTSDefault));

                return result;
            }

            public static SubShaderDescriptor LitGLESSubShader(UniversalTarget target, WorkflowMode workflowMode, string renderType, string renderQueue, bool complexLit)
            {
                // SM 2.0, GLES

                // ForwardOnly pass is used as complex Lit SM 2.0 fallback for GLES.
                // Drops advanced features and renders materials as Lit.

                SubShaderDescriptor result = new SubShaderDescriptor()
                {
                    pipelineTag = UniversalTarget.kPipelineTag,
                    customTags = UniversalTarget.kLitMaterialTypeTag,
                    renderType = renderType,
                    renderQueue = renderQueue,
                    generatesPreview = true,
                    passes = new PassCollection()
                };

                if (complexLit)
                    result.passes.Add(LitPasses.ForwardOnly(target, workflowMode, complexLit, CoreBlockMasks.Vertex, LitBlockMasks.FragmentComplexLit, CorePragmas.Forward));
                else
                    result.passes.Add(LitPasses.Forward(target, workflowMode));

                // cull the shadowcaster pass if we know it will never be used
                if (target.castShadows || target.allowMaterialOverride)
                    result.passes.Add(CorePasses.ShadowCaster(target));

                if (target.mayWriteDepth)
                    result.passes.Add(CorePasses.DepthOnly(target));

                if (complexLit)
                    result.passes.Add(CorePasses.DepthNormalOnly(target));
                else
                    result.passes.Add(CorePasses.DepthNormal(target));
                result.passes.Add(LitPasses.Meta(target));
                result.passes.Add(LitPasses._2D(target));
                result.passes.Add(CorePasses.SceneSelection(target));
                result.passes.Add(CorePasses.ScenePicking(target));

                return result;
            }
        }
        #endregion

        #region Passes
        static class LitPasses
        {
            static void AddWorkflowModeControlToPass(ref PassDescriptor pass, UniversalTarget target, WorkflowMode workflowMode)
            {
                if (target.allowMaterialOverride)
                    pass.keywords.Add(LitDefines.SpecularSetup);
                else if (workflowMode == WorkflowMode.Specular)
                    pass.defines.Add(LitDefines.SpecularSetup, 1);
            }

            static void AddReceiveShadowsControlToPass(ref PassDescriptor pass, UniversalTarget target, bool receiveShadows)
            {
                if (target.allowMaterialOverride)
                    pass.keywords.Add(LitKeywords.ReceiveShadowsOff);
                else if (!receiveShadows)
                    pass.defines.Add(LitKeywords.ReceiveShadowsOff, 1);
            }

            public static PassDescriptor SplitForward(UniversalTarget target, WorkflowMode workflowMode, PragmaCollection pragmas = null)
            {
                var result = new PassDescriptor()
                {
                    // Definition
                    displayName = "Universal Split Forward",
                    referenceName = "SHADERPASS_SPLIT_FORWARD",
                    lightMode = "SplitForward",
                    useInPreview = true,

                    // Template
                    //passTemplatePath = UniversalTarget.kUberTemplatePath,
                    passTemplatePath = "Assets/ScreenSpaceSubsurfaceScattering/Editor/ShaderGraph/Generation/SubsurfacePassMesh.template",
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = LitBlockMasks.FragmentLit,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = LitRequiredFields.Forward,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.UberSwitchedRenderState(target),
                    pragmas = pragmas ?? SubsurfacePragmas.SplitForward,     // NOTE: SM 2.0 only GL
                    defines = new DefineCollection() { CoreDefines.UseFragmentFog },
                    keywords = new KeywordCollection() { LitKeywords.Forward },
                    includes = LitIncludes.SplitForward,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddTargetSurfaceControlsToPass(ref result, target);
                AddWorkflowModeControlToPass(ref result, target, workflowMode);
                AddReceiveShadowsControlToPass(ref result, target, target.receiveShadows);

                return result;
            }

            public static PassDescriptor Forward(UniversalTarget target, WorkflowMode workflowMode, PragmaCollection pragmas = null)
            {
                var result = new PassDescriptor()
                {
                    // Definition
                    displayName = "Universal Forward",
                    referenceName = "SHADERPASS_FORWARD",
                    lightMode = "UniversalForward",
                    useInPreview = true,

                    // Template
                    //passTemplatePath = UniversalTarget.kUberTemplatePath,
                    passTemplatePath = "Assets/ScreenSpaceSubsurfaceScattering/Editor/ShaderGraph/Generation/SubsurfacePassMesh.template",
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = LitBlockMasks.FragmentLit,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = LitRequiredFields.Forward,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.UberSwitchedRenderState(target),
                    pragmas = pragmas ?? CorePragmas.Forward,     // NOTE: SM 2.0 only GL
                    defines = new DefineCollection() { CoreDefines.UseFragmentFog },
                    keywords = new KeywordCollection() { LitKeywords.Forward },
                    includes = LitIncludes.Forward,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddTargetSurfaceControlsToPass(ref result, target);
                AddWorkflowModeControlToPass(ref result, target, workflowMode);
                AddReceiveShadowsControlToPass(ref result, target, target.receiveShadows);

                return result;
            }

            public static PassDescriptor ForwardOnly(
                UniversalTarget target,
                WorkflowMode workflowMode,
                bool complexLit,
                BlockFieldDescriptor[] vertexBlocks,
                BlockFieldDescriptor[] pixelBlocks,
                PragmaCollection pragmas)
            {
                var result = new PassDescriptor
                {
                    // Definition
                    displayName = "Universal Forward Only",
                    referenceName = "SHADERPASS_FORWARDONLY",
                    lightMode = "UniversalForwardOnly",
                    useInPreview = true,

                    // Template
                    //passTemplatePath = UniversalTarget.kUberTemplatePath,
                    passTemplatePath = "Assets/ScreenSpaceSubsurfaceScattering/Editor/ShaderGraph/Generation/SubsurfacePassMesh.template",
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = vertexBlocks,
                    validPixelBlocks = pixelBlocks,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = LitRequiredFields.Forward,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.UberSwitchedRenderState(target),
                    pragmas = pragmas,
                    defines = new DefineCollection() { CoreDefines.UseFragmentFog },
                    keywords = new KeywordCollection() { LitKeywords.Forward },
                    includes = LitIncludes.Forward,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                if (complexLit)
                    result.defines.Add(LitDefines.ClearCoat, 1);

                CorePasses.AddTargetSurfaceControlsToPass(ref result, target);
                AddWorkflowModeControlToPass(ref result, target, workflowMode);
                AddReceiveShadowsControlToPass(ref result, target, target.receiveShadows);

                return result;
            }

            // Deferred only in SM4.5, MRT not supported in GLES2
            public static PassDescriptor GBuffer(UniversalTarget target, WorkflowMode workflowMode)
            {
                var result = new PassDescriptor
                {
                    // Definition
                    displayName = "GBuffer",
                    referenceName = "SHADERPASS_GBUFFER",
                    lightMode = "UniversalGBuffer",

                    // Template
                    passTemplatePath = UniversalTarget.kUberTemplatePath,
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = LitBlockMasks.FragmentLit,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = LitRequiredFields.GBuffer,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.UberSwitchedRenderState(target),
                    pragmas = CorePragmas.DOTSGBuffer,
                    defines = new DefineCollection() { CoreDefines.UseFragmentFog },
                    keywords = new KeywordCollection() { LitKeywords.GBuffer },
                    includes = LitIncludes.GBuffer,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddTargetSurfaceControlsToPass(ref result, target);
                AddWorkflowModeControlToPass(ref result, target, workflowMode);
                AddReceiveShadowsControlToPass(ref result, target, target.receiveShadows);

                return result;
            }

            public static PassDescriptor Meta(UniversalTarget target)
            {
                var result = new PassDescriptor()
                {
                    // Definition
                    displayName = "Meta",
                    referenceName = "SHADERPASS_META",
                    lightMode = "Meta",

                    // Template
                    passTemplatePath = UniversalTarget.kUberTemplatePath,
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = LitBlockMasks.FragmentMeta,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = LitRequiredFields.Meta,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.Meta,
                    pragmas = CorePragmas.Default,
                    defines = new DefineCollection() { CoreDefines.UseFragmentFog },
                    keywords = new KeywordCollection() { CoreKeywordDescriptors.EditorVisualization },
                    includes = LitIncludes.Meta,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddAlphaClipControlToPass(ref result, target);

                return result;
            }

            public static PassDescriptor _2D(UniversalTarget target)
            {
                var result = new PassDescriptor()
                {
                    // Definition
                    referenceName = "SHADERPASS_2D",
                    lightMode = "Universal2D",

                    // Template
                    passTemplatePath = UniversalTarget.kUberTemplatePath,
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = CoreBlockMasks.FragmentColorAlpha,

                    // Fields
                    structs = CoreStructCollections.Default,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.UberSwitchedRenderState(target),
                    pragmas = CorePragmas.Instanced,
                    defines = new DefineCollection(),
                    keywords = new KeywordCollection(),
                    includes = LitIncludes._2D,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddAlphaClipControlToPass(ref result, target);

                return result;
            }

            public static PassDescriptor DepthNormal(UniversalTarget target)
            {
                var result = new PassDescriptor()
                {
                    // Definition
                    displayName = "DepthNormals",
                    referenceName = "SHADERPASS_DEPTHNORMALS",
                    lightMode = "DepthNormals",
                    useInPreview = false,

                    // Template
                    passTemplatePath = UniversalTarget.kUberTemplatePath,
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = CoreBlockMasks.FragmentDepthNormals,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = CoreRequiredFields.DepthNormals,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.DepthNormalsOnly(target),
                    pragmas = CorePragmas.Instanced,
                    defines = new DefineCollection(),
                    keywords = new KeywordCollection(),
                    includes = CoreIncludes.DepthNormalsOnly,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddAlphaClipControlToPass(ref result, target);

                return result;
            }

            public static PassDescriptor DepthNormalOnly(UniversalTarget target)
            {
                var result = new PassDescriptor()
                {
                    // Definition
                    displayName = "DepthNormalsOnly",
                    referenceName = "SHADERPASS_DEPTHNORMALSONLY",
                    lightMode = "DepthNormalsOnly",
                    useInPreview = false,

                    // Template
                    passTemplatePath = UniversalTarget.kUberTemplatePath,
                    sharedTemplateDirectories = UniversalTarget.kSharedTemplateDirectories,

                    // Port Mask
                    validVertexBlocks = CoreBlockMasks.Vertex,
                    validPixelBlocks = CoreBlockMasks.FragmentDepthNormals,

                    // Fields
                    structs = CoreStructCollections.Default,
                    requiredFields = CoreRequiredFields.DepthNormals,
                    fieldDependencies = CoreFieldDependencies.Default,

                    // Conditional State
                    renderStates = CoreRenderStates.DepthNormalsOnly(target),
                    pragmas = CorePragmas.Instanced,
                    defines = new DefineCollection(),
                    keywords = new KeywordCollection(),
                    includes = CoreIncludes.DepthNormalsOnly,

                    // Custom Interpolator Support
                    customInterpolators = CoreCustomInterpDescriptors.Common
                };

                CorePasses.AddAlphaClipControlToPass(ref result, target);

                return result;
            }
        }
        #endregion

        #region Pragmas
        // TODO: should these be renamed and moved to UniversalPragmas/UniversalPragmas.cs ?
        // TODO: these aren't "core" as HDRP doesn't use them
        // TODO: and the same for the rest "Core" things
        static class SubsurfacePragmas
        {

            public static readonly PragmaCollection SplitForward = new PragmaCollection
            {
                { Pragma.Target(ShaderModel.Target45) },
                { Pragma.ExcludeRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
                { Pragma.MultiCompileInstancing },
                { Pragma.MultiCompileFog },
                { Pragma.DOTSInstancing },
                { Pragma.Vertex("vert") },
                { Pragma.Fragment("frag") },
            };

            public static readonly PragmaCollection DOTSGBuffer = new PragmaCollection
            {
                { Pragma.Target(ShaderModel.Target45) },
                { Pragma.ExcludeRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
                { Pragma.MultiCompileInstancing },
                { Pragma.MultiCompileFog },
                { Pragma.DOTSInstancing },
                { Pragma.Vertex("vert") },
                { Pragma.Fragment("frag") },
            };
        }
        #endregion


        public static readonly StencilDescriptor subsurfaceStencilDescriptor = new StencilDescriptor { Ref = "1", Comp = "Always", Pass = "Replace", ZFail = "Keep" };

        public static readonly RenderStateCollection SubsurfaceState = new RenderStateCollection
        {
            { RenderState.ZTest(ZTest.LEqual) },
            { RenderState.ZWrite(ZWrite.On), new FieldCondition(UniversalFields.SurfaceOpaque, true) },
            { RenderState.ZWrite(ZWrite.Off), new FieldCondition(UniversalFields.SurfaceTransparent, true) },
            { RenderState.Cull(Cull.Back), new FieldCondition(Fields.DoubleSided, false) },
            { RenderState.Cull(Cull.Off), new FieldCondition(Fields.DoubleSided, true) },
            { RenderState.Blend(Blend.One, Blend.Zero), new FieldCondition(UniversalFields.SurfaceOpaque, true) },
            { RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(Fields.BlendAlpha, true) },
            { RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(UniversalFields.BlendPremultiply, true) },
            { RenderState.Blend(Blend.One, Blend.One, Blend.One, Blend.One), new FieldCondition(UniversalFields.BlendAdd, true) },
            { RenderState.Blend(Blend.DstColor, Blend.Zero), new FieldCondition(UniversalFields.BlendMultiply, true) },
            { RenderState.Stencil(subsurfaceStencilDescriptor)},
        };

        #region PortMasks
        static class LitBlockMasks
        {
            public static readonly BlockFieldDescriptor[] FragmentLit = new BlockFieldDescriptor[]
            {
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.NormalOS,
                BlockFields.SurfaceDescription.NormalTS,
                BlockFields.SurfaceDescription.NormalWS,
                BlockFields.SurfaceDescription.Emission,
                BlockFields.SurfaceDescription.Metallic,
                BlockFields.SurfaceDescription.Specular,
                BlockFields.SurfaceDescription.Smoothness,
                BlockFields.SurfaceDescription.Occlusion,
                BlockFields.SurfaceDescription.Alpha,
                BlockFields.SurfaceDescription.AlphaClipThreshold,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.DiffusionProfileHash,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.SubsurfaceMask,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.Thickness,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.LobeInterpolation,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.LobeDerivation,

            };

            public static readonly BlockFieldDescriptor[] FragmentComplexLit = new BlockFieldDescriptor[]
            {
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.NormalOS,
                BlockFields.SurfaceDescription.NormalTS,
                BlockFields.SurfaceDescription.NormalWS,
                BlockFields.SurfaceDescription.Emission,
                BlockFields.SurfaceDescription.Metallic,
                BlockFields.SurfaceDescription.Specular,
                BlockFields.SurfaceDescription.Smoothness,
                BlockFields.SurfaceDescription.Occlusion,
                BlockFields.SurfaceDescription.Alpha,
                BlockFields.SurfaceDescription.AlphaClipThreshold,
                BlockFields.SurfaceDescription.CoatMask,
                BlockFields.SurfaceDescription.CoatSmoothness,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.DiffusionProfileHash,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.SubsurfaceMask,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.Thickness,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.LobeInterpolation,
                UniversalSubsurfaceScatteringBlockFields.SurfaceDescription.LobeDerivation,
            };

            public static readonly BlockFieldDescriptor[] FragmentMeta = new BlockFieldDescriptor[]
            {
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.Emission,
                BlockFields.SurfaceDescription.Alpha,
                BlockFields.SurfaceDescription.AlphaClipThreshold,
            };

            public static readonly BlockFieldDescriptor[] FragmentDepthNormals = new BlockFieldDescriptor[]
            {
                BlockFields.SurfaceDescription.NormalOS,
                BlockFields.SurfaceDescription.NormalTS,
                BlockFields.SurfaceDescription.NormalWS,
                BlockFields.SurfaceDescription.Alpha,
                BlockFields.SurfaceDescription.AlphaClipThreshold,
            };

        }
        #endregion

        #region RequiredFields
        static class LitRequiredFields
        {
            public static readonly FieldCollection Forward = new FieldCollection()
            {
                StructFields.Attributes.uv1,                            // needed for meta vertex position
                StructFields.Varyings.positionWS,
                StructFields.Varyings.normalWS,
                StructFields.Varyings.tangentWS,                        // needed for vertex lighting
                StructFields.Varyings.viewDirectionWS,
                UniversalStructFields.Varyings.staticLightmapUV,
                UniversalStructFields.Varyings.dynamicLightmapUV,
                UniversalStructFields.Varyings.sh,
                UniversalStructFields.Varyings.fogFactorAndVertexLight, // fog and vertex lighting, vert input is dependency
                UniversalStructFields.Varyings.shadowCoord,             // shadow coord, vert input is dependency
            };

            public static readonly FieldCollection GBuffer = new FieldCollection()
            {
                StructFields.Attributes.uv1,                            // needed for meta vertex position
                StructFields.Varyings.positionWS,
                StructFields.Varyings.normalWS,
                StructFields.Varyings.tangentWS,                        // needed for vertex lighting
                StructFields.Varyings.viewDirectionWS,
                UniversalStructFields.Varyings.staticLightmapUV,
                UniversalStructFields.Varyings.dynamicLightmapUV,
                UniversalStructFields.Varyings.sh,
                UniversalStructFields.Varyings.fogFactorAndVertexLight, // fog and vertex lighting, vert input is dependency
                UniversalStructFields.Varyings.shadowCoord,             // shadow coord, vert input is dependency
            };

            public static readonly FieldCollection DepthNormals = new FieldCollection()
            {
                StructFields.Attributes.uv1,                            // needed for meta vertex position
                StructFields.Varyings.normalWS,
                StructFields.Varyings.tangentWS,                        // needed for vertex lighting
            };

            public static readonly FieldCollection Meta = new FieldCollection()
            {
                StructFields.Attributes.uv1,                            // needed for meta vertex position
                StructFields.Attributes.uv2,                            //needed for meta vertex position
            };
        }
        #endregion

        #region Defines
        static class LitDefines
        {
            public static readonly KeywordDescriptor ClearCoat = new KeywordDescriptor()
            {
                displayName = "Clear Coat",
                referenceName = "_CLEARCOAT 1",
                type = KeywordType.Boolean,
                definition = KeywordDefinition.ShaderFeature,
                scope = KeywordScope.Local,
            };

            //public static readonly DefineCollection ComplexLit = new DefineCollection()
            //{
            //    {ClearCoat, 1},
            //};
            public static readonly KeywordDescriptor SpecularSetup = new KeywordDescriptor()
            {
                displayName = "Specular Setup",
                referenceName = "_SPECULAR_SETUP",
                type = KeywordType.Boolean,
                definition = KeywordDefinition.ShaderFeature,
                scope = KeywordScope.Local,
                stages = KeywordShaderStage.Fragment
            };
        }
        #endregion

        #region Keywords
        static class LitKeywords
        {
            public static readonly KeywordDescriptor ReceiveShadowsOff = new KeywordDescriptor()
            {
                displayName = "Receive Shadows Off",
                referenceName = ShaderKeywordStrings._RECEIVE_SHADOWS_OFF,
                type = KeywordType.Boolean,
                definition = KeywordDefinition.ShaderFeature,
                scope = KeywordScope.Local,
            };

            public static readonly KeywordDescriptor ScreenSpaceAmbientOcclusion = new KeywordDescriptor()
            {
                displayName = "Screen Space Ambient Occlusion",
                referenceName = "_SCREEN_SPACE_OCCLUSION",
                type = KeywordType.Boolean,
                definition = KeywordDefinition.MultiCompile,
                scope = KeywordScope.Global,
            };

            public static readonly KeywordCollection Forward = new KeywordCollection
            {
                { ScreenSpaceAmbientOcclusion },
                { CoreKeywordDescriptors.StaticLightmap },
                { CoreKeywordDescriptors.DynamicLightmap },
                { CoreKeywordDescriptors.DirectionalLightmapCombined },
                { CoreKeywordDescriptors.MainLightShadows },
                { CoreKeywordDescriptors.AdditionalLights },
                { CoreKeywordDescriptors.AdditionalLightShadows },
                { CoreKeywordDescriptors.ReflectionProbeBlending },
                { CoreKeywordDescriptors.ReflectionProbeBoxProjection },
                { CoreKeywordDescriptors.ShadowsSoft },
                { CoreKeywordDescriptors.LightmapShadowMixing },
                { CoreKeywordDescriptors.ShadowsShadowmask },
                { CoreKeywordDescriptors.DBuffer },
                { CoreKeywordDescriptors.LightLayers },
                { CoreKeywordDescriptors.DebugDisplay },
                { CoreKeywordDescriptors.LightCookies },
                { CoreKeywordDescriptors.ClusteredRendering },
            };

            public static readonly KeywordCollection GBuffer = new KeywordCollection
            {
                { CoreKeywordDescriptors.StaticLightmap },
                { CoreKeywordDescriptors.DynamicLightmap },
                { CoreKeywordDescriptors.DirectionalLightmapCombined },
                { CoreKeywordDescriptors.MainLightShadows },
                { CoreKeywordDescriptors.ReflectionProbeBlending },
                { CoreKeywordDescriptors.ReflectionProbeBoxProjection },
                { CoreKeywordDescriptors.ShadowsSoft },
                { CoreKeywordDescriptors.LightmapShadowMixing },
                { CoreKeywordDescriptors.MixedLightingSubtractive },
                { CoreKeywordDescriptors.DBuffer },
                { CoreKeywordDescriptors.GBufferNormalsOct },
                { CoreKeywordDescriptors.LightLayers },
                { CoreKeywordDescriptors.RenderPassEnabled },
                { CoreKeywordDescriptors.DebugDisplay },
            };

            //public static readonly KeywordCollection Meta = new KeywordCollection
            //{
            //    { CoreKeywordDescriptors.SmoothnessChannel },
            //};
        }
        #endregion

        #region Includes
        static class LitIncludes
        {
            const string kShadows = "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl";
            const string kMetaInput = "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl";

            const string kGBuffer = "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl";
            const string kPBRGBufferPass = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl";
            const string kLightingMetaPass = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl";
            const string k2DPass = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl";

            const string kColor = "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl";
            const string kCore = "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl";
            const string kLighting = "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl";
            const string kTextureStack = "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl";

            const string kForwardPass = "Assets/ScreenSpaceSubsurfaceScattering/Editor/ShaderGraph/Skin/Includes/SubsurfaceScatteringPBRForwardPass.hlsl";
            const string kSplitForwardPass = "Assets/ScreenSpaceSubsurfaceScattering/Editor/ShaderGraph/Skin/Includes/SubsurfaceSplitForwardPass.hlsl";
            const string kSubsurfaceScatteringInputs = "Assets/ScreenSpaceSubsurfaceScattering/Editor/ShaderGraph/Skin/SkinUtility.hlsl";

            public static readonly IncludeCollection CorePregraph = new IncludeCollection
            {
                { kColor, IncludeLocation.Pregraph },
                { kCore, IncludeLocation.Pregraph },
                { kLighting, IncludeLocation.Pregraph },
                { kTextureStack, IncludeLocation.Pregraph },        // TODO: put this on a conditional
                { kSubsurfaceScatteringInputs, IncludeLocation.Pregraph},
            };

            public static readonly IncludeCollection Forward = new IncludeCollection
            {
                // Pre-graph
                { CorePregraph },
                { kShadows, IncludeLocation.Pregraph },
                { CoreIncludes.ShaderGraphPregraph },

                // Post-graph
                { CoreIncludes.CorePostgraph },
                { kForwardPass, IncludeLocation.Postgraph },
            };

            public static readonly IncludeCollection SplitForward = new IncludeCollection
            {
                // Pre-graph
                { CorePregraph },
                { kShadows, IncludeLocation.Pregraph },
                { CoreIncludes.ShaderGraphPregraph },

                // Post-graph
                { CoreIncludes.CorePostgraph },
                { kSplitForwardPass, IncludeLocation.Postgraph },
            };

            public static readonly IncludeCollection GBuffer = new IncludeCollection
            {
                // Pre-graph
                { CoreIncludes.CorePregraph },
                { kShadows, IncludeLocation.Pregraph },
                { CoreIncludes.ShaderGraphPregraph },

                // Post-graph
                { CoreIncludes.CorePostgraph },
                { kGBuffer, IncludeLocation.Postgraph },
                { kPBRGBufferPass, IncludeLocation.Postgraph },
            };

            public static readonly IncludeCollection Meta = new IncludeCollection
            {
                // Pre-graph
                { CoreIncludes.CorePregraph },
                { CoreIncludes.ShaderGraphPregraph },
                { kMetaInput, IncludeLocation.Pregraph },

                // Post-graph
                { CoreIncludes.CorePostgraph },
                { kLightingMetaPass, IncludeLocation.Postgraph },
            };

            public static readonly IncludeCollection _2D = new IncludeCollection
            {
                // Pre-graph
                { CoreIncludes.CorePregraph },
                { CoreIncludes.ShaderGraphPregraph },

                // Post-graph
                { CoreIncludes.CorePostgraph },
                { k2DPass, IncludeLocation.Postgraph },
            };
        }
        #endregion
    }

}