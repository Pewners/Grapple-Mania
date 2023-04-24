using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Homing))]
public class HomingEditor : Editor
{
    public SerializedProperty
        targetMode,
        preferTarget,
        raycastLength,
        targetFixedForwardDistance,
        targetFixedRightDistance,
        sphereCastRadius,
        whatIsTarget,
        enableFixedDistanceAsFallback,
        activateHomingAfterTime,
        useInitialVelocityAsSpeed,
        fixedSpeed,
        rotationMode,
        rotateSpeed,
        rotateSpeedIncrease,
        farDistanceRotateSpeed,
        closeDistanceRotateSpeed,
        maxDistancePredict,
        minDistancePredict,
        maxTimePrediction,
        deviationSpeed,
        deviationAmount,
        debugging;

    private void OnEnable()
    {
        targetMode  = serializedObject.FindProperty("targetMode");
        preferTarget  = serializedObject.FindProperty("preferTarget");
        raycastLength = serializedObject.FindProperty("raycastLength");
        targetFixedForwardDistance = serializedObject.FindProperty("targetFixedForwardDistance");
        targetFixedRightDistance = serializedObject.FindProperty("targetFixedRightDistance");
        sphereCastRadius = serializedObject.FindProperty("sphereCastRadius");
        whatIsTarget = serializedObject.FindProperty("whatIsTarget");
        enableFixedDistanceAsFallback = serializedObject.FindProperty("enableFixedDistanceAsFallback");
        activateHomingAfterTime = serializedObject.FindProperty("activateHomingAfterTime");
        useInitialVelocityAsSpeed = serializedObject.FindProperty("useInitialVelocityAsSpeed");
        fixedSpeed = serializedObject.FindProperty("fixedSpeed");
        rotationMode = serializedObject.FindProperty("rotationMode");
        rotateSpeed = serializedObject.FindProperty("rotateSpeed");
        rotateSpeedIncrease = serializedObject.FindProperty("rotateSpeedIncrease");
        farDistanceRotateSpeed = serializedObject.FindProperty("farDistanceRotateSpeed");
        closeDistanceRotateSpeed = serializedObject.FindProperty("closeDistanceRotateSpeed");
        minDistancePredict = serializedObject.FindProperty("minDistancePredict");
        maxDistancePredict = serializedObject.FindProperty("maxDistancePredict");
        maxTimePrediction = serializedObject.FindProperty("maxTimePrediction");
        deviationSpeed = serializedObject.FindProperty("deviationSpeed");
        deviationAmount = serializedObject.FindProperty("deviationAmount");
        debugging = serializedObject.FindProperty("enableDebugging");
    }

    public override void OnInspectorGUI()
    {
        Homing script = target as Homing;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Homing);


        // activation

        EditorUi.DrawHeader("Activation", 1);

        EditorGUILayout.PropertyField(activateHomingAfterTime);


        // target
        EditorUi.DrawHeader("Target", 2);

        EditorGUILayout.PropertyField(targetMode);

        /// find with raycast
        if(script.targetMode == Homing.TargetMode.FindWithRaycast)
        {
            EditorGUILayout.PropertyField(raycastLength);
            EditorGUILayout.PropertyField(sphereCastRadius);
            EditorGUILayout.PropertyField(whatIsTarget);

            EditorGUILayout.PropertyField(enableFixedDistanceAsFallback);

            if (script.enableFixedDistanceAsFallback)
            {
                EditorGUILayout.PropertyField(targetFixedForwardDistance);
                EditorGUILayout.PropertyField(targetFixedRightDistance);
            }
        }

        else if (script.targetMode == Homing.TargetMode.FixedDistance)
        {
            EditorGUILayout.PropertyField(targetFixedForwardDistance);
            EditorGUILayout.PropertyField(targetFixedRightDistance);
        }

        EditorGUILayout.PropertyField(preferTarget);


        // movement

        EditorUi.DrawHeader("Movement", 3);

        EditorGUILayout.PropertyField(useInitialVelocityAsSpeed);

        if(!script.useInitialVelocityAsSpeed)
        {
            EditorGUILayout.PropertyField(fixedSpeed);
        }


        // rotation

        EditorUi.DrawHeader("Rotation", 4);

        EditorGUILayout.PropertyField(rotationMode);

        /// constant speed
        if (script.rotationMode == Homing.RotationMode.constantSpeed)
        {
            EditorGUILayout.PropertyField(rotateSpeed);
        }

        /// constant speed increase
        if (script.rotationMode == Homing.RotationMode.constantIncrease)
        {
            EditorGUILayout.PropertyField(rotateSpeed);
            EditorGUILayout.PropertyField(rotateSpeedIncrease);
        }

        /// scale with distance
        if (script.rotationMode == Homing.RotationMode.scaleWithDistance)
        {
            EditorGUILayout.PropertyField(closeDistanceRotateSpeed);
            EditorGUILayout.PropertyField(farDistanceRotateSpeed);
        }


        // extensions

        EditorUi.DrawHeader("Extensions", 5);

        /// prediction

        EditorUi.DrawToggle(ref script.enablePrediction, "Enable Prediction");

        if(script.enablePrediction)
        {
            EditorGUILayout.PropertyField(minDistancePredict);
            EditorGUILayout.PropertyField(maxDistancePredict);
            EditorGUILayout.PropertyField(maxTimePrediction);
        }


        /// deviation

        EditorUi.DrawToggle(ref script.enableDeviation, "Enable Deviation");

        if(script.enableDeviation)
        {
            EditorGUILayout.PropertyField(deviationSpeed);
            EditorGUILayout.PropertyField(deviationAmount);
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(debugging);

        serializedObject.ApplyModifiedProperties();
    }
}
