using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ability))]
public class AbilityEditor : Editor
{
    public SerializedProperty
        abilityName,
        isWeapon,
        weaponType,
        simpleWeapon,
        actions,
        effects,
        cooldown,
        enableComboSystem,
        comboContinuation,
        stopComboTimer,
        inputHandling,
        interruptionHandling,
        overrideInterruptionSettings,
        canAlwaysInterrupt,
        canNeverInterrupt,
        priority,
        overrideHandsNeeded,
        fixedHandsNeeded,
        blasterType,
        enableUiDisplay,
        abilityIcon,
        abilityColor;


    // complexWeaponData
    public SerializedProperty
        overallMagazine,
        handsNeededComplexWeapon,
        spawnOnlyOnMouseDown;


    private void OnEnable()
    {
        // ability
        abilityName = serializedObject.FindProperty("abilityName");
        isWeapon = serializedObject.FindProperty("isWeapon");
        weaponType = serializedObject.FindProperty("weaponType");
        simpleWeapon = serializedObject.FindProperty("weapon");
        actions = serializedObject.FindProperty("actions");
        cooldown = serializedObject.FindProperty("cooldown");
        enableComboSystem = serializedObject.FindProperty("enableComboSystem");
        comboContinuation = serializedObject.FindProperty("comboContinuation");
        stopComboTimer = serializedObject.FindProperty("stopComboTimer");
        inputHandling = serializedObject.FindProperty("inputHandling");
        interruptionHandling = serializedObject.FindProperty("interruptionHandling");
        overrideInterruptionSettings = serializedObject.FindProperty("overrideInterruptionSettings");
        canAlwaysInterrupt = serializedObject.FindProperty("canAlwaysInterrupt");
        canNeverInterrupt = serializedObject.FindProperty("canNeverInterrupt");
        priority = serializedObject.FindProperty("priority");
        overrideHandsNeeded = serializedObject.FindProperty("overrideHandsNeeded");
        fixedHandsNeeded = serializedObject.FindProperty("fixedHandsNeeded");
        blasterType = serializedObject.FindProperty("blasterType");
        enableUiDisplay = serializedObject.FindProperty("enableUiDisplay");
        abilityIcon = serializedObject.FindProperty("abilityIcon");
        abilityColor = serializedObject.FindProperty("abilityColor");

        // effect playlist
        effects = serializedObject.FindProperty("effectPlaylist").FindPropertyRelative("effects");

        // complexWeaponData
        overallMagazine = serializedObject.FindProperty("complexWeaponData").FindPropertyRelative("magazine");
        handsNeededComplexWeapon = serializedObject.FindProperty("complexWeaponData").FindPropertyRelative("handsNeeded");
        spawnOnlyOnMouseDown = serializedObject.FindProperty("complexWeaponData").FindPropertyRelative("spawnOnlyOnMouseDown");

        (target as Ability).effectPlaylist = new EffectPlaylist();
    }

    public override void OnInspectorGUI()
    {
        Ability script = target as Ability;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Ability, EditorUi.CustomColor.abilityPurple, script.abilityName);

        // settings

        EditorUi.DrawHeader("Settings", 1, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(abilityName);

        EditorGUILayout.PropertyField(isWeapon);

        if (script.isWeapon)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(weaponType);

            if(script.weaponType == Ability.WeaponType.simpleWeapon)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(simpleWeapon);
                EditorGUI.indentLevel--;
            }
            else if (script.weaponType == Ability.WeaponType.complexWeapon)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(overallMagazine);
                EditorGUILayout.PropertyField(handsNeededComplexWeapon);
                EditorGUILayout.PropertyField(spawnOnlyOnMouseDown);
                EditorGUI.indentLevel--;    
            }

            EditorGUI.indentLevel--;
        }

        // actions

        EditorUi.DrawHeader("Actions", 2, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(inputHandling);
        EditorGUILayout.PropertyField(actions);
        EditorGUILayout.PropertyField(cooldown);

        // effects

        EditorUi.DrawHeader("Effects", 3, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(blasterType);
        EditorGUILayout.PropertyField(effects);

        /// for some reason if(serializedObject.hasModifiedProperties) calls perfectly, but doesn't change the customInspectorName variable
        if (true)
        {
            float time = 0f;

            script.effectPlaylist ??= new EffectPlaylist();
            script.effectPlaylist.effects ??= new List<EffectPlaylistItem>();

            for (int i = 0; i < script.effectPlaylist.effects.Count; i++)
            {
                EffectPlaylistItem playlistItem = script.effectPlaylist.effects[i];

                time += playlistItem.startDelay;

                if (playlistItem.effect != null)
                {
                    playlistItem.customInspectorName = playlistItem.effect.GetEffectType().ToString() 
                                                        + " Effect " + "(" + time + ")";
                }
            }
        }

        // interruption handling

        EditorUi.DrawHeader("Interruption Handling", 4, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(priority);
        EditorGUILayout.PropertyField(interruptionHandling);

        EditorGUILayout.PropertyField(overrideInterruptionSettings);

        if (script.overrideInterruptionSettings)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(canAlwaysInterrupt);
            EditorGUILayout.PropertyField(canNeverInterrupt);

            EditorGUI.indentLevel--;
        }

        // extra settings

        EditorUi.DrawHeader("Extra Settings", 5, EditorUi.CustomColor.white);

        /// combo system

        EditorUi.DrawToggle(ref script.enableComboSystem, "Enable Combo Sytem");

        if (script.enableComboSystem)
        { 
            EditorGUILayout.PropertyField(comboContinuation);
            EditorGUILayout.PropertyField(stopComboTimer);
        }

        /// hands needed

        EditorUi.DrawToggle(ref script.overrideHandsNeeded, "Override Hands Needed");

        if (script.overrideHandsNeeded)
        {
            EditorGUILayout.PropertyField(fixedHandsNeeded);
        }

        /// ui display

        EditorUi.DrawToggle(ref script.enableUiDisplay, "Enable Ui Display");

        if (script.enableUiDisplay)
        {
            EditorGUILayout.PropertyField(abilityIcon);
            EditorGUILayout.PropertyField(abilityColor);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
