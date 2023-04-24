using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerReferences))]
public class PlayerReferencesEditor : Editor
{
    public SerializedProperty
        tr,
        rb,
        col,
        animator,
        input,
        abilities,
        actionPlayer,
        effectPlayer,
        animationPlayer,
        movementCore,
        detector,
        movement,
        cam,
        dashing,
        wallRunning,
        sliding,
        grappling,
        ledgeGrabbing,
        stats,
        inventory,
        combat,
        weaponHolder;

    private void OnEnable()
    {
        tr = serializedObject.FindProperty("tr");
        rb = serializedObject.FindProperty("rb");
        col = serializedObject.FindProperty("col");
        animator = serializedObject.FindProperty("animator");
        input = serializedObject.FindProperty("input");
        abilities = serializedObject.FindProperty("abilities");
        actionPlayer = serializedObject.FindProperty("actionPlayer");
        effectPlayer = serializedObject.FindProperty("effectPlayer");
        animationPlayer = serializedObject.FindProperty("animationPlayer");
        movementCore = serializedObject.FindProperty("movementCore");
        detector = serializedObject.FindProperty("detector");
        movement = serializedObject.FindProperty("movement");
        cam = serializedObject.FindProperty("cam");
        dashing = serializedObject.FindProperty("dashing");
        wallRunning = serializedObject.FindProperty("wallRunning");
        sliding = serializedObject.FindProperty("sliding");
        grappling = serializedObject.FindProperty("grappling");
        ledgeGrabbing = serializedObject.FindProperty("ledgeGrabbing");
        stats = serializedObject.FindProperty("stats");
        inventory = serializedObject.FindProperty("inventory");
        combat = serializedObject.FindProperty("combat");
        weaponHolder = serializedObject.FindProperty("weaponHolder");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.PlayerReferences);

        // components

        EditorUi.DrawHeader("Components", 1, EditorUi.CustomColor.white);
        EditorGUILayout.PropertyField(tr);
        EditorGUILayout.PropertyField(rb);
        EditorGUILayout.PropertyField(col);
        EditorGUILayout.PropertyField(animator);

        // main

        EditorUi.DrawHeader("Gameplay Logic", 2, EditorUi.CustomColor.white);
        EditorGUILayout.PropertyField(input);
        EditorGUILayout.PropertyField(abilities);
        EditorGUILayout.PropertyField(actionPlayer);
        EditorGUILayout.PropertyField(effectPlayer);
        EditorGUILayout.PropertyField(animationPlayer);

        // movement

        EditorUi.DrawHeader("Movement", 3, EditorUi.CustomColor.white);
        EditorGUILayout.PropertyField(movementCore);
        EditorGUILayout.PropertyField(detector);
        EditorGUILayout.PropertyField(movement);
        EditorGUILayout.PropertyField(cam);
        EditorGUILayout.PropertyField(dashing);
        EditorGUILayout.PropertyField(wallRunning);
        EditorGUILayout.PropertyField(sliding);
        EditorGUILayout.PropertyField(grappling);
        EditorGUILayout.PropertyField(ledgeGrabbing);

        // combat

        EditorUi.DrawHeader("Combat", 4, EditorUi.CustomColor.white);
        EditorGUILayout.PropertyField(stats);
        EditorGUILayout.PropertyField(inventory);
        EditorGUILayout.PropertyField(combat);
        EditorGUILayout.PropertyField(weaponHolder);

        serializedObject.ApplyModifiedProperties();
    }
}
