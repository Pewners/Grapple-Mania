using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RcLab;
using Dave;

/// main projectile script, handles basic effects, setting up addons and destroying the projectile

public class Projectile : ProjectileAddon
{
    // lock parts of projectile
    public bool functionalityLocked;
    public bool movementLocked;

    // stats
    [Range(1f,100f)]public float size = 1;
    [Range(0.1f, 10f)] public float mass = 1; // in kg

    // rigidbody settings
    public bool rb_useGravity = false;

    // stats
    public int damage = 20;
    public bool destroyOnHit = true;
    public bool destroyOnEnemyHit = true;

    // visual effects
    public bool enableVisualEffects;
    public ImpactEffectModifiable impactEffect;
    public bool keepTrailAfterImpact;

    // sound
    public bool enableSound;
    public AudioClipData hitSoundEffect;

    // player stat effects
    private List<ProjectileEffectApplier> effectAppliers;

    // extra settings
    public bool stayAtAttackPoint;

    // projectile addons
    private Explosive explosive;
    private ProjectileSpawnerConnection psc;

    // stored info
    [HideInInspector] public Combat originScript;
    private float weaponDamageMultiplier;
    private Vector3 spawnPoint;
    private RangedWeapon.DamageFalloffStats falloffStats;

    private Vector3 impactPoint;

    // events
    public event UnityAction OnSpawn;
    public event UnityAction OnCollision;
    public event UnityAction OnDestruction;
    public event UnityAction<Stats> OnDamage;

    private Rigidbody rb;

    private bool hitTarget;

    private Vector3 initialVelocity;

    private Transform realAttackPoint;
    private Vector3 attackPointOffset;

    private Vector3 collisionPoint;

    [HideInInspector] public float spawnTime;

    private bool isBouncy;

    public void SetInformation(Combat _originScript, List<ProjectileEffectApplier> _effectAppliers,
                               Vector3 _initialVelocity, Vector3 _spawnPoint, float _weaponDamageMultiplier,
                               RangedWeapon.DamageFalloffStats _fallOffStats)
    {
        originScript = _originScript;
        initialVelocity = _initialVelocity;
        weaponDamageMultiplier = _weaponDamageMultiplier;
        spawnPoint = _spawnPoint;
        falloffStats = _fallOffStats;

        effectAppliers = _effectAppliers;
    }

    public void SetAdditionalInformation(Transform _realAttackPoint, Vector3 _attackPointOffset)
    {
        realAttackPoint = _realAttackPoint;
        attackPointOffset = _attackPointOffset;
    }

