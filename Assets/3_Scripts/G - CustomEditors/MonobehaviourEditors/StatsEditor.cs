using Dave;
using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stats))]
public class StatsEditor : Editor
{
    public SerializedProperty
        statsType,
        health,
        maxHealth,
        naturalRegeneration,
        shield,
        overShield,
        resistance,
        moveSpeed,
        strength,
        power,
        healthbar,
        playerEffects,
        weaponEffects,
        destructionSound,
        useRagdollExplosion,
        ragdollExplosionForce,
        explosionVisualEffects;

    private void OnEnable()
    {
        // stats
        statsType = serializedObject.FindProperty("statsType");
        healthbar = serializedObject.FindProperty("healthbar");
        playerEffects = serializedObject.FindProperty("playerEffects");
        weaponEffects = serializedObject.FindProperty("weaponEffects");
        destructionSound = serializedObject.FindProperty("destructionSound");
        useRagdollExplosion = serializedObject.FindProperty("useRagdollExplosion");
        ragdollExplosionForce = serializedObject.FindProperty("ragdollExplosionForce");
        explosionVisualEffects = serializedObject.FindProperty("explosionVisualEffects");

        // base stats
        health = serializedObject.FindProperty("baseStats").FindPropertyRelative("health");
        maxHealth = serializedObject.FindProperty("baseStats").FindPropertyRelative("maxHealth");
        naturalRegeneration = serializedObject.FindProperty("baseStats").FindPropertyRelative("naturalRegeneration");
        shield = serializedObject.FindProperty("baseStats").FindPropertyRelative("shield");
        overShield = serializedObject.FindProperty("baseStats").FindPropertyRelative("overShield");
        resistance = serializedObject.FindProperty("baseStats").FindPropertyRelative("resistance");
        moveSpeed = serializedObject.FindProperty("baseStats").FindPropertyRelative("moveSpeed");
        strength = serializedObject.FindProperty("baseStats").FindPropertyRelative("strength");
        power = serializedObject.FindProperty("baseStats").FindPropertyRelative("power");
    }

    public override void OnInspectorGUI()
    {
        Stats script = target as Stats;

        serializedObject.Update();

        string typeName = script.statsType.ToString();
        EditorUi.DrawComponentTitle(EditorUiData.Component.Stats, EditorUi.CustomColor.statEffectBlue, typeName + " Stats");

        // stats type

        ///EditorUi.DrawSubHeader("Stats Type");

        EditorGUILayout.PropertyField(statsType);

        // base stats

        EditorUi.DrawHeader("Base Stats", 1, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(health);
        EditorGUILayout.PropertyField(maxHealth);
        EditorGUILayout.PropertyField(naturalRegeneration);
        EditorGUILayout.PropertyField(shield);
        EditorGUILayout.PropertyField(overShield);
        EditorGUILayout.PropertyField(resistance);
        EditorGUILayout.PropertyField(moveSpeed);
        EditorGUILayout.PropertyField(strength);
        EditorGUILayout.PropertyField(power);

        // active effects

        EditorUi.DrawHeader("Active Effects", 2, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(playerEffects);
        EditorGUILayout.PropertyField(weaponEffects);

        // references

        EditorUi.DrawHeader("References", 3, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(healthbar);

        // enemy only

        if(script.statsType == Stats.StatsType.Enemy)
        {
            EditorUi.DrawHeader("Enemy only", 4, EditorUi.CustomColor.white);

            EditorGUILayout.PropertyField(useRagdollExplosion);

            if (script.useRagdollExplosion)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(ragdollExplosionForce);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(destructionSound);
            EditorGUILayout.PropertyField(explosionVisualEffects);
        }

        EditorUi.DrawMonoBehaviourOutroSpace();

        serializedObject.ApplyModifiedProperties();
    }
}
