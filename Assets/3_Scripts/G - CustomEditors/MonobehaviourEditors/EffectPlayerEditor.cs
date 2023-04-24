using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EffectPlayer))]
public class EffectPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.EffectPlayer);

        EditorUi.DrawLargeMonoBehaviourOutroSpace();

        serializedObject.ApplyModifiedProperties();
    }
}
