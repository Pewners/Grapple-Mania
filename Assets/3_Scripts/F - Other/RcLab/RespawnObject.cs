using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObject : MonoBehaviour
{
    public ObjectRespawner.ObjectType type;
    public float respawnDelay = 1f;

    private Vector3 spawnPos;
    private Quaternion spawnRotation;

    private void Start()
    {
        spawnPos = transform.position;
        spawnRotation = transform.rotation;

        GetComponent<Stats>().OnDestruction += OnDestruction;
    }

    private void OnDestruction()
    {
        FindObjectOfType<ObjectRespawner>().RespawnObject(type, spawnPos, spawnRotation, respawnDelay);
    }
}
