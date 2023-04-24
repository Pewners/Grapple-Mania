using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectRespawner : MonoBehaviour
{
    public List<RespawneableObject> respawneableObjects;

    public enum ObjectType
    {
        Dummy
    }

    public void RespawnObject(ObjectType objectType, Vector3 position, Quaternion rotation, float delay)
    {
        StartCoroutine(RespawnObjectDelayed(objectType, position, rotation, delay));
    }

    private IEnumerator RespawnObjectDelayed(ObjectType objectType, Vector3 position, Quaternion rotation, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject objectToSpawn = GetRespawneableObject(objectType);

        Instantiate(objectToSpawn, position, rotation);
    }

    #region Getters

    public GameObject GetRespawneableObject(ObjectType objectType)
    {
        for (int i = 0; i < respawneableObjects.Count; i++)
        {
            if (respawneableObjects[i].objectType == objectType)
                return respawneableObjects[i].objectToSpawn;
        }

        Debug.LogError("Respawneable object type not implemented");
        return null;
    }

    #endregion
}

[Serializable]
public class RespawneableObject
{
    public ObjectRespawner.ObjectType objectType;

    public GameObject objectToSpawn;
}