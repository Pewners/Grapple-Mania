using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note:
// currently not being used, soon enough it might be useful though
// then I'll decide what structures and enemies etc. need to be spawned

/// Spawner class, most functions here are called by GameLogic Manager
public class Spawner : MonoBehaviour
{
    [Header("EnableSpawning")]
    public bool disableEnemies;
    public bool disableStations;
    public bool disableLootboxes;

    public bool usePremadeSpawnPoints;

    public static Spawner instance;

    [Header("Settings")]
    public int baseEnemySpawnAmount;
    public float baseStationSpawnChance;

    public LayerMask whatIsGround;

    public List<GameObject> enemyPrefs;
    private List<int> enemySpawnChances;

    public Transform enemyContainer;

    [Header("Level Boarder")]
    public Transform levelBoarder1;
    public Transform levelBoarder2;

    public bool performSpawnRoomRoutine;
    public bool performTestSpawns;
    public GameObject testMarker;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (performTestSpawns)
        {
            for (int i = 0; i < 100; i++)
            {
                Instantiate(testMarker, FindSpawnPos(), Quaternion.identity);
            }
        }

        if(performSpawnRoomRoutine)
            SpawnRoomRoutine();
    }

    public void SpawnRoomRoutine()
    {
        // Find Level Boarders
        levelBoarder1 = GameObject.Find("LevelBoarder1").transform;
        levelBoarder2 = GameObject.Find("LevelBoarder2").transform;

        // CalculateSpawnChancesForRoom();

        // Step 1 - Enemies Spawn
        int enemyAmount = disableEnemies ? 0 : CalculateEnemyAmountForRoom(1);
        SpawnEnemies(enemyAmount);

        /* Step 2 - Station Spawn
        GameAssets.StructurePrefType station = disableStations ? GameAssets.StructurePrefType.None : CalculateStationSpawn();
        if(station != GameAssets.StructurePrefType.None) SpawnStructure(station);

        // Step 3 - Lootbox Spawn
        int amountOfLootboxes = disableLootboxes ? 0 : Random.Range(2, enemyAmount + 1);
        for (int i = 0; i < amountOfLootboxes; i++)
            SpawnStructure(GetRandomLootbox());
        */
    }

    private int CalculateEnemyAmountForRoom(int roomDifficulty)
    {
        return baseEnemySpawnAmount;
    }

    private void CalculateEnemySpawnChancesForRoom()
    {
        for (int i = 0; i < enemySpawnChances.Count; i++)
        {

        }
    }

    /* so far implemented: ammoStation, weaponStation
    private GameAssets.StructurePrefType CalculateStationSpawn()
    {
        bool stationSpawn = Random.Range(0f, 100f) < baseStationSpawnChance;

        if (stationSpawn)
        {
            int randomStation = Random.Range(0, 2);
            if (randomStation == 0) return GameAssets.StructurePrefType.AmmoStation;
            if (randomStation == 1) return GameAssets.StructurePrefType.WeaponStation;
        }

        print("No station will be generated this level");
        return GameAssets.StructurePrefType.None;
    }

    private GameAssets.StructurePrefType GetRandomLootbox()
    {
        int randomStation = Random.Range(0, 3);
        if (randomStation == 0) return GameAssets.StructurePrefType.LootboxNormal;
        if (randomStation == 1) return GameAssets.StructurePrefType.LootboxAmmoAddons;
        if (randomStation == 2) return GameAssets.StructurePrefType.LootboxWeaponChipsets;

        Debug.LogError("Dave please learn again about random.range");
        return GameAssets.StructurePrefType.None;
    }
    */

    public void SpawnEnemies(int spawnAmount)
    {
        print("spawning enemies...");

        // no spawn chances for now
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnPos = FindSpawnPos();

            int randomEnemy = Random.Range(0, enemyPrefs.Count);

            GameObject enemy = Instantiate(enemyPrefs[randomEnemy], spawnPos + Vector3.up * 3f, Quaternion.identity);
            enemy.transform.SetParent(enemyContainer);

            print("spawned " + enemy.name + " at " + spawnPos);
        }
    }

    /*
    public void SpawnStructure(GameAssets.StructurePrefType structurePrefType)
    {
        Vector3 spawnPos = FindSpawnPos();

        GameObject structureObj = GameAssets.instance.GetStructurePrefab(structurePrefType);
        Instantiate(structureObj, spawnPos + Vector3.up * 0.25f, Quaternion.identity);
    }
    */

    private Vector3 FindSpawnPos()
    {
        return usePremadeSpawnPoints ? FindSpawnPos_Premade() : FindSpawnPos_Raycast();
    }

    private Vector3 FindSpawnPos_Premade()
    {
        Transform spawnPoints = GameObject.Find("SpawnPoints").transform;

        int randomPoint = UnityEngine.Random.Range(0, spawnPoints.childCount);

        return spawnPoints.GetChild(randomPoint).position;
    }

    private Vector3 FindSpawnPos_Raycast()
    {
        int iterationsLeft = 10;

        while (iterationsLeft > 0)
        {
            float randomXPos = Random.Range(levelBoarder1.position.x, levelBoarder2.position.x);
            float randomZPos = Random.Range(levelBoarder1.position.z, levelBoarder2.position.z);

            float yPos = levelBoarder1.position.y + 50f;

            // raycast downward to check for ground
            Vector3 raycastPos = new Vector3(randomXPos, yPos, randomZPos);

            RaycastHit hit;
            if (Physics.Raycast(raycastPos, Vector3.down, out hit, 100f, whatIsGround))
            {
                return hit.point;
            }

            iterationsLeft--;
        }

        Debug.LogError("Couldn't find spawn pos, eighter the map isn't correctly marked or you're super unlucky haha");
        return Vector3.zero;
    }
}
