using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionPlayer))]
public class ActionPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.ActionPlayer);

        EditorUi.DrawLargeMonoBehaviourOutroSpace();

        serializedObject.ApplyModifiedProperties();
    }
}
