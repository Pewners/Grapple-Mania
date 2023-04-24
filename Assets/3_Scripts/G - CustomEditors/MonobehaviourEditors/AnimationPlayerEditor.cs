using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationPlayer))]
public class AnimationPlayerEditor : Editor
{
    public SerializedProperty
        weaponAnimations,
        fixedAnimations;

    private void OnEnable()
    {
        weaponAnimations = serializedObject.FindProperty("weaponAnimations");
        fixedAnimations = serializedObject.FindProperty("fixedAnimations");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.AnimationPlayer);

        // animations

        EditorUi.DrawHeader("Animations", 1, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(weaponAnimations);
        EditorGUILayout.PropertyField(fixedAnimations);

        EditorUi.DrawMonoBehaviourOutroSpace();

        serializedObject.ApplyModifiedProperties();
    }
}
