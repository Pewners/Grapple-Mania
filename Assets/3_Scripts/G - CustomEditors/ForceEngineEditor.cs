using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(ForceEngine))]
public class ForceEngineEditor : Editor
{
    public SerializedProperty
        useVelocityForward,
        forwardForce,
        upwardForce,
        rightForce,
        forceMultiplierCurve,
        curveDuration,
        maxRbVelocity,
        engineStartDelay,
        engineStopDelay,
        useForceFadeout,
        fadeoutDuration,
        minForce,
        fadeoutMultiplier,
        pulseIntervall,
        offPulseDrag,
        torqueDirection,
        forceMode,
        engineMode;

    private void OnEnable()
    {
        useVelocityForward = serializedObject.FindProperty("useVelocityForward");
        forwardForce = serializedObject.FindProperty("forwardForce");
        upwardForce = serializedObject.FindProperty("upwardForce");
        rightForce = serializedObject.FindProperty("rightForce");
        forceMultiplierCurve = serializedObject.FindProperty("forceMultiplierCurve");
        curveDuration = serializedObject.FindProperty("curveDuration");
        maxRbVelocity = serializedObject.FindProperty("maxRbVelocity");
        engineStartDelay = serializedObject.FindProperty("engineStartDelay");
        engineStopDelay = serializedObject.FindProperty("engineStopDelay");
        useForceFadeout = serializedObject.FindProperty("useForceFadeout");
        fadeoutDuration = serializedObject.FindProperty("fadeoutDuration");
        minForce = serializedObject.FindProperty("minForce");
        pulseIntervall = serializedObject.FindProperty("pulseIntervall");
        offPulseDrag = serializedObject.FindProperty("offPulseDrag");
        torqueDirection = serializedObject.FindProperty("torqueDirection");
        forceMode = serializedObject.FindProperty("forceMode");
        engineMode = serializedObject.FindProperty("engineMode");
    }

    public override void OnInspectorGUI()
    {
        ForceEngine script = target as ForceEngine;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.ForceEngine);


        // engine setup

        EditorUi.DrawHeader("Engine Setup", 1);

        EditorGUILayout.PropertyField(engineStartDelay);
        EditorGUILayout.PropertyField(engineStopDelay);
        EditorGUILayout.PropertyField(useVelocityForward);
        EditorGUILayout.PropertyField(maxRbVelocity);
        EditorGUILayout.PropertyField(forceMode);
        EditorGUILayout.PropertyField(engineMode);

        if(script.engineMode == ForceEngine.EngineMode.pulsive)
        {
            EditorGUILayout.PropertyField(pulseIntervall);
            EditorGUILayout.PropertyField(offPulseDrag);
        }


        // engine force

        EditorUi.DrawHeader("Engine Force", 2);

        EditorGUILayout.PropertyField(forwardForce);
        EditorGUILayout.PropertyField(upwardForce);
        EditorGUILayout.PropertyField(rightForce);


        // force adjustments

        EditorUi.DrawHeader("Force Adjustments", 3);

        EditorGUILayout.PropertyField(forceMultiplierCurve);
        EditorGUILayout.PropertyField(curveDuration);

        /// force fadeout

        EditorUi.DrawToggle(ref script.useForceFadeout, "Enable Force Fadeout");

        if (script.useForceFadeout)
        {
            EditorGUILayout.PropertyField(fadeoutDuration);
            EditorGUILayout.PropertyField(minForce);
        }


        // rotation

        EditorUi.DrawHeader("Rotation", 4);

        EditorGUILayout.PropertyField(torqueDirection);


        serializedObject.ApplyModifiedProperties();
    }
}
