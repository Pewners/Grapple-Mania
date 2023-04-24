using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Connector))]
public class ConnectorEditor : Editor
{
    public SerializedProperty
        maxConnections,
        maxConnectionRange,
        connectionTarget,
        connectionPreference,
        whatIsProjectile,
        whatIsEnemy;

    private void OnEnable()
    {
        maxConnections = serializedObject.FindProperty("maxConnections");
        maxConnectionRange = serializedObject.FindProperty("maxConnectionRange");
        connectionTarget = serializedObject.FindProperty("connectionTarget");
        connectionPreference = serializedObject.FindProperty("connectionPreference");
        whatIsProjectile = serializedObject.FindProperty("whatIsProjectile");
        whatIsEnemy = serializedObject.FindProperty("whatIsEnemy");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Connector);


        // capacity

        EditorUi.DrawHeader("Capacity", 1);

        EditorGUILayout.PropertyField(maxConnections);
        EditorGUILayout.PropertyField(maxConnectionRange);


        // target

        EditorUi.DrawHeader("Target", 2);

        EditorGUILayout.PropertyField(connectionTarget);
        EditorGUILayout.PropertyField(connectionPreference);
        EditorGUILayout.PropertyField(whatIsProjectile);
        EditorGUILayout.PropertyField(whatIsEnemy);

        serializedObject.ApplyModifiedProperties();
    }
}
