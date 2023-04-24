using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProjectileSpawnerConnection;

/// this script only serves as connection to the globalProjectileSpawner

public class ProjectileSpawnerConnection : MonoBehaviour
{
    public SpawnProjectileData spawnProjectileData = new SpawnProjectileData();

    public int totalRounds = -1;
    public int roundsSpawnedPerEvent = 1;

    public List<SpawnEvent> spawnEvents = new List<SpawnEvent>() { SpawnEvent.OnDestruction };
    public enum SpawnEvent
    {
        OnDestruction,
        FixedInterval,
        OnLaserHit
    }

    public float timeBetweenRounds;

    private int spawnsLeft;
    private float timeToNextSpawn;

    private bool continuousSpawningActive;

    private GlobalProjectileSpawner gps;

    private void Start()
    {
        if (FindObjectOfType<GlobalProjectileSpawner>() != null)
            gps = FindObjectOfType<GlobalProjectileSpawner>();
        else
            Debug.LogError("No Gps found!");

        for (int i = 0; i < spawnEvents.Count; i++)
        {
            SpawnEvent spawnEvent = spawnEvents[i];

            if (spawnEvent == SpawnEvent.FixedInterval)
                StartContinuousSpawning();

            else if (spawnEvent == SpawnEvent.OnDestruction)
                GetComponent<Projectile>().OnDestruction += SpawnAllProjectiles;

            else if (spawnEvent == SpawnEvent.OnLaserHit)
                if (GetComponent<Laser>() != null)
                    GetComponent<Laser>().OnLaserHit += SpawnAllProjectilesAtPosition;
        }
    }

    public void StartContinuousSpawning()
    {
        timeToNextSpawn = timeBetweenRounds;
        spawnsLeft = totalRounds;

        if (totalRounds == -1) spawnsLeft = 999999;

        continuousSpawningActive = true;
    }

    private void Update()
    {
        /// Spawn Event - Continous Spawning
        if (!continuousSpawningActive) return;

        if (spawnsLeft <= 0) return;

        timeToNextSpawn -= Time.deltaTime;

        if(timeToNextSpawn <= 0)
        {
            timeToNextSpawn = timeBetweenRounds;

            spawnsLeft--;

            SpawnProjectileViaGlobalSpawner();
        }
        /// End
    }

    private void SpawnAllProjectiles()
    {
        print("spawnProjectiles");

        for (int i = 0; i < totalRounds; i++)
            SpawnProjectileViaGlobalSpawner();
    }

    private void SpawnAllProjectilesAtPosition(Vector3 spawnPosition)
    {
        for (int i = 0; i < roundsSpawnedPerEvent; i++)
            SpawnProjectileViaGlobalSpawner(spawnPosition);
    }

    private void SpawnProjectileViaGlobalSpawner(Vector3 spawnPosition = default)
    {
        print("received position = " + spawnPosition);

        if(spawnPosition != default)
        {
            SpawnProjectileData dataToUse = spawnProjectileData;
            Vector3 relativePosition = spawnPosition - transform.position;
            dataToUse.spawnPattern = SpawnProjectileData.SpawnPattern.Simple;
            dataToUse.spawnPosRelative = relativePosition;
            gps.StartSpawnCycle(dataToUse, transform);
        }
        else
            gps.StartSpawnCycle(spawnProjectileData, transform);

        if (totalRounds != -1)
            totalRounds--;
    }

    private void OnDestroy()
    {
        GetComponent<Projectile>().OnDestruction -= SpawnAllProjectiles;
    }
}
