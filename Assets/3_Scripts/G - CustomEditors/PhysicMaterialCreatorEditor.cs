using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysicMaterialCreator))]
public class PhysicMaterialCreatorEditor : Editor
{
    public SerializedProperty
        bounciness,
        friction,
        stickToSurface,
        extraBounceForce;

    private void OnEnable()
    {
        bounciness = serializedObject.FindProperty("bounciness");
        friction = serializedObject.FindProperty("friction");
        stickToSurface = serializedObject.FindProperty("stickToSurface");
        extraBounceForce = serializedObject.FindProperty("extraBounceForce");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.PhysicsMaterialCreator, EditorUi.CustomColor.green);


        // settings

        EditorUi.DrawHeader("Settings", 1);

        EditorGUILayout.PropertyField(bounciness);
        EditorGUILayout.PropertyField(friction);
        EditorGUILayout.PropertyField(stickToSurface);
        EditorGUILayout.PropertyField(extraBounceForce);

        serializedObject.ApplyModifiedProperties();
    }
}
