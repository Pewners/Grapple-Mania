using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dave;
using UnityEngine.ProBuilder;

public class Laser : ProjectileAddon
{
    public bool laserActive = true;

    // settings
    public float range = 40f;
    public float sphereCastRadius = 0.25f;
    public float damagePerTick = 5f;
    public float damageInterval = 0.1f;
    public float maxDuration = 999f;

    public LayerMask whatIsEnemy;
    public LayerMask whatIsWall;

    // extras
    public bool penetration = false;
    [Tooltip("If true, overrides the stayAtAttackPoint bool of the Projectile script.")]
    public bool stayAtAttackPoint_override = true;

    // event - on expired
    public OnExpiredAction onExpiredAction;
    public enum OnExpiredAction
    {
        DoNothing,
        Destroy,
        ShootForward,
        ShootForwardStopLaser
    }

    // event on fire stop
    public OnFireStopAction onFireStopAction;
    public enum OnFireStopAction
    {
        DoNothing,
        Destroy,
        ShootForward,
        ShootForwardStopLaser
    }

    // visuals
    public AnimationCurve lineRendererWidthCurve;
    public Material lineRendererMat;

    // events
    public event Action<Vector3> OnLaserHit;

    private bool damageTick;
    private LineRenderer lr;
    private Transform cam;
    private Projectile projectile;

    private TargetMode targetMode;
    public enum TargetMode
    {
        CamForward,
        FixedTarget
    }
    private Transform fixedTarget;

    private void Start()
    {
        // references
        cam = PlayerReferences.instance.cam.cam.transform;

        projectile = GetComponent<Projectile>();

        SubscribeToEvents();

        // overrides
        if (stayAtAttackPoint_override)
            projectile.SetStayAtAttackPoint(true);

        if(laserActive)
            StartLaser();
    }

    // this doesn't make much sense honestly, no clue how this is supposed to work with lasers
    // shot by enemies or non primary abilities
    private void SubscribeToEvents()
    {
        PlayerReferences.instance.input.OnPrimaryFireUp += OnFireStop;
        PlayerReferences.instance.inventory.OnWeaponFireStop += OnFireStop;
        PlayerReferences.instance.inventory.OnWeaponSwitch += OnFireStop;
    }

    private void OnDestroy() { UnsubscribeToEvents(); }
    private void UnsubscribeToEvents()
    {
        PlayerReferences.instance.input.OnPrimaryFireUp -= OnFireStop;
        PlayerReferences.instance.inventory.OnWeaponFireStop -= OnFireStop;
        PlayerReferences.instance.inventory.OnWeaponSwitch -= OnFireStop;
    }

    public void StartLaser(Transform overrideTarget = null)
    {
        // select target mode
        if (overrideTarget != null)
        {
            targetMode = TargetMode.FixedTarget;
            fixedTarget = overrideTarget;
        }
        else
            targetMode = TargetMode.CamForward;

        // create linerenderer
        lr = gameObject.AddComponent<LineRenderer>();
        lr.material = lineRendererMat;
        lr.positionCount = 2;
        lr.widthCurve = lineRendererWidthCurve;

        Invoke(nameof(NextDamageTick), damageInterval);
        Invoke(nameof(LaserExpired), maxDuration);

        laserActive = true;
    }

    public void StopLaser()
    {
        if(lr != null) Destroy(lr);
        laserActive = false;
    }

    private void Update()
    {
        if (!laserActive) return;

        // Step 1 - Set lineRenderer positions
        Vector3 hitPoint = new Vector3();

        // sphereCast variables
        Vector3 dirToHitPoint = new Vector3();
        Vector3 startPoint = new Vector3();
        float distance = 0f;

        if (targetMode == TargetMode.CamForward)
        {
            hitPoint = RaycastWall().point;
            startPoint = cam.position;
            dirToHitPoint = hitPoint - startPoint;
            distance = range;
        }

        else if (targetMode == TargetMode.FixedTarget)
        {
            hitPoint = fixedTarget.position;
            startPoint = transform.position;
            dirToHitPoint = hitPoint - startPoint;
            distance = dirToHitPoint.magnitude;
        }

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, hitPoint);

        // Step 2 - Damage enemies
        if (damageTick)
        {
            print("damage tick 1");

            RaycastHit[] sphereCastHits = SpherecastEnemies(startPoint, dirToHitPoint, distance);
            for (int i = 0; i < sphereCastHits.Length; i++)
            {
                if (sphereCastHits[i].point == Vector3.zero) { /* Do Nothing */ }
                else if (sphereCastHits[i].transform.TryGetComponent(out IDamageable damageable))
                {
                    projectile.DealDamage(damageable, damagePerTick, true);
                }
            }

            if (OnLaserHit != null) OnLaserHit(hitPoint);

            damageTick = false;
            Invoke(nameof(NextDamageTick), damageInterval);
        }
    }

    private void NextDamageTick()
    {
        damageTick = true;
    }

    private void LaserExpired()
    {
        if (onExpiredAction == OnExpiredAction.Destroy)
            projectile.InvokeSilentDestruction(0.05f);

        if (onExpiredAction == OnExpiredAction.ShootForward)
            projectile.ApplyInitialVelocity(true);

        if(onExpiredAction == OnExpiredAction.ShootForwardStopLaser)
        {
            projectile.ApplyInitialVelocity(true);
            StopLaser();
        }
    }

    private void OnFireStop()
    {
        if(onFireStopAction == OnFireStopAction.Destroy)
            projectile.InvokeSilentDestruction(0.05f);

        if (onFireStopAction == OnFireStopAction.ShootForward)
            projectile.ApplyInitialVelocity(true);

        if (onFireStopAction == OnFireStopAction.ShootForwardStopLaser)
        {
            projectile.ApplyInitialVelocity(true);
            StopLaser();
        }
    }

    private RaycastHit RaycastWall()
    {
        RaycastHit hit;

        // search for whatIsGround object
        if (penetration)
            Physics.Raycast(cam.position, cam.forward, out hit, range, whatIsWall);

        // search for any object
        else
            Physics.Raycast(cam.position, cam.forward, out hit, range);

        // Case: no wall found -> maxRange point in air
        if (hit.point == Vector3.zero)
            hit.point = cam.position + cam.forward * range;

        return hit;
    }

    private RaycastHit[] SpherecastEnemies(Vector3 position, Vector3 direction, float distance)
    {
        RaycastHit[] hits = new RaycastHit[] { new RaycastHit() };

        LayerMask whatIsEnemyOrWall = MathsExtension.CombineLayerMask(whatIsEnemy, whatIsWall);

        // raycastAll whatIsEnemy objects (can't penetrate walls though)
        if (penetration)
            hits = Physics.SphereCastAll(position, sphereCastRadius, direction, distance, whatIsEnemyOrWall);

        // raycast the first object
        else
        {
            RaycastHit singleHit;
            Physics.Raycast(position, direction, out singleHit, distance);
            hits[0] = singleHit;
        }

        return hits;
    }
}
