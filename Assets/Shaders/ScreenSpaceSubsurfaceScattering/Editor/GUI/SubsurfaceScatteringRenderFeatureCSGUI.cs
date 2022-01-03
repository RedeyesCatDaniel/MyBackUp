using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

[CustomEditor(typeof(SubsurfaceScatteringRenderFeatureComputeShader))]
public class SubsurfaceScatteringRenderFeatureCSGUI : Editor
{
    SubsurfaceScatteringRenderFeatureComputeShader m_SRF;
    SerializedObject m_SerObj;
    SerializedProperty m_ProfilesObj;
    int useDisney;
    ReorderableList ProfileList;

    SerializedProperty m_SubsurfaceLayer;
    SerializedProperty m_DownSamplingMode;


    void OnEnable()
    {
        if (target == null) return;
        m_SRF = (SubsurfaceScatteringRenderFeatureComputeShader)target;
        m_SerObj = new SerializedObject(m_SRF);

        m_ProfilesObj = m_SerObj.FindProperty("profileManager");
        m_ProfilesObj = m_ProfilesObj.FindPropertyRelative("profilesInGUI");

        ProfileList = new ReorderableList(m_ProfilesObj.serializedObject, m_ProfilesObj, true, true, true, true);
        // Header
        ProfileList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Diffusion Profile List"); };
        // Element
        ProfileList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = ProfileList.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 1;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight + 1), element, GUIContent.none);
        };

        ProfileList.onAddCallback = (ReorderableList list) =>
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            m_SerObj.ApplyModifiedProperties();
            (m_SerObj.targetObject as SubsurfaceScatteringRenderFeatureComputeShader).profileManager.UpdateProfileListFromGUI();

        };

        ProfileList.onRemoveCallback = (ReorderableList list) =>
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            m_SerObj.ApplyModifiedProperties();
            (m_SerObj.targetObject as SubsurfaceScatteringRenderFeatureComputeShader).profileManager.UpdateProfileListFromGUI();

        };

        ProfileList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
        {
            m_SerObj.ApplyModifiedProperties();
            (m_SerObj.targetObject as SubsurfaceScatteringRenderFeatureComputeShader).profileManager.UpdateProfileListFromGUI();
        };

        m_SubsurfaceLayer = m_SerObj.FindProperty("settings").FindPropertyRelative("layer");
        //m_DownSamplingMode = m_SerObj.FindProperty("settings").FindPropertyRelative("downSamplingMode");

    }



    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        if (m_SerObj == null) return;

        EditorGUILayout.LabelField("Subsurface Scattering Render Feature Setting");
        EditorGUI.BeginChangeCheck();
        m_SerObj.Update();

        EditorGUILayout.Space();

        //Layer
        EditorGUILayout.PropertyField(m_SubsurfaceLayer, new GUIContent("Subsurface Layer"));
        EditorGUILayout.Space();

        
        //Reorderable List
        ProfileList.DoLayoutList();
        EditorGUILayout.Space();


        if (SubsurfaceScatteringProfileManager.needToUpdateGUI)
        {
            (m_SerObj.targetObject as SubsurfaceScatteringRenderFeatureComputeShader).profileManager.UpdateProfileListFromGUI();
            EditorUtility.SetDirty(m_SerObj.targetObject);
            m_SerObj.Update();
        }



        if (m_SerObj.hasModifiedProperties)
        {
            m_SerObj.ApplyModifiedProperties();
            (m_SerObj.targetObject as SubsurfaceScatteringRenderFeatureComputeShader).profileManager.UpdateProfileListFromGUI();
        }

        EditorGUI.EndChangeCheck();
    }

}
