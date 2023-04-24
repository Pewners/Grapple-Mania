using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeleeAction))]
public class MeleeActionEditor : Editor
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
        handsNeeded = serializedObject.FindProperty("handsNeeded");
    }

    public override void OnInspectorGUI()
    {
        MeleeAction script = target as MeleeAction;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.MeleeAction, EditorUi.CustomColor.meleeActionOrange, script.actionName);

        // action settings

        ActionEditor.DrawActionEditor(actionSerializedProperties, serializedObject);

        // ranged

        EditorUi.DrawHeader("Melee", 2, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(weapon);
        EditorGUILayout.PropertyField(iterations);
        EditorGUILayout.PropertyField(handsNeeded);

        serializedObject.ApplyModifiedProperties();
    }
}
