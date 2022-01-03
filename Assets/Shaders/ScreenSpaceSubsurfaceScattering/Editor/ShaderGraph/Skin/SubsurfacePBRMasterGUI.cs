using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Linq;

namespace UnityEditor.ShaderGraph
{
    class SubsurfacePBRMasterGUI : ShaderGUI
    {
        private bool pendingForProfile = false;
        //Subsurface
        protected SubsurfaceScatteringProfile subsurfaceProfile = null;
        protected SubsurfaceScatteringProfile m_subsurfaceProfile = null;
        protected MaterialProperty subsurfaceProfileID = null;
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            materialEditor.PropertiesDefaultGUI(props);

            Material material = materialEditor.target as Material;

            if (materialEditor.EmissionEnabledProperty())
            { 
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            }
        }

        public static GUIContent subsurfaceProfileText = new GUIContent("Subsurface Profile", "A profile determines the shape of the blur filter.");
        public static GUIContent diffusionProfileNotInRenderFeature = new GUIContent("This Profile is not referenced in the Render Feature. To reference this profile, press Add");
        protected const string kSubsurfaceProfileID = "_DiffusionProfileHash";

        public static bool IsSupported(MaterialEditor materialEditor)
        {
            return materialEditor.targets.Any(o => {
                Material m = o as Material;
                return !m.HasProperty("_DiffusionProfileAsset") || !m.HasProperty("_DiffusionProfileHash");
            });
        }

        public void SSSPropertiesGUI(Material material, uint hash)
        {
            EditorGUILayout.Space();

            GUILayout.Label("Subsurface Scattering", EditorStyles.boldLabel);

            if (!pendingForProfile)
            {
                //m_subsurfaceProfile = SubsurfaceScatteringRenderFeature.GetSSProfileByIndex((int)subsurfaceProfileID.floatValue);
                m_subsurfaceProfile = SubsurfaceScatteringProfileManager.GetSubsurfaceProfile((int)hash);
            }

            m_subsurfaceProfile = EditorGUILayout.ObjectField(subsurfaceProfileText, m_subsurfaceProfile, typeof(SubsurfaceScatteringProfile), false) as SubsurfaceScatteringProfile;

            if (m_subsurfaceProfile == null)
            {
                //subsurfaceProfile = m_subsurfaceProfile;
                subsurfaceProfileID.floatValue = -1;
            }
            else
            {
                //int profileIndex = SubsurfaceScatteringRenderFeature.GetSSProfile(m_subsurfaceProfile);
                int profileIndex = SubsurfaceScatteringProfileManager.GetSubsurfaceProfileIndex(m_subsurfaceProfile);

                if (profileIndex < 0)
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

                if (m_subsurfaceProfile.texturingMode == SubsurfaceScatteringProfile.TexturingMode.PostScatter)
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

            


            EditorGUILayout.Space();
        }

        internal void DrawDiffusionProfileWarning()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUIStyle wordWrap = new GUIStyle(EditorStyles.label);
                wordWrap.wordWrap = true;
                EditorGUILayout.LabelField(diffusionProfileNotInRenderFeature, wordWrap);
                if (GUILayout.Button("Add", GUILayout.ExpandHeight(true)))
                {

                    int profileIndex = SubsurfaceScatteringProfileManager.AddSubsurfaceProfile(m_subsurfaceProfile);

                    subsurfaceProfileID.floatValue = profileIndex;
                    pendingForProfile = false;
                }
            }
        }
    }
}
