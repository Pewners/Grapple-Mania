using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Laser))]
public class LaserEditor : Editor
{
    public SerializedProperty
        range,
        sphereCastRadius,
        damagePerTick,
        damageInterval,
        maxDuration,
        whatIsEnemy,
        whatIsWall,
        penetration,
        stayAtAttackPoint_override,
        onExpiredAction,
        onFireStopAction,
        lineRendererWidthCurve,
        lineRendererMat;

    private void OnEnable()
    {
        range = serializedObject.FindProperty("range");
        sphereCastRadius = serializedObject.FindProperty("sphereCastRadius");
        damagePerTick = serializedObject.FindProperty("damagePerTick");
        damageInterval = serializedObject.FindProperty("damageInterval");
        maxDuration = serializedObject.FindProperty("maxDuration");
        whatIsEnemy = serializedObject.FindProperty("whatIsEnemy");
        whatIsWall = serializedObject.FindProperty("whatIsWall");
        penetration = serializedObject.FindProperty("penetration");
        stayAtAttackPoint_override = serializedObject.FindProperty("stayAtAttackPoint_override");
        onExpiredAction = serializedObject.FindProperty("onExpiredAction");
        onFireStopAction = serializedObject.FindProperty("onFireStopAction");
        lineRendererWidthCurve = serializedObject.FindProperty("lineRendererWidthCurve");
        lineRendererMat = serializedObject.FindProperty("lineRendererMat");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Laser);


        // stats

        EditorUi.DrawHeader("Stats", 1);

        EditorGUILayout.PropertyField(damagePerTick);
        EditorGUILayout.PropertyField(damageInterval);
        EditorGUILayout.PropertyField(maxDuration);


        // detection

        EditorUi.DrawHeader("Detection", 2);

        EditorGUILayout.PropertyField(range);
        EditorGUILayout.PropertyField(sphereCastRadius);
        EditorGUILayout.PropertyField(whatIsEnemy);
        EditorGUILayout.PropertyField(whatIsWall);


        // extra settings

        EditorUi.DrawHeader("Extra Settings", 3);

        EditorGUILayout.PropertyField(penetration);
        EditorGUILayout.PropertyField(stayAtAttackPoint_override);


        // event actions

        EditorUi.DrawHeader("Event Actions", 4);

        EditorGUILayout.PropertyField(onExpiredAction);
        EditorGUILayout.PropertyField(onFireStopAction);


        // visuals

        EditorUi.DrawHeader("Visuals", 5);

        EditorGUILayout.PropertyField(lineRendererWidthCurve);
        EditorGUILayout.PropertyField(lineRendererMat);

        serializedObject.ApplyModifiedProperties();
    }
}
