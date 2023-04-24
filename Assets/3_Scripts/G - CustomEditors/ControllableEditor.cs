using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;

[CustomEditor(typeof(Controllable))]
public class ControllableEditor : Editor
{
    public SerializedProperty
        activationDelay,
        moveCameraWithProjectile,
        lockPlayerMovement,
        controlMode,
        maxSpeed,
        moveForce,
        forwardThrust,
        timeUntillMaxSpeed,
        allowedDirections,
        whatIsPlayer;

    private void OnEnable()
    {
        activationDelay = serializedObject.FindProperty("activationDelay");
        moveCameraWithProjectile = serializedObject.FindProperty("moveCameraWithProjectile");
        lockPlayerMovement = serializedObject.FindProperty("lockPlayerMovement");
        controlMode = serializedObject.FindProperty("controlMode");
        maxSpeed = serializedObject.FindProperty("maxSpeed");
        moveForce = serializedObject.FindProperty("moveForce");
        forwardThrust = serializedObject.FindProperty("forwardThrust");
        timeUntillMaxSpeed = serializedObject.FindProperty("timeUntillMaxSpeed");
        allowedDirections = serializedObject.FindProperty("allowedDirections");
        whatIsPlayer = serializedObject.FindProperty("whatIsPlayer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Controllable);


        // settings

        EditorUi.DrawHeader("Settings", 1);

        EditorGUILayout.PropertyField(activationDelay);
        EditorGUILayout.PropertyField(moveCameraWithProjectile);
        EditorGUILayout.PropertyField(lockPlayerMovement);
        EditorGUILayout.PropertyField(whatIsPlayer);


        // movement

        EditorUi.DrawHeader("Movement", 2);

        EditorGUILayout.PropertyField(controlMode);
        EditorGUILayout.PropertyField(forwardThrust);
        EditorGUILayout.PropertyField(moveForce);
        EditorGUILayout.PropertyField(maxSpeed);
        EditorGUILayout.PropertyField(timeUntillMaxSpeed);
        EditorGUILayout.PropertyField(allowedDirections);

        serializedObject.ApplyModifiedProperties();
    }
}
