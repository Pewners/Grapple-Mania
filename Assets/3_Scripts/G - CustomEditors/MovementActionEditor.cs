using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementAction))]
public class MovementActionEditor : Editor
{
    public ActionSerializedProperties actionSerializedProperties;

    public SerializedProperty
        movementActionType;

    private void OnEnable()
    {
        actionSerializedProperties = ActionEditor.GetSerializedProperties(serializedObject);

        movementActionType = serializedObject.FindProperty("movementActionType");
    }

    public override void OnInspectorGUI()
    {
        MovementAction script = target as MovementAction;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.MovementAction, EditorUi.CustomColor.movementActionOrange, script.actionName);

        // action settings

        ActionEditor.DrawActionEditor(actionSerializedProperties, serializedObject);

        // ranged

        EditorUi.DrawHeader("Movement", 2, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(movementActionType);

        serializedObject.ApplyModifiedProperties();
    }
}
