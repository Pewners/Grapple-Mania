using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dave;
using RcLab;


public class GlobalProjectileSpawner : MonoBehaviour
{
    private Combat combat;

    private void Start()
    {
        combat = PlayerReferences.instance.combat;
    }

    public void StartSpawnCycle(SpawnProjectileData data, Transform relativeTransform)
    {
        StartCoroutine(SpawnProjectileCycle(data, relativeTransform));
    }

    private IEnumerator SpawnProjectileCycle(SpawnProjectileData data, Transform relativeDirection)
    {
        int spawnsToExecute = data.spawnAmount;

        for (int i = 0; i < spawnsToExecute; i++)
        {
            SpawnProjectile(data, relativeDirection, i);
            yield return new WaitForSeconds(data.timeBetweenSpawns);
        }

        yield return null;
    }

    public void SpawnProjectile(SpawnProjectileData data, Transform relativeDirection, int indexOfRound)
    {
        Vector3 spawnPos = CalculateSpawnPoint(data, relativeDirection, indexOfRound);
        Vector3 forceDirection = CalculateDirectionWithSpread(data, relativeDirection, spawnPos);
        forceDirection = MathsExtension.RotateVectorByVerticalAngle(data.verticalAngle, forceDirection, relativeDirection);

        // a bit scuffed, should unify this sometime
        Projectile projectile = combat.InstantiateProjectile(data.projectileName, spawnPos, forceDirection);
        float initialAcceleration = combat.CalculateInitialAcceleration(data.force);
        combat.AddForceToProjectile(projectile, forceDirection, initialAcceleration);

        AddStrictUpwardsForce(projectile.GetComponent<Rigidbody>(), data.upwardForce);

        print("force added -> " + projectile.GetComponent<Rigidbody>().velocity.magnitude);
    }

    private Vector3 CalculateSpawnPoint(SpawnProjectileData data, Transform relativeDirection, int index)
    {
        Vector3 spawnPoint = Vector3.zero;

        if (data.spawnPattern == SpawnProjectileData.SpawnPattern.Simple)
            spawnPoint = relativeDirection.localPosition + data.spawnPosRelative;
        else
            spawnPoint = MathsExtension.CalculateSpawnPos(relativeDirection, relativeDirection.position, data.spawnPosData, index);

        return spawnPoint;
    }

    private Vector3 CalculateDirectionWithSpread(SpawnProjectileData data, Transform relativeT, Vector3 spawnPoint)
    {
        Vector3 spreadDirectionOffset = MathsExtension.CalculateRelativeSpreadVector(relativeT, data.spread, data.minSpread);

        Vector3 baseDirection = relativeT.forward;
        Vector3 baseDirectionWithSpread = baseDirection + spreadDirectionOffset;

        return baseDirectionWithSpread;
    }

    private Vector3 AddStrictUpwardsForce(Rigidbody rb, float upwardsForce)
    {
        Vector3 forceVector = Vector3.up * upwardsForce;
        rb.AddForce(forceVector, ForceMode.Impulse);
        return forceVector;
    }
}

[Serializable]
public class SpawnProjectileData
{
    public string projectileName = "DefaultProjectile";
    public int spawnAmount;
    public float timeBetweenSpawns;

    public SpawnPattern spawnPattern;
    public enum SpawnPattern
    {
        Simple,
        Complex
    }
    public Vector3 spawnPosRelative;
    public SpawnPosData spawnPosData;

    public float force;
    public float upwardForce;
    public float verticalAngle;
    public float minSpread;
    public float spread;
}