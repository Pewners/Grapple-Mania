using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PostProcessingEffect))]
public class PostProcessingEffectEditor : Editor
{
    public static string[] propertyNames = new string[]
    {
        "duration",
        "effectCurve",
        "m_Script"
    };

    public static PostProcessingEffectSerializedProperties GetSerializedProperties(SerializedObject serializedObject)
    {
        PostProcessingEffectSerializedProperties properties = new PostProcessingEffectSerializedProperties();
        properties.duration = serializedObject.FindProperty("duration");
        properties.effectCurve = serializedObject.FindProperty("effectCurve");
        return properties;
    }

    public static void DrawPostProcessingEffectEditor(PostProcessingEffectSerializedProperties properties, SerializedObject serializedObject)
    {
        EditorUi.DrawHeader("Effect Settings", 1, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(properties.duration);
        EditorGUILayout.PropertyField(properties.effectCurve);

        serializedObject.ApplyModifiedProperties();
    }
}

public class PostProcessingEffectSerializedProperties
{
    public SerializedProperty duration;
    public SerializedProperty effectCurve;
}