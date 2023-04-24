using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dave;

[RequireComponent(typeof(PhysicMaterialCreator))]
public class AssistedBouncing : ProjectileAddon
{
    // settings
    public float detectionRange;
    public float spread;
    public float maxCorrectionAngle = -2;
    public LayerMask whatIsEnemy;

    private Projectile projectile;

    private void Start()
    {
        projectile = GetComponent<Projectile>();
        projectile.OnCollision += OnCollision;
    }

    private void OnCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, whatIsEnemy);

        float minDistanceRecord = Mathf.Infinity;
        int closestColliderIndex = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            float distanceToCollider = Vector3.Distance(transform.position, colliders[i].transform.position);

            if(distanceToCollider < minDistanceRecord)
            {
                closestColliderIndex = i;
                minDistanceRecord = distanceToCollider;
            }
        }

        Vector3 dirToClosestTarget = colliders[closestColliderIndex].transform.position - transform.position;

        transform.LookAt(transform.position + dirToClosestTarget);
        dirToClosestTarget = MathsExtension.AddSpreadToVector(transform, 0f, spread);

        projectile.RedirectVelocity(dirToClosestTarget);
    }

    private void OnDestroy()
    {
        projectile.OnCollision -= OnCollision;
    }
}
