using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Lingering))]
public class LingeringEditor : Editor
{
    public SerializedProperty
        damagePerTick,
        damageInterval,
        maxDuration,
        whatIsEnemy,
        onExpiredAction;


    private void OnEnable()
    {
        damagePerTick = serializedObject.FindProperty("damagePerTick");
        damageInterval = serializedObject.FindProperty("damageInterval");
        maxDuration = serializedObject.FindProperty("maxDuration");
        whatIsEnemy = serializedObject.FindProperty("whatIsEnemy");
        onExpiredAction = serializedObject.FindProperty("onExpiredAction");
    }

    public override void OnInspectorGUI()
    {
        Lingering script = target as Lingering;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Lingering, EditorUi.CustomColor.lightBlue);

        // settings

        EditorUi.DrawHeader("Settings", 1, EditorUi.CustomColor.lightBlueLs);

        EditorGUILayout.PropertyField(damagePerTick);
        EditorGUILayout.PropertyField(damageInterval);
        EditorGUILayout.PropertyField(maxDuration);
        EditorGUILayout.PropertyField(whatIsEnemy);

        // events

        EditorUi.DrawHeader("Events", 2, EditorUi.CustomColor.lightBlueLs);

        EditorGUILayout.PropertyField(onExpiredAction);

        serializedObject.ApplyModifiedProperties();
    }
}
