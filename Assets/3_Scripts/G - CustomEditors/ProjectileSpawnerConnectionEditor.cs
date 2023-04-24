using Dave;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProjectileSpawnerConnection))]
public class ProjectileSpawnerConnectionEditor : Editor
{
    public SerializedProperty
        totalRounds,
        roundsSpawnedPerEvent,
        spawnEvent,
        timeBetweenRounds;

    public SerializedProperty
        projectileName,
        timeBetweenSpawns,
        spawnPattern,
        force,
        upwardsForce,
        verticalAngle,
        spread,
        minSpread,
        spawnAmount,
        spawnPosRelative,
        spawnPosData,
        relativeDirection;


    private void OnEnable()
    {
        totalRounds = serializedObject.FindProperty("totalRounds");
        spawnEvent = serializedObject.FindProperty("spawnEvents");
        timeBetweenRounds = serializedObject.FindProperty("timeBetweenRounds");
        roundsSpawnedPerEvent = serializedObject.FindProperty("roundsSpawnedPerEvent");

        // SpawnProjectileData
        projectileName = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("projectileName");
        timeBetweenSpawns = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("timeBetweenSpawns");
        spawnPattern = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("spawnPattern");
        force = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("force");
        upwardsForce = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("upwardForce");
        verticalAngle = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("verticalAngle");
        spread = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("spread");
        minSpread = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("minSpread");
        spawnAmount = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("spawnAmount");
        spawnPosRelative = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("spawnPosRelative");
        spawnPosData = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("spawnPosData");
        relativeDirection = serializedObject.FindProperty("spawnProjectileData").FindPropertyRelative("relativeDirection");
    }

    public override void OnInspectorGUI()
    {
        ProjectileSpawnerConnection script = target as ProjectileSpawnerConnection;

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.ProjectileSpawnerConnection, EditorUi.CustomColor.lightPurple);

        // projectile

        EditorUi.DrawHeader("Projectile", 1, EditorUi.CustomColor.white);

        /// spawn
        
        EditorUi.DrawSubHeader("Spawn");

        EditorGUILayout.PropertyField(projectileName);
        EditorGUILayout.PropertyField(spawnAmount);
        EditorGUILayout.PropertyField(timeBetweenSpawns);
        EditorGUILayout.PropertyField(spawnPattern);

        if(script.spawnProjectileData.spawnPattern == SpawnProjectileData.SpawnPattern.Simple)
        {
            EditorGUILayout.PropertyField(spawnPosRelative);
        }
        else if (script.spawnProjectileData.spawnPattern == SpawnProjectileData.SpawnPattern.Complex)
        {
            EditorGUILayout.PropertyField(spawnPosData);
        }

        /// force

        EditorUi.DrawSubHeader("Force");

        EditorGUILayout.PropertyField(force);
        EditorGUILayout.PropertyField(upwardsForce);
        EditorGUILayout.PropertyField(verticalAngle);

        /// spread

        EditorUi.DrawSubHeader("Spread");

        EditorGUILayout.PropertyField(minSpread);
        EditorGUILayout.PropertyField(spread);

        // spawn conditions

        EditorUi.DrawHeader("Spawn Conditions", 2, EditorUi.CustomColor.white);

        EditorGUILayout.PropertyField(totalRounds);
        EditorGUILayout.PropertyField(roundsSpawnedPerEvent);
        EditorGUILayout.PropertyField(timeBetweenRounds);
        EditorGUILayout.PropertyField(spawnEvent);

        serializedObject.ApplyModifiedProperties();
    }
}
