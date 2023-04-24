using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RcLab;

[CustomEditor(typeof(PlayerInput))]
public class PlayerInputEditor : Editor
{
    public SerializedProperty
        keybinds,
        camInputData;

    private void OnEnable()
    {
        keybinds = serializedObject.FindProperty("keybinds");
        camInputData = serializedObject.FindProperty("camInputData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.PlayerInput);

        // variables

        EditorUi.DrawHeader("Input Settings", 1, EditorUi.CustomColor.white);
        EditorGUILayout.PropertyField(keybinds);
        EditorGUILayout.PropertyField(camInputData);

        EditorUi.DrawMonoBehaviourOutroSpace();

        serializedObject.ApplyModifiedProperties();
    }
}
