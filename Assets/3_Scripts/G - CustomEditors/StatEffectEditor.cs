using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatEffect))]
public class StatEffectEditor : Editor
{
    public SerializedProperty
        statEffectType,
        duration,
        stacks,
        maxReacheableStacks,
        maxReacheableDuration;


    private void OnEnable()
    {
        statEffectType = serializedObject.FindProperty("statEffectType");
        duration = serializedObject.FindProperty("duration");
        stacks = serializedObject.FindProperty("stacks");
        maxReacheableStacks = serializedObject.FindProperty("maxReacheableStacks");
        maxReacheableDuration = serializedObject.FindProperty("maxReacheableDuration");
    }

    public override void OnInspectorGUI()
    {
        StatEffect script = target as StatEffect;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.StatEffect, EditorUi.CustomColor.statEffectBlue);

        // settings

        EditorUi.DrawHeader("Settings", 1, EditorUi.CustomColor.lightBlueLs);

        EditorGUILayout.PropertyField(statEffectType);
        EditorGUILayout.PropertyField(duration);
        EditorGUILayout.PropertyField(stacks);

        // stacking

        EditorUi.DrawHeader("Stacking", 2, EditorUi.CustomColor.lightBlueLs);

        /// value stacking

        EditorUi.DrawToggle(ref script.enableValueStacking, "Enable Value Stacking");

        if (script.enableValueStacking)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(maxReacheableStacks);

            EditorGUI.indentLevel--;
        }

        /// duration stacking

        EditorUi.DrawToggle(ref script.enableDurationStacking, "Enable Duration Stacking");

        if (script.enableDurationStacking)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(maxReacheableDuration);

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
