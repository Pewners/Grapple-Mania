using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float teleportationDelay;
    public bool useGlobalCoordinates = false;
    public Vector3 offsetVector;

    [Header("Randomizing")]
    public bool useRandomOffset;
    public Vector3 minRandom;
    public Vector3 maxRandom;

    private void Start()
    {
        Invoke(nameof(Teleport), teleportationDelay);
    }

    private void Teleport()
    {
        float offsetX = useRandomOffset ? Random.Range(minRandom.x, maxRandom.x) : offsetVector.x;
        float offsetY = useRandomOffset ? Random.Range(minRandom.y, maxRandom.y) : offsetVector.y;
        float offsetZ = useRandomOffset ? Random.Range(minRandom.z, maxRandom.z) : offsetVector.z;

        print("OffsetX = " + offsetX);
        print("OffsetXx = " + Random.Range(minRandom.x, maxRandom.x));

        if (useGlobalCoordinates)
            transform.position = transform.position + new Vector3(offsetX, offsetY, offsetZ);

        else
        {
            Vector3 localOffset = transform.right * offsetX + transform.up * offsetY + transform.forward * offsetZ;

            transform.position = transform.position + localOffset;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        if (useGlobalCoordinates)
            Gizmos.DrawLine(transform.position, transform.position + offsetVector);

        else
            Gizmos.DrawLine(transform.position, transform.position + transform.right * offsetVector.x + transform.up * offsetVector.y + transform.forward * offsetVector.z);
    }
}
