using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dave;

/// addon to give projectile a homing ability

public class Homing : ProjectileAddon
{
    // find target
    public TargetMode targetMode;
    public enum TargetMode
    {
        FindWithRaycast, // try raycast, if no hits, either fly to hit point or fall back to fixed distance (enableFixedDistanceAsFallback)
        FixedDistance // only use fixed distance
    }

    // preferences
    public PreferTarget preferTarget;
    public enum PreferTarget
    {
        ClosestToSpawnPoint,
        ClosestToViewPoint
    }
    
    public float raycastLength = 30;
    public float sphereCastRadius = 5f;
    public LayerMask whatIsTarget = 1 << 13; // for now just 13, later also 9
    public bool enableFixedDistanceAsFallback;

    public float targetFixedForwardDistance = 0;
    public float targetFixedRightDistance = 0;

    // references
    private Rigidbody rb;
    private Vector3 targetDestination; // a fixed destination to target (only used when targetT == null)
    private Vector3 targetPosToDestinationOffset; // otherwise they always fly to the same point, no matter if aimed at head or feet
    private Transform targetT;
    private Rigidbody targetRb;
    private Transform playerCam;

    // activation
    public float activateHomingAfterTime = 0f;
    private bool homingActive = false;

    // movement
    public bool useInitialVelocityAsSpeed;
    [Tooltip("Only used when useInitialvelocityAsSpeed is not active.")]
    public float fixedSpeed = 12f;
    private float speed;

    public RotationMode rotationMode = RotationMode.scaleWithDistance;
    public enum RotationMode
    {
        constantSpeed,
        constantIncrease,
        scaleWithDistance
    }
    public float rotateSpeed = 5f;
    public float rotateSpeedIncrease = 50f;
    public float farDistanceRotateSpeed = 5f;
    public float closeDistanceRotateSpeed = 120f;
    private float startDistance;

    // prediction
    public bool enablePrediction;
    public float maxDistancePredict = 0f;
    public float minDistancePredict = 0f;
    public float maxTimePrediction = 0f;

    public Vector3 standardPrediction;
    public Vector3 deviatedPrediction;

    // deviation
    public bool enableDeviation;
    public float deviationSpeed = 0f;
    public float deviationAmount = 0f;
    private float deviationOffset;

    // debugging
    public bool enableDebugging;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main.transform;

        rb.rotation = transform.rotation;

        deviationOffset = Random.Range(0f, 1f);

        if (targetMode == TargetMode.FindWithRaycast)
            Invoke(nameof(FindTarget), 0.05f);

        else if (targetMode == TargetMode.FixedDistance)
            TargetFixedDistance();

