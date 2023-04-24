using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// not really a custom editor itself, but stores info on how to display action specific variables
public class ActionEditor : Editor
{
    public static ActionSerializedProperties GetSerializedProperties(SerializedObject serializedObject)
    {
        ActionSerializedProperties properties = new ActionSerializedProperties();
        properties.actionName = serializedObject.FindProperty("actionName");
        properties.actionType = serializedObject.FindProperty("actionType");
        properties.startDelay = serializedObject.FindProperty("startDelay");
        properties.skipActionDuration = serializedObject.FindProperty("skipActionDuration");
        properties.timeTillNextAction = serializedObject.FindProperty("timeTillNextAction");
        return properties;
    }

    public static void DrawActionEditor(ActionSerializedProperties properties, SerializedObject serializedObject)
    {
        EditorUi.DrawHeader("Action Settings", 1, EditorUi.CustomColor.white);
        
        EditorGUILayout.PropertyField(properties.actionName);
        EditorGUILayout.PropertyField(properties.actionType);
        EditorGUILayout.PropertyField(properties.startDelay);
        EditorGUILayout.PropertyField(properties.skipActionDuration);
        EditorGUILayout.PropertyField(properties.timeTillNextAction);

        serializedObject.ApplyModifiedProperties();
    }
}

public class ActionSerializedProperties
{
    public SerializedProperty
        actionName,
        actionType,
        startDelay,
        skipActionDuration,
        timeTillNextAction;
}
