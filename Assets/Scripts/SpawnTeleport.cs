using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTeleport : MonoBehaviour
{
    public GameObject player;
    public GameObject cam;
    public Transform spawnLocation;

    void OnTriggerEnter(Collider other)
    {
        player.transform.position = spawnLocation.transform.position;
    }
}
