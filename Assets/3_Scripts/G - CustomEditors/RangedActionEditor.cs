using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RangedAction))]
public class RangedActionEditor : Editor
{
    public ActionSerializedProperties actionSerializedProperties;

    public SerializedProperty
        weapon,
        iterations,
        handsNeeded;

    private void OnEnable()
    {
        actionSerializedProperties = ActionEditor.GetSerializedProperties(serializedObject);

        weapon = serializedObject.FindProperty("weapon");
        iterations = serializedObject.FindProperty("iterations");
    }

    public override void OnInspectorGUI()
    {
        RangedAction script = target as RangedAction;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.RangedAction, EditorUi.CustomColor.rangedActionOrange, script.actionName);

        // action settings

        ActionEditor.DrawActionEditor(actionSerializedProperties, serializedObject);

        // ranged

        EditorUi.DrawHeader("Ranged", 2, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(weapon);
        EditorGUILayout.PropertyField(iterations);

        serializedObject.ApplyModifiedProperties();
    }
}
