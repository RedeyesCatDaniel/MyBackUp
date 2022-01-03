// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

using System;
using UnityEngine;

namespace UnityEditor
{
    public abstract class SubsurfaceScatteringBaseGUI : ShaderGUI
    {
        protected SubsurfaceScatteringModel.Settings sssSettings;

        public SubsurfaceScatteringBaseGUI(){
          
        }

        protected enum WorkflowMode
        {
            Specular,
            Metallic,
            Dielectric
        }

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
        }

        public enum SmoothnessMapChannel
        {
            SpecularMetallicAlpha,
            AlbedoAlpha,
        }

        private static class SSSBaseStyles
        {
            // Subsurface
            public static GUIContent subsurfaceProfileText   = new GUIContent("Subsurface Profile", "A profile determines the shape of the blur filter.");
            public static GUIContent subsurfaceRadiusText    = new GUIContent("Subsurface Mask", "Determines the range of the blur.");
            public static GUIContent subsurfaceRadiusMapText = new GUIContent("Subsurface Mask Map", "Determines the range of the blur.");
            public static GUIContent thicknessText           = new GUIContent("Thickness", "If subsurface scattering is enabled, low values allow some light to be transmitted through the object.");
            public static GUIContent thicknessMapText        = new GUIContent("Thickness Map", "If subsurface scattering is enabled, low values allow some light to be transmitted through the object.");

            public static GUIContent uvSetLabel = new GUIContent("UV Set");

            public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
            public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
            public static GUIContent specularMapText = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");
            public static GUIContent metallicMapText = new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");
            public static GUIContent smoothnessText = new GUIContent("Smoothness", "Smoothness value");
            public static GUIContent smoothnessScaleText = new GUIContent("Smoothness", "Smoothness scale factor");
            public static GUIContent smoothnessMapChannelText = new GUIContent("Source", "Smoothness texture and channel");
            public static GUIContent highlightsText = new GUIContent("Specular Highlights", "Specular Highlights");
            public static GUIContent reflectionsText = new GUIContent("Reflections", "Glossy Reflections");
            public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map");
            public static GUIContent heightMapText = new GUIContent("Height Map", "Height Map (G)");
            public static GUIContent occlusionText = new GUIContent("Occlusion", "Occlusion (G)");
            public static GUIContent emissionText = new GUIContent("Color", "Emission (RGB)");
            public static GUIContent detailMaskText = new GUIContent("Detail Mask", "Mask for Secondary Maps (A)");
            public static GUIContent detailAlbedoText = new GUIContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
            public static GUIContent detailNormalMapText = new GUIContent("Normal Map", "Normal Map");

            public static GUIContent detailSmoothnessMapText = new GUIContent("Detail Smoothness", "Detail Smoothness");
            public static GUIContent occlusionColorBleedingText = new GUIContent("Color Bleeding AO", "Color Bleeding AO");
            public static GUIContent specularAOText = new GUIContent("Specular Occlusion", "Apply Powered AO as SO");

