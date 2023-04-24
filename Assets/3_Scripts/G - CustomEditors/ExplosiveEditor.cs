using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Dave;

[CustomEditor(typeof(Explosive))]
public class ExplosiveEditor : Editor
{
    public SerializedProperty
        explosionRadius,
        explosionForce,
        explosionDamage,
        autoExplosionDelay,
        whatIsPushable,
        mineDetectionRange,
        whatIsMineTrigger,
        explosionSoundEffect;

    /// explosionEffect
    public SerializedProperty
        impactEffectType,
        impactEffectSize;
    public List<string> _impactEffectVariations;
    GameAssets.ImpactEffect _lastImpactEffectType;


    private void OnEnable()
    {
        explosionRadius = serializedObject.FindProperty("explosionRadius");
        explosionForce = serializedObject.FindProperty("explosionForce");
        explosionDamage = serializedObject.FindProperty("explosionDamage");
        autoExplosionDelay = serializedObject.FindProperty("autoExplosionDelay");
        whatIsPushable = serializedObject.FindProperty("whatIsPushable");
        mineDetectionRange = serializedObject.FindProperty("mineDetectionRange");
        whatIsMineTrigger = serializedObject.FindProperty("whatIsMineTrigger");
        explosionSoundEffect = serializedObject.FindProperty("explosionSoundEffect");

        // explosionEffect
        impactEffectType = serializedObject.FindProperty("explosionVisualEffect").FindPropertyRelative("impactEffectType");
        impactEffectSize = serializedObject.FindProperty("explosionVisualEffect").FindPropertyRelative("size");
    }

    public override void OnInspectorGUI()
    {
        Explosive script = target as Explosive;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Explosive);

        // settings

        EditorUi.DrawHeader("Settings", 1, EditorUi.CustomColor.orangeLs);

        EditorGUILayout.PropertyField(explosionRadius);
        EditorGUILayout.PropertyField(explosionForce);
        EditorGUILayout.PropertyField(explosionDamage);
        EditorGUILayout.PropertyField(autoExplosionDelay);
        EditorGUILayout.PropertyField(whatIsPushable);

        /// explosion effect

        EditorUi.DrawToggle(ref script.enableVisualEffects, "Enable Visual Effects");

        if (script.enableVisualEffects)
        {
            EditorGUILayout.PropertyField(impactEffectType);

            /// load variations if needed
            if (_lastImpactEffectType != script.explosionVisualEffect.impactEffectType)
            {
                _impactEffectVariations = EditorUi.LoadEffectVariationSelection(script.explosionVisualEffect.impactEffectType);
                if (_impactEffectVariations != null)
                    script.explosionVisualEffect.effectVariation = _impactEffectVariations[0];
            }

            /// dropdown menu
            if (_impactEffectVariations != null)
            {
                EditorGUI.BeginChangeCheck();

                script.selected_VisualEffect = EditorGUILayout.Popup("Effect Variations", script.selected_VisualEffect, _impactEffectVariations.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    Debug.Log("Dropdown selection: " + _impactEffectVariations[script.selected_VisualEffect]);
                    script.explosionVisualEffect.effectVariation = _impactEffectVariations[script.selected_VisualEffect];
                }

                EditorGUILayout.PropertyField(impactEffectSize);
            }
        }

        /// sound effect

        EditorUi.DrawToggle(ref script.enableSound, "Enable Sound Effects");

        if (script.enableSound)
        {
            EditorGUILayout.PropertyField(explosionSoundEffect);
        }

        // extensions

        EditorUi.DrawHeader("Extensions", 2, EditorUi.CustomColor.orangeLs);

        /// mine

        EditorUi.DrawToggle(ref script.enableMine, "Enable Mine Extension");

        if (script.enableMine)
        {
            EditorGUILayout.PropertyField(mineDetectionRange);
            EditorGUILayout.PropertyField(whatIsMineTrigger);
        }

        _lastImpactEffectType = script.explosionVisualEffect.impactEffectType;

        serializedObject.ApplyModifiedProperties();
    }
}
