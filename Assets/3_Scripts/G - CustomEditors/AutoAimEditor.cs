using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoAim))]
public class AutoAimEditor : Editor
{
    public SerializedProperty
        detectionRange,
        whatIsTarget,
        rangedWeaponContainer,
        maxLookUpAngle,
        maxLookDownAngle;


    private void OnEnable()
    {
        detectionRange = serializedObject.FindProperty("detectionRange");
        whatIsTarget = serializedObject.FindProperty("whatIsTarget");
        rangedWeaponContainer = serializedObject.FindProperty("rangedWeaponContainer");
        maxLookUpAngle = serializedObject.FindProperty("maxLookUpAngle");
        maxLookDownAngle = serializedObject.FindProperty("maxLookDownAngle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.AutoAim);


        // detection

        EditorUi.DrawHeader("Detection", 1);

        EditorGUILayout.PropertyField(detectionRange);
        EditorGUILayout.PropertyField(maxLookUpAngle);
        EditorGUILayout.PropertyField(maxLookDownAngle);
        EditorGUILayout.PropertyField(whatIsTarget);


        // attacking

        EditorUi.DrawHeader("Attacking", 2);

        EditorGUILayout.PropertyField(rangedWeaponContainer);

        serializedObject.ApplyModifiedProperties();
    }
}
