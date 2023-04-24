using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;

public class DavesWindow : EditorWindow
{
    public EditorUiData.Component componentToChange;
    [Range(0f, 800f)] public float height;
    [Range(0f, 100f)] public float alphaValue;
    public bool drawSpaceBeforeFirstVariable;

    [MenuItem("Window/Dave")]
    public static void ShowWindow()
    {
        GetWindow<DavesWindow>("Dave");
    }

    void OnGUI()
    {
        GUILayout.Label("Editor Inspector", EditorStyles.boldLabel);
        GUILayout.Label("Only saving for like 10min, Idk why", EditorStyles.boldLabel);

        componentToChange = (EditorUiData.Component)EditorGUILayout.EnumPopup("ComponentToEdit:", componentToChange);
        height = EditorGUILayout.FloatField("height", height);
        alphaValue = EditorGUILayout.FloatField("alphavalue", alphaValue);
        drawSpaceBeforeFirstVariable = EditorGUILayout.Toggle("drawSpaceBeforeFistVariable", drawSpaceBeforeFirstVariable);

        if (GUILayout.Button("Change Component Design"))
        {
            ChangeComponentDesign();
        }
    }

    void ChangeComponentDesign()
    {
        EditorUiData editorUiData = Resources.Load<EditorUiData>("EditorUiSettings");
        editorUiData.cd_componentDesingLookup[componentToChange].baseHeigth = height;
        editorUiData.cd_componentDesingLookup[componentToChange].bgAlphaValue = alphaValue;
        editorUiData.cd_componentDesingLookup[componentToChange].drawSpaceBeforeFirstVariable = drawSpaceBeforeFirstVariable;
    }
}