            public static string primaryMapsText = "Main Maps";
            public static string secondaryMapsText = "Secondary Maps";
            public static string forwardText = "Forward Rendering Options";
            public static string renderingMode = "Rendering Mode";
            public static string workFlow = "Workflow Mode";
            public static string advancedText = "Advanced Options";
            public static GUIContent emissiveWarning = new GUIContent("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");
            public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));

            public static readonly string[] workFlowNames = { "Specular", "Metallic" };

            public static GUIContent diffusionProfileNotInRenderFeature = new GUIContent("This Profile is not referenced in the Render Feature. To reference this profile, press Add");
        }

        //Subsurface
        protected SubsurfaceScatteringProfile subsurfaceProfile = null;
        protected SubsurfaceScatteringProfile m_subsurfaceProfile = null;

        protected MaterialProperty subsurfaceProfileID  = null;
        protected const string     kSubsurfaceProfileID = "_SubsurfaceProfile";
        protected MaterialProperty subsurfaceRadius     = null;
        protected const string     kSubsurfaceRadius    = "_SubsurfaceRadius";
        protected MaterialProperty subsurfaceRadiusMap  = null;
        protected const string     kSubsurfaceRadiusMap = "_SubsurfaceRadiusMap";
        protected MaterialProperty thickness            = null;
        protected const string     kThickness           = "_Thickness";
        protected MaterialProperty thicknessMap         = null;
        protected const string     kThicknessMap        = "_ThicknessMap";

        MaterialProperty blendMode = null;
        MaterialProperty albedoMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty specularMap = null;
        MaterialProperty specularColor = null;
        MaterialProperty metallicMap = null;
        MaterialProperty metallic = null;
        MaterialProperty smoothness = null;
        MaterialProperty smoothnessScale = null;
        MaterialProperty smoothnessMapChannel = null;
        MaterialProperty highlights = null;
        MaterialProperty reflections = null;
        MaterialProperty bumpScale = null;
        MaterialProperty bumpMap = null;
        MaterialProperty occlusionStrength = null;
        MaterialProperty occlusionMap = null;
        MaterialProperty heigtMapScale = null;
        MaterialProperty heightMap = null;
        MaterialProperty emissionColorForRendering = null;
        MaterialProperty emissionMap = null;
        MaterialProperty detailMask = null;
        MaterialProperty detailAlbedoMap = null;
        MaterialProperty detailNormalMapScale = null;
        MaterialProperty detailNormalMap = null;
        MaterialProperty detailSmoothnessMap = null;
        MaterialProperty uvSetSecondary = null;

        MaterialProperty colorBleedingAO = null;
        MaterialProperty specularOcclusion = null;

        MaterialProperty workflowMode = null;

        protected MaterialEditor m_MaterialEditor;
        protected WorkflowMode m_WorkflowMode = WorkflowMode.Specular;
        //ColorPickerHDRConfig m_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1 / 99f, 3f);

        bool m_FirstTimeApply = true;

        protected abstract void FindMiscSSSProperties(MaterialProperty[] props);
        protected abstract void MiscSSSPropertiesGUI(Material material);
        protected abstract void SetupMaterialKeywordsAndPassInternal(Material material, WorkflowMode workFlow);

        public void FindProperties(MaterialProperty[] props)
        {
            blendMode = FindProperty("_Mode", props);
            albedoMap = FindProperty("_BaseMap", props);
            albedoColor = FindProperty("_BaseColor", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            specularMap = FindProperty("_SpecGlossMap", props, false);
            specularColor = FindProperty("_SpecColor", props, false);
            metallicMap = FindProperty("_MetallicGlossMap", props, false);
            metallic = FindProperty("_Metallic", props, false);
            workflowMode = FindProperty("_WorkflowMode", props);

            if (workflowMode != null && (WorkflowMode)workflowMode.floatValue == WorkflowMode.Specular)
                m_WorkflowMode = WorkflowMode.Specular;
            else if (workflowMode != null && (WorkflowMode)workflowMode.floatValue == WorkflowMode.Metallic)
                m_WorkflowMode = WorkflowMode.Metallic;
            else
                m_WorkflowMode = WorkflowMode.Dielectric;

            smoothness = FindProperty("_Smoothness", props);
            smoothnessScale = FindProperty("_GlossMapScale", props, false);
            smoothnessMapChannel = FindProperty("_SmoothnessTextureChannel", props, false);
            highlights = FindProperty("_SpecularHighlights", props, false);
            reflections = FindProperty("_GlossyReflections", props, false);
            bumpScale = FindProperty("_BumpScale", props);
            bumpMap = FindProperty("_BumpMap", props);
            heigtMapScale = FindProperty("_Parallax", props);
            heightMap = FindProperty("_ParallaxMap", props);
            occlusionStrength = FindProperty("_OcclusionStrength", props);
            occlusionMap = FindProperty("_OcclusionMap", props);
            emissionColorForRendering = FindProperty("_EmissionColor", props);
            emissionMap = FindProperty("_EmissionMap", props);
            detailMask = FindProperty("_DetailMask", props);
            detailAlbedoMap = FindProperty("_DetailAlbedoMap", props);
            detailNormalMapScale = FindProperty("_DetailNormalMapScale", props);
            detailNormalMap = FindProperty("_DetailNormalMap", props);
            detailSmoothnessMap = FindProperty("_DetailSmoothnessMap", props);
            uvSetSecondary = FindProperty("_UVSec", props);

            colorBleedingAO = FindProperty("_OcclusionColorBleeding", props);
            specularOcclusion = FindProperty("_SpecularOcculsion", props, false);

            //Subsurface
            subsurfaceProfileID = FindProperty(kSubsurfaceProfileID, props);
            subsurfaceRadius    = FindProperty(kSubsurfaceRadius,    props);
            subsurfaceRadiusMap = FindProperty(kSubsurfaceRadiusMap, props);
            thickness           = FindProperty(kThickness,           props);
            thicknessMap        = FindProperty(kThicknessMap,        props);

            FindMiscSSSProperties(props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            
            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;

            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
            // material to a standard shader.
            // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
            if (m_FirstTimeApply)
            {
                SetupMaterialKeywordsAndPassInternal(material, m_WorkflowMode);
                m_FirstTimeApply = false;
            }

            ShaderPropertiesGUI(material);
        }

        private bool pendingForProfile = false;

        public void SSSPropertiesGUI(Material material){
            EditorGUILayout.Space();

            GUILayout.Label("Subsurface Scattering", EditorStyles.boldLabel);

            if(!pendingForProfile)
            {
                m_subsurfaceProfile = SubsurfaceScatteringProfileManager.GetSubsurfaceProfile((int)subsurfaceProfileID.floatValue);
            }

            m_subsurfaceProfile = EditorGUILayout.ObjectField(SSSBaseStyles.subsurfaceProfileText, m_subsurfaceProfile, typeof(SubsurfaceScatteringProfile), false) as SubsurfaceScatteringProfile;

            if(m_subsurfaceProfile == null)
            {
                //subsurfaceProfile = m_subsurfaceProfile;
                subsurfaceProfileID.floatValue = -1;
            }
            else
            {
                //int profileIndex = SubsurfaceScatteringRenderFeature.GetSSProfile(m_subsurfaceProfile);
                int profileIndex = SubsurfaceScatteringProfileManager.GetSubsurfaceProfileIndex(m_subsurfaceProfile);

                if (profileIndex < 0 )
                {
                    pendingForProfile = true;
                    DrawDiffusionProfileWarning();
                }
                else
                {
                    subsurfaceProfileID.floatValue = profileIndex;
                }
            }

            if (m_subsurfaceProfile != null && !pendingForProfile)
            {
                material.SetInt("_TransmissionFlags", (int)m_subsurfaceProfile.transmissionMode);

                if(m_subsurfaceProfile.texturingMode == SubsurfaceScatteringProfile.TexturingMode.PostScatter)
                {
                    material.EnableKeyword("_POST_SCATTER");
                }
                else
                {
                    material.DisableKeyword("_POST_SCATTER");

                }
                material.SetInt(kSubsurfaceProfileID, (int)subsurfaceProfileID.floatValue);
            }
            else
            {
                material.SetInt("_TransmissionFlags", (int)SubsurfaceScatteringProfile.TransmissionMode.None);
                subsurfaceProfileID.floatValue = -1;
            }

            //DrawDiffusionProfileWarning();

            m_MaterialEditor.ShaderProperty(subsurfaceRadius, SSSBaseStyles.subsurfaceRadiusText);
            m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.subsurfaceRadiusMapText, subsurfaceRadiusMap);
            m_MaterialEditor.ShaderProperty(thickness, SSSBaseStyles.thicknessText);
            m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.thicknessMapText, thicknessMap);


            EditorGUILayout.Space();
        }

        public void ShaderPropertiesGUI(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                BlendModePopup();

                WorkFlowModePopup();
                DetermineWorkflow(material, MaterialEditor.GetMaterialProperties(new Material[] { material }));

                // Primary properties
                GUILayout.Label(SSSBaseStyles.primaryMapsText, EditorStyles.boldLabel);
                DoAlbedoArea(material);
                DoSpecularMetallicArea();
                m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
                m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.heightMapText, heightMap, heightMap.textureValue != null ? heigtMapScale : null);
                m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? colorBleedingAO : null, occlusionMap.textureValue != null ? occlusionStrength: null);
                //m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.occlusionColorBleedingText, colorBleedingAO, occlusionMap.textureValue != null ? occlusionStrength : null);

                if(specularOcclusion != null)
                {
                    m_MaterialEditor.ShaderProperty(specularOcclusion, SSSBaseStyles.specularAOText);
                }
                else
                {
                    specularOcclusion.floatValue = 0;
                }

                m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.detailMaskText, detailMask);
                DoEmissionArea(material);
                EditorGUI.BeginChangeCheck();
                m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
                if (EditorGUI.EndChangeCheck())
                    emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake

                EditorGUILayout.Space();

                // Secondary properties
                GUILayout.Label(SSSBaseStyles.secondaryMapsText, EditorStyles.boldLabel);
                m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.detailAlbedoText, detailAlbedoMap);
                m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.detailNormalMapText, detailNormalMap, detailNormalMapScale);

                
                m_MaterialEditor.TextureScaleOffsetProperty(detailAlbedoMap);
                m_MaterialEditor.ShaderProperty(uvSetSecondary, SSSBaseStyles.uvSetLabel.text);

                //Alembic
                EditorGUILayout.Space();
                
                //Subsurface Scattering Propery Field
                SSSPropertiesGUI(material);
                MiscSSSPropertiesGUI(material);

                // Third properties
                GUILayout.Label(SSSBaseStyles.forwardText, EditorStyles.boldLabel);
                if (highlights != null)
                    m_MaterialEditor.ShaderProperty(highlights, SSSBaseStyles.highlightsText);
                if (reflections != null)
                    m_MaterialEditor.ShaderProperty(reflections, SSSBaseStyles.reflectionsText);
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendMode.targets)
                    SetupMaterialKeywordsAndPassInternal((Material)obj, m_WorkflowMode);
            }

            EditorGUILayout.Space();

            // NB renderqueue editor is not shown on purpose: we want to override it based on blend mode
            GUILayout.Label(SSSBaseStyles.advancedText, EditorStyles.boldLabel);
            m_MaterialEditor.EnableInstancingField();

            
        }

        internal void DrawDiffusionProfileWarning()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUIStyle wordWrap = new GUIStyle(EditorStyles.label);
                wordWrap.wordWrap = true;
                EditorGUILayout.LabelField(SSSBaseStyles.diffusionProfileNotInRenderFeature, wordWrap);
                if (GUILayout.Button("Add", GUILayout.ExpandHeight(true)))
                {

                    int profileIndex = SubsurfaceScatteringProfileManager.AddSubsurfaceProfile(m_subsurfaceProfile);

                    subsurfaceProfileID.floatValue = profileIndex;
                    pendingForProfile = false;
                }
            }
        }

        
        internal void DetermineWorkflow(Material material, MaterialProperty[] props)
        {
            if (workflowMode != null && (WorkflowMode)material.GetFloat("_WorkflowMode") == WorkflowMode.Specular)
                m_WorkflowMode = WorkflowMode.Specular;
            else if (workflowMode != null && (WorkflowMode)material.GetFloat("_WorkflowMode") == WorkflowMode.Metallic)
                m_WorkflowMode = WorkflowMode.Metallic;
            else
                m_WorkflowMode = WorkflowMode.Specular;
        }


        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
                return;
            }

            BlendMode blendMode = BlendMode.Opaque;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                blendMode = BlendMode.Cutout;
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                blendMode = BlendMode.Fade;
            }
            material.SetFloat("_Mode", (float)blendMode);

            DetermineWorkflow(material, MaterialEditor.GetMaterialProperties(new Material[] { material }));
            
            SetupMaterialKeywordsAndPassInternal(material, m_WorkflowMode);
        }

        void BlendModePopup()
        {
            EditorGUI.showMixedValue = blendMode.hasMixedValue;
            var mode = (BlendMode)blendMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (BlendMode)EditorGUILayout.Popup(SSSBaseStyles.renderingMode, (int)mode, SSSBaseStyles.blendNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                blendMode.floatValue = (float)mode;
            }

            EditorGUI.showMixedValue = false;
        }

        void WorkFlowModePopup()
        {
            EditorGUI.showMixedValue = workflowMode.hasMixedValue;
            var mode = (WorkflowMode)workflowMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (WorkflowMode)EditorGUILayout.Popup(SSSBaseStyles.workFlow, (int)mode, SSSBaseStyles.workFlowNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Workflow Mode");
                workflowMode.floatValue = (float)mode;
            }

            EditorGUI.showMixedValue = false;


        }

        void DoAlbedoArea(Material material)
        {
            m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.albedoText, albedoMap, albedoColor);
            if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
            {
                m_MaterialEditor.ShaderProperty(alphaCutoff, SSSBaseStyles.alphaCutoffText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
            }
        }

        void DoEmissionArea(Material material)
        {
            // Emission for GI?
            if (m_MaterialEditor.EmissionEnabledProperty())
            {
                bool hadEmissionTexture = emissionMap.textureValue != null;

                // Texture and HDR color controls
                //m_MaterialEditor.TexturePropertyWithHDRColor(SSSBaseStyles.emissionText, emissionMap, emissionColorForRendering, m_ColorPickerHDRConfig, false);
                m_MaterialEditor.TexturePropertyWithHDRColor(SSSBaseStyles.emissionText, emissionMap, emissionColorForRendering,false);

                // If texture was assigned and color was black set color to white
                float brightness = emissionColorForRendering.colorValue.maxColorComponent;
                if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                    emissionColorForRendering.colorValue = Color.white;

                // change the GI flag and fix it up with emissive as black if necessary
                m_MaterialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
            }
        }

        

        void DoSpecularMetallicArea()
        {
            bool hasGlossMap = false;
            if (m_WorkflowMode == WorkflowMode.Specular)
            {
                hasGlossMap = specularMap.textureValue != null;
                //TODO: Add specularmap
                m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.specularMapText, specularMap, hasGlossMap ? null : specularColor);
            }
            else if (m_WorkflowMode == WorkflowMode.Metallic)
            {
                hasGlossMap = metallicMap.textureValue != null;
                m_MaterialEditor.TexturePropertySingleLine(SSSBaseStyles.metallicMapText, metallicMap, hasGlossMap ? null : metallic);
            }

            bool showSmoothnessScale = hasGlossMap;
            if (smoothnessMapChannel != null)
            {
                int smoothnessChannel = (int)smoothnessMapChannel.floatValue;
                if (smoothnessChannel == (int)SmoothnessMapChannel.AlbedoAlpha)
                    showSmoothnessScale = true;
            }

            int indentation = 2; // align with labels of texture properties
            m_MaterialEditor.ShaderProperty(showSmoothnessScale ? smoothness : smoothness, showSmoothnessScale ? SSSBaseStyles.smoothnessScaleText : SSSBaseStyles.smoothnessText, indentation);

            ++indentation;
            if (smoothnessMapChannel != null)
                m_MaterialEditor.ShaderProperty(smoothnessMapChannel, SSSBaseStyles.smoothnessMapChannelText, indentation);
        }

        public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
            }
        }

        static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
        {
            int ch = (int)material.GetFloat("_SmoothnessTextureChannel");
            if (ch == (int)SmoothnessMapChannel.AlbedoAlpha)
                return SmoothnessMapChannel.AlbedoAlpha;
            else
                return SmoothnessMapChannel.SpecularMetallicAlpha;
        }

        static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
        {
            SetKeyword(material, "_SPECULAROCCLUSION", material.GetFloat("_SpecularOcculsion") == 1);
            SetKeyword(material, "_DUOSPECULARLOBE", material.GetFloat("_DualSpecularLobe") == 1);
            // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
            // (MaterialProperty value might come from renderer material property block)
            SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
            SetKeyword(material, "_SPECULAR_SETUP", workflowMode == WorkflowMode.Specular);
            if (workflowMode == WorkflowMode.Specular)
            {
                SetKeyword(material, "_METALLICSPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
                
            }
            else if (workflowMode == WorkflowMode.Metallic)
            {
                
                SetKeyword(material, "_METALLICSPECGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
            }

            SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));

            SetKeyword(material, "_DETAIL_SCALED", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));
            SetKeyword(material, "_DETAIL_MULX2", false);
            //SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));
            SetKeyword(material, "_DETAIL_AlbedoMap", material.GetTexture("_DetailAlbedoMap"));
            SetKeyword(material, "_DETAIL_SMOOTHNESS", material.GetTexture("_DetailSmoothnessMap")); //Have to separate

            SetKeyword(material, "_THICKNESSMAP", material.GetTexture("_ThicknessMap"));
            SetKeyword(material, "_SUBSURFACE_RADIUS_MAP", material.GetTexture("_SubsurfaceRadiusMap"));

            SetKeyword(material, "_OCCLUSIONMAP", material.GetTexture("_OcclusionMap"));

            // A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
            // or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
            // The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
            MaterialEditor.FixupEmissiveFlag(material);
            bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
            SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

            if (material.HasProperty("_SmoothnessTextureChannel"))
            {
                SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
            }
        }

        protected static void MaterialChanged(Material material, WorkflowMode workflowMode)
        {
            SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));

            //SetupSSSKeyword(material);
            SetMaterialKeywords(material, workflowMode);
        }

        protected static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }
    }
} // namespace UnityEditor
