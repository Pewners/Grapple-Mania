using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssistedBouncing))]
public class AssistedBouncingEditor : Editor
{
    public SerializedProperty
        detectionRange,
        spread,
        maxCorrectionAngle,
        whatIsEnemy;

    private void OnEnable()
    {
        detectionRange = serializedObject.FindProperty("detectionRange");
        spread = serializedObject.FindProperty("spread");
        maxCorrectionAngle = serializedObject.FindProperty("maxCorrectionAngle");
        whatIsEnemy = serializedObject.FindProperty("whatIsEnemy");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.AssistedBouncing);


        // settings

        EditorUi.DrawHeader("Settings", 1);

        EditorGUILayout.PropertyField(detectionRange);
        EditorGUILayout.PropertyField(spread);
        EditorGUILayout.PropertyField(maxCorrectionAngle);
        EditorGUILayout.PropertyField(whatIsEnemy);

        serializedObject.ApplyModifiedProperties();
    }
}
