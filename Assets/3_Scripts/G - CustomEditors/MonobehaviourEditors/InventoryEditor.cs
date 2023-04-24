using Dave;
using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    public SerializedProperty
        inventoryType,
        weaponHotbarSlots,
        weaponHotbar,
        startWeaponIndex,
        characterAbilitySet,
        wieldingState,
        ultimateBlaster,
        infiniteAmmo,
        interactionDistance,
        interactionSpherecastRadius,
        whatIsInteractable,
        handSlots;

    private void OnEnable()
    {
        // playerstats
        inventoryType = serializedObject.FindProperty("inventoryType");
        weaponHotbarSlots = serializedObject.FindProperty("weaponHotbarSlots");
        weaponHotbar = serializedObject.FindProperty("weaponHotbar");
        startWeaponIndex = serializedObject.FindProperty("startWeaponIndex");
        characterAbilitySet = serializedObject.FindProperty("characterAbilitySet");
        wieldingState = serializedObject.FindProperty("wieldingState");
        ultimateBlaster = serializedObject.FindProperty("ultimateBlaster");
        infiniteAmmo = serializedObject.FindProperty("infiniteAmmo");
        interactionDistance = serializedObject.FindProperty("interactionDistance");
        interactionSpherecastRadius = serializedObject.FindProperty("interactionSpherecastRadius");
        whatIsInteractable = serializedObject.FindProperty("whatIsInteractable");
        handSlots = serializedObject.FindProperty("handSlots");
    }

    public override void OnInspectorGUI()
    {
        Inventory script = target as Inventory;

        serializedObject.Update();

        string typeName = script.inventoryType.ToString();
        EditorUi.DrawComponentTitle(EditorUiData.Component.Inventory, EditorUi.CustomColor.blue, typeName + " Inventory");

        // inventory type

        EditorGUILayout.PropertyField(inventoryType);

        // skillset

        EditorUi.DrawHeader("Skillset", 1, EditorUi.CustomColor.white);

        EditorUi.DrawSubHeader("Abilities");

        EditorGUILayout.PropertyField(characterAbilitySet);

        EditorUi.DrawSubHeader("Weapons");

        EditorGUILayout.PropertyField(infiniteAmmo);
        EditorGUILayout.PropertyField(weaponHotbarSlots);
        EditorGUILayout.PropertyField(weaponHotbar);
        EditorGUILayout.PropertyField(startWeaponIndex);

        // interaction

        EditorUi.DrawHeader("Interaction", 2, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(interactionDistance);
        EditorGUILayout.PropertyField(interactionSpherecastRadius);
        EditorGUILayout.PropertyField(whatIsInteractable);

        // references

        EditorUi.DrawHeader("References", 3, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(ultimateBlaster);



        /// Properties applied here
        serializedObject.ApplyModifiedProperties();



        // display only

        EditorUi.DrawHeader("Display only", 4, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(handSlots);
        EditorGUILayout.PropertyField(wieldingState);

        EditorUi.DrawMonoBehaviourOutroSpace();
    }
}
