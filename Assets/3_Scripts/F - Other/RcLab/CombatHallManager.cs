using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CombatHallManager : MonoBehaviour
{
    public Transform spawnPointOutsideHall;
    public Transform spawnPointInsideHall;

    [Header("Waves")]
    public List<WaveData> waveDatas;
    public Transform waveContainerContainer;

    [Header("RandomWeapons")]
    public List<RandomWeaponSlot> randomWeaponSlots;

    private Transform player;

    private GameObject currWaveContainer;

    private int currWaveIndex;
    private int currEnemyDefeatedCount;

    private void Start()
    {
        player = PlayerReferences.instance.tr;
    }

    public void StartRandomWave()
    {
        currEnemyDefeatedCount = 0;

        int randomWave = UnityEngine.Random.Range(0, waveDatas.Count);

        currWaveContainer = Instantiate(waveDatas[randomWave].waveContainer, waveContainerContainer);

        // activate "Content" gameObject of wave
        currWaveContainer.transform.GetChild(0).gameObject.SetActive(true);

        player.position = spawnPointInsideHall.position;
    }

    public void OnEnemyDefeated()
    {
        currEnemyDefeatedCount++;

        if (currEnemyDefeatedCount >= waveDatas[currWaveIndex].enemyCount)
            WaveFinished();
    }

    private void WaveFinished()
    {
        //player.position = spawnPointOutsideHall.position;

        Destroy(currWaveContainer);

        // refresh random weapons
        for (int i = 0; i < randomWeaponSlots.Count; i++)
        {
            randomWeaponSlots[i].Setup();
        }
    }
}

[Serializable]
public class WaveData
{
    public string waveName;
    public GameObject waveContainer;
    public int enemyCount;
}
