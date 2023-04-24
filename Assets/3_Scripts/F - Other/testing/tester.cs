using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour
{
    public float roationAngle;

    public SpawnProjectileData projectileSpawn;

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Quaternion rotation = Quaternion.AngleAxis(roationAngle, -transform.right);

        Vector3 rotatedForward = rotation * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, rotatedForward);
    }
}