        Invoke(nameof(ActivateHoming), activateHomingAfterTime);
    }

    private void FindTarget()
    {
        print("Player cam = " + playerCam);

        // where the player clicked in the world -> cast spherecast in that direction to find enemies in range

        Projectile projectile = GetComponent<Projectile>();
        Vector3 spawnPoint = projectile.GetSpawnPoint();
        Vector3 viewPointTarget = projectile.originScript.lastAttackViewPoint;
        Vector3 dirToViewPoint = Vector3.Normalize(viewPointTarget - transform.position);

        print("raycast sent " + viewPointTarget);

        RaycastHit[] raycastHits = Physics.SphereCastAll(spawnPoint, sphereCastRadius, dirToViewPoint, raycastLength, whatIsTarget);

        if (raycastHits.Length > 0)
        {
            RaycastHit preferredHit = default;

            if (preferTarget == PreferTarget.ClosestToViewPoint)
                preferredHit = GetHitClosestToPoint(raycastHits, viewPointTarget);

            else if (preferTarget == PreferTarget.ClosestToSpawnPoint)
                preferredHit = GetHitClosestToPoint(raycastHits, spawnPoint);

            print("raycast hit enemy");

            // just fly to hit point
            targetDestination = preferredHit.point;

            // fly towards enemy
            if (preferredHit.transform.GetComponent<Rigidbody>() != null)
            {
                targetT = preferredHit.transform;
                targetRb = preferredHit.transform.GetComponent<Rigidbody>();

                targetPosToDestinationOffset = targetDestination - targetT.position;
            }

            // Testing
            if(targetT != null)
            {
                if(enableDebugging) /// debug
                    FindObjectOfType<DebugExtensionManager>().PlaceMarker(targetDestination + Vector3.up, 1f, "green");

                print("Homing locked onto " + targetT.gameObject.name);
            }
        }

        else
        {
            if (enableFixedDistanceAsFallback)
            {
                TargetFixedDistance();
            }
            else
                targetDestination = viewPointTarget;

            ///FindObjectOfType<DebugExtensionManager>().PlaceMarker(targetDestination, 0.25f, "blue");
        }

        startDistance = Vector3.Distance(transform.position, targetDestination);
    }

    private RaycastHit GetHitClosestToPoint(RaycastHit[] raycastHits, Vector3 point)
    {
        RaycastHit preferredHit = default;
        float minDistanceRecord = 10000f;

        // find closest hit to viewPoint
        for (int i = 0; i < raycastHits.Length; i++)
        {
            float distanceToViewPoint = Vector3.Distance(point, raycastHits[i].point);

            if (distanceToViewPoint < minDistanceRecord)
            {
                preferredHit = raycastHits[i];
                minDistanceRecord = distanceToViewPoint;
            }

            if(enableDebugging) /// debug
                FindObjectOfType<DebugExtensionManager>().PlaceMarker(raycastHits[i].point + Vector3.up, 0.75f, "blue");
        }

        return preferredHit;
    }

    private void TargetFixedDistance()
    {
        Vector3 fixedOffset = playerCam.forward * targetFixedForwardDistance + playerCam.right * targetFixedRightDistance;
        targetDestination = transform.position + fixedOffset;
    }

    private void ActivateHoming()
    {
        homingActive = true;

        if (useInitialVelocityAsSpeed)
            speed = GetComponent<Projectile>().GetInitialVelocity().magnitude;

        else
            speed = fixedSpeed;
    }

    private void Update()
    {
        if (!homingActive)
            return;

        if (rotationMode == RotationMode.constantIncrease)
            rotateSpeed += rotateSpeedIncrease * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!homingActive) 
            return;

        // base thrust
        rb.velocity = transform.forward * speed;

        float leadTimePercentage;

        float distanceToTarget = 0f;
        if (targetT != null)
            distanceToTarget = Vector3.Distance(transform.position, targetT.position + targetPosToDestinationOffset);
        else
            distanceToTarget = Vector3.Distance(transform.position, targetDestination);

        leadTimePercentage = Mathf.InverseLerp(minDistancePredict, maxDistancePredict, distanceToTarget);

        if (rotationMode == RotationMode.scaleWithDistance)
        {
            float t = distanceToTarget / startDistance;
            rotateSpeed = t > 1 ? farDistanceRotateSpeed : Mathf.Lerp(closeDistanceRotateSpeed, farDistanceRotateSpeed, t);
            print("Homing rotate speed set: " + distanceToTarget + " " + startDistance + " " + t + " " + rotateSpeed);
        }

        //deviatedPrediction = targetDestination
        PredictMovement(leadTimePercentage);
        AddDeviation(leadTimePercentage);
        RotateRocket();
    }

    private void PredictMovement(float leadTimePercentage)
    {
        float predictionTime = Mathf.Lerp(0, maxTimePrediction, leadTimePercentage);

        // just fly to the destination
        if (targetT == null)
            standardPrediction = targetDestination;

        // just fly to the target
        else if (targetT != null && targetRb == null)
            standardPrediction = targetT.position + targetPosToDestinationOffset;

        // predict the targets movement using its rigidbody velocity
        else if(targetT != null && targetRb != null)
            standardPrediction = targetT.position + targetPosToDestinationOffset + targetRb.velocity * predictionTime;
    }

    private void AddDeviation(float leadTimePercentage)
    {
        // honestly don't really get this but who cares
        ///print("Cosine: " + Time.time * deviationSpeed);
        ///print("Cosine offset: " + (Time.time * deviationSpeed + deviationOffset * 50f));
        Vector3 deviation = new Vector3(Mathf.Cos(Time.time * deviationSpeed + deviationOffset * 100f), 0, 0);

        Vector3 predictionOffset = transform.TransformDirection(deviation) * deviationAmount * leadTimePercentage;

        deviatedPrediction = standardPrediction + predictionOffset;
    }

    private void RotateRocket()
    {
        Vector3 heading = deviatedPrediction - transform.position;

        // heading = targetDestination - transform.position;

        //transform.forward = Vector3.Lerp(transform.forward, heading, rotateSpeed * Time.deltaTime);
        //return;

        Quaternion rotation = Quaternion.LookRotation(heading);

        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime));
        //transform.Rotate(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime).eulerAngles);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(standardPrediction, deviatedPrediction);
        Vector3 fixedOffset = transform.forward * targetFixedForwardDistance + transform.right * targetFixedRightDistance;
        Gizmos.DrawLine(transform.position, transform.position + fixedOffset);
    }
}