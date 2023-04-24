using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Vanishing))]
public class VanishingEditor : Editor
{
    public SerializedProperty
        vanishStartDelay,
        vanishDuration,
        componentsToDeactivate,
        gameObjectsToDeactivate;

    private void OnEnable()
    {
        vanishStartDelay = serializedObject.FindProperty("vanishStartDelay");
        vanishDuration = serializedObject.FindProperty("vanishDuration");
        componentsToDeactivate = serializedObject.FindProperty("componentsToDeactivate");
        gameObjectsToDeactivate = serializedObject.FindProperty("gameObjectsToDeactivate");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Vanishing, EditorUi.CustomColor.purple);


        // settings

        EditorUi.DrawHeader("Settings", 1); 

        EditorGUILayout.PropertyField(vanishStartDelay);
        EditorGUILayout.PropertyField(vanishDuration);
        EditorGUILayout.PropertyField(componentsToDeactivate);
        EditorGUILayout.PropertyField(gameObjectsToDeactivate);


        serializedObject.ApplyModifiedProperties();
    }
}
