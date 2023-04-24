using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(PlayerAbilities))]
public class PlayerAbilitiesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.PlayerAbilities);

        // settings

        // EditorUi.DrawHeader("Handles all Ability Cycles", 1, EditorUi.CustomColor.white);

        EditorUi.DrawLargeMonoBehaviourOutroSpace();

        serializedObject.ApplyModifiedProperties();
    }
}
