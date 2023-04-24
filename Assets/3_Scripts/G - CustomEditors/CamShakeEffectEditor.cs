using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CamShakeEffectEditor : Editor
{
    public static string[] propertyNames = new string[]
    {
        "duration",
        "m_Script"
    };

    public static CamShakeEffectSerializedProperties GetSerializedProperties(SerializedObject serializedObject)
    {
        CamShakeEffectSerializedProperties properties = new CamShakeEffectSerializedProperties();
        properties.duration = serializedObject.FindProperty("duration");
        return properties;
    }

    public static void DrawCamShakeEffectEditor(CamShakeEffectSerializedProperties properties, SerializedObject serializedObject)
    {
        EditorUi.DrawHeader("CamShake Effect Settings", 1, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(properties.duration);

        serializedObject.ApplyModifiedProperties();
    }
}

public class CamShakeEffectSerializedProperties
{
    public SerializedProperty
        duration;
}