    private void Start()
    {
        // add rigidbody if needed
        if (GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();

        rb = GetComponent<Rigidbody>();

        OnSpawn?.Invoke();

        spawnTime = Time.time;

        if (GetComponent<PhysicMaterialCreator>() != null)
            isBouncy = GetComponent<PhysicMaterialCreator>().stickToSurface == false;

        RigidbodySetup();
        AddonSetup();
    }

    private void Update()
    {
        print("Projectile velocity " + GetComponent<Rigidbody>().velocity + " " + initialVelocity);

        if (movementLocked)
            rb.velocity = Vector3.zero;
        // make sure projectile doesn't glitch around after collision
        else if (collisionPoint != Vector3.zero && !isBouncy)
            transform.position = collisionPoint;

        if (stayAtAttackPoint)
        {
            if(realAttackPoint == null)
            {
                transform.position = originScript.attackPoint.position;
                transform.rotation = originScript.attackPoint.rotation;
            }
            else
            {
                Vector3 withOffset = realAttackPoint.position;

                withOffset += realAttackPoint.right * attackPointOffset.x;
                withOffset += realAttackPoint.forward * attackPointOffset.z;
                withOffset += realAttackPoint.up * attackPointOffset.y;

                transform.position = withOffset;
                transform.rotation = realAttackPoint.rotation;
            }
        }
        else
        {
            transform.forward = rb.velocity;
        }
    }

    private void RigidbodySetup()
    {
        rb.useGravity = rb_useGravity;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void AddonSetup()
    {
        if (GetComponent<Explosive>() != null)
            explosive = GetComponent<Explosive>();

        if (GetComponent<ProjectileSpawnerConnection>() != null)
            psc = GetComponent<ProjectileSpawnerConnection>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        impactPoint = transform.position;
        CheckCollision(collision.gameObject);
        if(OnCollision != null) OnCollision();
    }

    private void OnTriggerEnter(Collider other)
    {
        impactPoint = transform.position;
        CheckCollision(other.gameObject);
        if(OnCollision != null) OnCollision();
    }

    public void OnRaycastHitDetected(Vector3 _impactPoint, GameObject targetObject)
    {
        impactPoint = _impactPoint;
        CheckCollision(targetObject);
    }

    private void CheckCollision(GameObject collisionObject)
    {
        if (functionalityLocked)
            return;

        if (hitTarget)
            return;

        collisionPoint = transform.position;

        // enemy hit
        if (collisionObject.TryGetComponent(out IDamageable damageable))
        {
            hitTarget = true;

            //BasicEnemy enemy = collision.gameObject.GetComponent<BasicEnemy>();

            //enemy.TakeDamage(damage);

            if (damageable != null)
                DealDamage(damageable, damage, true);

            if (destroyOnEnemyHit)
            {
                if (explosive != null)
                    explosive.Explode();
                else
                    InvokeDestruction(0.1f);
            }
        }

        // any hit
        if (impactEffect.impactEffectType != GameAssets.ImpactEffect.None)
            GlobalEffectsSpawner.instance.SpawnImpactEffect(impactEffect, transform.position);

        if (destroyOnHit && explosive != null)
        {
            explosive.Explode();
            return;
        }

        if (destroyOnHit)
            InvokeDestruction(0.1f);
    }

    private bool destructionInvoked = false;
    public void InvokeDestruction(float delay)
    {
        if (destructionInvoked) return;
        else destructionInvoked = true;

        // audio
        SoundManager.PlayEffect(hitSoundEffect);

        // visuals
        if(GetComponent<MeshRenderer>() != null) GetComponent<MeshRenderer>().enabled = false;
        if(GetComponent<TrailRenderer>() != null && !keepTrailAfterImpact) GetComponent<TrailRenderer>().enabled = false;

        if (OnDestruction != null) OnDestruction();

        Invoke(nameof(CheckDestroyProjectile), delay);
    }

    // no spawns, no audio
    public void InvokeSilentDestruction(float delay)
    {
        Invoke(nameof(CheckDestroyProjectile), delay);
    }

    private void CheckDestroyProjectile()
    {
        if (keepTrailAfterImpact)
        {
            functionalityLocked = true;
            movementLocked = true;

            TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer != null)
                Invoke(nameof(DestroyProjectile), trailRenderer.time);
            else
                Debug.LogError("keepTrailAfterImpact is enabled but no trail renderer could be found");

            return;
        }

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        print("now destroying projectile " + gameObject.name);
        Destroy(gameObject);
    }

    #region Functions

    public void DealDamage(IDamageable damageable, float damage, bool useWeaponDamageMultiplier)
    {
        float multipliedDamage = useWeaponDamageMultiplier ? damage * weaponDamageMultiplier : damage;
        float finalDamage = multipliedDamage;

        // for now weaponDamageMultiplier and damageFalloff can't be separated
        if (useWeaponDamageMultiplier)
        {
            float travelDistance = Vector3.Distance(spawnPoint, impactPoint);
            finalDamage = MathsExtension.CalculateDamageWithFalloff(multipliedDamage, travelDistance, falloffStats);
        }

        print("Projectile: Dealing " + finalDamage + " final damage, no clue to whom");
        damageable.TakeDamage(finalDamage);

        if (OnDamage != null) OnDamage(damageable.GetPlayerStatsObj());
    }

    public void ApplyInitialVelocity(bool forwards)
    {
        SetStayAtAttackPoint(false);

        if (forwards)
            rb.velocity = transform.forward * initialVelocity.magnitude;

        else
            rb.velocity = initialVelocity;
    }

    public void RedirectVelocity(Vector3 newDirection)
    {
        rb.velocity = rb.velocity.magnitude * newDirection.normalized;
    }

    #endregion

    #region Getters

    public Vector3 GetInitialVelocity(){ return initialVelocity; }

    public Vector3 GetSpawnPoint() { return spawnPoint; }

    #endregion

    #region Setters

    public void SetStayAtAttackPoint(bool value) { stayAtAttackPoint = value; }

    #endregion
}