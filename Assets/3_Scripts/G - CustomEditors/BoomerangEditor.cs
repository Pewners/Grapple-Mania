using Cinemachine.Editor;
using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Searcher;
using UnityEngine;

[CustomEditor(typeof(Boomerang))]
public class BoomerangEditor : Editor
{
    public SerializedProperty
        timeBeforeReturn,
        returnDuration,
        interpolation,
        returnOnCollision,
        destroyOnReturn;

    private void OnEnable()
    {
        timeBeforeReturn = serializedObject.FindProperty("timeBeforeReturn");
        returnDuration = serializedObject.FindProperty("returnDuration");
        interpolation = serializedObject.FindProperty("interpolation");
        returnOnCollision = serializedObject.FindProperty("returnOnCollision");
        destroyOnReturn = serializedObject.FindProperty("destroyOnReturn");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Boomerang);


        // settings

        EditorUi.DrawHeader("Settings", 1);

        EditorGUILayout.PropertyField(timeBeforeReturn);
        EditorGUILayout.PropertyField(returnDuration);
        EditorGUILayout.PropertyField(interpolation);


        // extensions

        EditorUi.DrawHeader("Extensions", 2);

        EditorGUILayout.PropertyField(returnOnCollision);
        EditorGUILayout.PropertyField(destroyOnReturn);

        serializedObject.ApplyModifiedProperties();
    }
}
