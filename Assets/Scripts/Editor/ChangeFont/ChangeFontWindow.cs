using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class ChangeFontWindow : EditorWindow
{
    [MenuItem("Tools/更换字体")]
    public static void Open()
    {
        EditorWindow.GetWindow(typeof(ChangeFontWindow));
    }

    TMP_FontAsset toChange;
    static TMP_FontAsset toChangeFont;
    FontStyle toFontStyle;
    static FontStyle toChangeFontStyle;

    void OnGUI()
    {
        toChange = (TMP_FontAsset)EditorGUILayout.ObjectField(toChange, typeof(TMP_FontAsset), true, GUILayout.MinWidth(100f));
        toChangeFont = toChange;
        toFontStyle = (FontStyle)EditorGUILayout.EnumPopup(toFontStyle, GUILayout.MinWidth(100f));
        toChangeFontStyle = toFontStyle;
        if (GUILayout.Button("更换"))
        {
            Change();
        }
    }

    public static void Change()
    {
        var tmpArray = GameObject.FindObjectsOfType<TextMeshProUGUI>();
        if (tmpArray.Length == 0)
        {
            Debug.Log("NO Canvas");
            return;
        }
        //TextMeshProUGUI[] tArray = canvas.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < tmpArray.Length; i++)
        {
            Debug.Log(tmpArray[i].name);
            TextMeshProUGUI t = tmpArray[i];//tArray[i].GetComponent<TextMeshProUGUI>();
            if (t)
            {
                //如果不加这个代码  在做完更改后 自己随便修改下场景里物体的状态 在保存就好了 ，不然他会觉得没变 就不会保存 就失败了
                Undo.RecordObject(t, t.gameObject.name);
                t.font = toChangeFont;
                t.fontStyle = (FontStyles)toChangeFontStyle;
                //刷新下
                EditorUtility.SetDirty(t);
            }
        }
        Debug.Log("Succed");
    }
}