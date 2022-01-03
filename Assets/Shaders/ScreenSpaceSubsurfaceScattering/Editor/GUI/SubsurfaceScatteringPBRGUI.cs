using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SubsurfaceScatteringPBRGUI : SubsurfaceScatteringBaseGUI {

	public static class SSSSkinStyles{
        public static GUIContent specularLobeInterpolationText       = new GUIContent("Lobe Interpolation", "");
		public static GUIContent secondLobeRoughnessDerivationText   = new GUIContent("Second Lobe Smoothness Derivation", "");
		public static GUIContent dualSpecularLobeText = new GUIContent("Dual Specular Lobe", "Apply two specular lobes");
	}

	protected MaterialProperty specularLobeInterpolation = null;
	protected const string kSpecularLobeInterpolation = "_SpecularLobeInterpolation";
	protected MaterialProperty secondLobeRoughnessDerivation = null;
	protected const string kSecondLobeRoughnessDerivation = "_SecondLobeRoughnessDerivation";
	MaterialProperty duoSpecularLobes = null;


	protected override void FindMiscSSSProperties(MaterialProperty[] props){
		specularLobeInterpolation    = FindProperty(kSpecularLobeInterpolation, props);
		secondLobeRoughnessDerivation = FindProperty(kSecondLobeRoughnessDerivation, props);
		duoSpecularLobes = FindProperty("_DualSpecularLobe", props);
	}

	protected override void MiscSSSPropertiesGUI(Material material){
		GUILayout.Label("Skin Options", EditorStyles.boldLabel);
		if (duoSpecularLobes != null)
		{
			m_MaterialEditor.ShaderProperty(duoSpecularLobes, SSSSkinStyles.dualSpecularLobeText);
		}
		else
		{
			duoSpecularLobes.floatValue = 0;
		}

		if(duoSpecularLobes.floatValue == 1)
        {
			m_MaterialEditor.ShaderProperty(specularLobeInterpolation, SSSSkinStyles.specularLobeInterpolationText);
			m_MaterialEditor.ShaderProperty(secondLobeRoughnessDerivation, SSSSkinStyles.secondLobeRoughnessDerivationText);
		}

	}

	protected override void SetupMaterialKeywordsAndPassInternal(Material material, WorkflowMode workFlow){
		SetupMaterialKeywordsAndPass(material, workFlow);
	}

	static protected void SetupMaterialKeywordsAndPass(Material material, WorkflowMode workFlow){
		MaterialChanged(material, workFlow);
	}

}	