using Dave;
using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Combat))]
public class CombatEditor : Editor
{
    public SerializedProperty
        combatType,
        locked,
        cam,
        camRecoil,
        weaponHolder,
        raycastInitialAccelThreshold,
        whatIsTarget,
        state,
        moveSpreadMultiplier,
        moveSpreadMultiplierChangeSpeed,
        rbSpeedThreshold,
        debuggingEnabled;

    private void OnEnable()
    {
        // playerstats
        combatType = serializedObject.FindProperty("combatType");
        locked = serializedObject.FindProperty("locked");
        cam = serializedObject.FindProperty("cam");
        camRecoil = serializedObject.FindProperty("camRecoil");
        weaponHolder = serializedObject.FindProperty("weaponHolder");
        raycastInitialAccelThreshold = serializedObject.FindProperty("raycastInitialAccelThreshold");
        whatIsTarget = serializedObject.FindProperty("whatIsTarget");
        state = serializedObject.FindProperty("state");
        moveSpreadMultiplier = serializedObject.FindProperty("moveSpreadMultiplier");
        moveSpreadMultiplierChangeSpeed = serializedObject.FindProperty("moveSpreadMultiplierChangeSpeed");
        rbSpeedThreshold = serializedObject.FindProperty("rbSpeedThreshold");
        debuggingEnabled = serializedObject.FindProperty("debuggingEnabled");
    }

    public override void OnInspectorGUI()
    {
        Combat script = target as Combat;

        serializedObject.Update();

        string typeName = script.combatType.ToString();
        EditorUi.DrawComponentTitle(EditorUiData.Component.Combat, EditorUi.CustomColor.rangedActionOrange, typeName + " Combat");

        // inventory type

        EditorGUILayout.PropertyField(combatType);
        EditorGUILayout.PropertyField(locked);

        // ranged combat

        EditorUi.DrawHeader("Ranged Combat", 1, EditorUi.CustomColor.white);

        EditorUi.DrawSubHeader("Raycasting");

        EditorGUILayout.PropertyField(raycastInitialAccelThreshold);
        EditorGUILayout.PropertyField(whatIsTarget);

        EditorUi.DrawSubHeader("Spread");

        EditorGUILayout.PropertyField(moveSpreadMultiplier);
        EditorGUILayout.PropertyField(moveSpreadMultiplierChangeSpeed);
        EditorGUILayout.PropertyField(rbSpeedThreshold);
        

        // references

        EditorUi.DrawHeader("References", 2, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(cam);
        EditorGUILayout.PropertyField(camRecoil);
        EditorGUILayout.PropertyField(weaponHolder);

        // debugging and display

        EditorUi.DrawHeader("Debugging and Display", 3, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(debuggingEnabled);



        /// Properties applied here
        serializedObject.ApplyModifiedProperties();



        EditorGUILayout.PropertyField(state);

        EditorUi.DrawMonoBehaviourOutroSpace();
    }
}
