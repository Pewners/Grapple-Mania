using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    public SerializedProperty
        size,
        mass,
        rb_useGravity,
        damage,
        destroyOnHit,
        destroyOnEnemyHit,
        hitSoundEffect,
        stayAtAttackPoint,
        explosive,
        keepTrailAfterImpact;

    /// impactEffect
    public SerializedProperty
        impactEffectType,
        impactEffectSize;
    public List<string> _impactEffectVariations;
    int _selected = 0;
    GameAssets.ImpactEffect _lastImpactEffectType;

    private void OnEnable()
    {
        size = serializedObject.FindProperty("size");
        mass = serializedObject.FindProperty("mass");
        rb_useGravity = serializedObject.FindProperty("rb_useGravity");
        damage = serializedObject.FindProperty("damage");
        destroyOnHit = serializedObject.FindProperty("destroyOnHit");
        destroyOnEnemyHit = serializedObject.FindProperty("destroyOnEnemyHit");
        hitSoundEffect = serializedObject.FindProperty("hitSoundEffect");
        stayAtAttackPoint = serializedObject.FindProperty("stayAtAttackPoint");
        explosive = serializedObject.FindProperty("explosive");
        keepTrailAfterImpact = serializedObject.FindProperty("keepTrailAfterImpact");

        /// impact effect
        impactEffectType = serializedObject.FindProperty("impactEffect").FindPropertyRelative("impactEffectType");
        impactEffectSize = serializedObject.FindProperty("impactEffect").FindPropertyRelative("size");

        _lastImpactEffectType = GameAssets.ImpactEffect.None;
        _impactEffectVariations = new List<string>();
    }

    public override void OnInspectorGUI()
    {
        Projectile script = target as Projectile;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.Projectile, EditorUi.CustomColor.lightBlue);

        /// mass?
        /// size?

        // stats

        EditorUi.DrawHeader("Stats", 1, EditorUi.CustomColor.lightBlueLs);

        EditorGUILayout.PropertyField(damage);

        // behaviour

        EditorUi.DrawHeader("Behaviour", 2, EditorUi.CustomColor.lightBlueLs);

        EditorGUILayout.PropertyField(rb_useGravity);
        EditorGUILayout.PropertyField(destroyOnHit);
        EditorGUILayout.PropertyField(destroyOnEnemyHit);
        EditorGUILayout.PropertyField(stayAtAttackPoint);

        // sound and vfx

        EditorUi.DrawHeader("Sound and Vfx", 3, EditorUi.CustomColor.lightBlueLs);

        /// visuals

        EditorUi.DrawToggle(ref script.enableVisualEffects, "Enable Visual Effects");

        if (script.enableVisualEffects)
        {
            EditorGUILayout.PropertyField(impactEffectType);

            /// load variations if needed
            if (_lastImpactEffectType != script.impactEffect.impactEffectType)
            {
                _impactEffectVariations = EditorUi.LoadEffectVariationSelection(script.impactEffect.impactEffectType);
                if (_impactEffectVariations != null)
                    script.impactEffect.effectVariation = _impactEffectVariations[0];
            }

            /// dropdown menu
            if(_impactEffectVariations != null)
            {
                EditorGUI.BeginChangeCheck();

                _selected = EditorGUILayout.Popup("Effect Variations", _selected, _impactEffectVariations.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    Debug.Log("Dropdown selection: " + _impactEffectVariations[_selected]);
                    script.impactEffect.effectVariation = _impactEffectVariations[_selected];
                }

                EditorGUILayout.PropertyField(impactEffectSize);
            }

            EditorGUILayout.PropertyField(keepTrailAfterImpact);
        }

        /// sound

        EditorUi.DrawToggle(ref script.enableSound, "Enable Sound Effects");

        if (script.enableSound)
        {
            EditorGUILayout.PropertyField(hitSoundEffect);
        }

        _lastImpactEffectType = script.impactEffect.impactEffectType;

        serializedObject.ApplyModifiedProperties();

        SerializedProperty property = serializedObject.GetIterator();

        int amount = 0;
        while (property.NextVisible(true))
        {
            Debug.Log("property: " + property.name + ": " + property.type);
            amount++;
        }
        Debug.Log("total properties: " + amount);
    }
}
