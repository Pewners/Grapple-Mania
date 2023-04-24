using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dave;

/// addon to make projectiles explosive

public class Explosive : ProjectileAddon
{
    // Settings
    public float explosionRadius = 3;
    public float explosionForce = 25;
    public int explosionDamage = 30;
    public float autoExplosionDelay = 999;
    public LayerMask whatIsPushable = -1; // 9 and 13 (player and enemy) -> see void start

    // Mine extension
    public bool enableMine = false;
    public float mineDetectionRange;
    public LayerMask whatIsMineTrigger;

    // Premade settings
    public bool enablePremadeSettings;
    public ExplosiveSettings premadeSettings;

    // visual effects
    public bool enableVisualEffects;
    public ImpactEffectModifiable explosionVisualEffect;
    [HideInInspector] public int selected_VisualEffect;

    // sound
    public bool enableSound;
    public AudioClipData explosionSoundEffect;

    private Projectile projectile;

    private void Start()
    {
        whatIsPushable = MathsExtension.CombineLayerMask(9, 13);

        if(GetComponent<Projectile>() != null)
            projectile = GetComponent<Projectile>();

        if(enablePremadeSettings && premadeSettings != null)
            LoadPremadeSettings();

        Invoke(nameof(Explode), autoExplosionDelay);

        if (enableMine)
        {
            mineDeactivated = true;
            Invoke(nameof(ActivateMine), 1f);
        }
    }

    private void ActivateMine()
    {
        mineDeactivated = false;
    }

    private void Update()
    {
        if (enableMine)
            CheckForRangeEntries();
    }

    private bool mineDeactivated;
    public void CheckForRangeEntries()
    {
        if (mineDeactivated) return;

        bool entryDetected = Physics.CheckSphere(transform.position, mineDetectionRange, whatIsMineTrigger);

        if (entryDetected)
        {
            Explode();
            mineDeactivated = true;
        }
    }

    private Vector3 explosionPoint = Vector3.zero;
    public void OverrideExplosionPoint(Vector3 _explosionPoint)
    {
        explosionPoint = _explosionPoint;
    }

    public void Explode()
    {
        if (explosionPoint == Vector3.zero)
            explosionPoint = transform.position;

        if(explosionVisualEffect != null)
        {
            if(explosionVisualEffect.impactEffectType != GameAssets.ImpactEffect.None)
                GlobalEffectsSpawner.instance.SpawnImpactEffect(explosionVisualEffect, explosionPoint);
        }

        if(enableSound)
            SoundManager.PlayEffect(explosionSoundEffect);

        Collider[] objectsInRange = Physics.OverlapSphere(explosionPoint, explosionRadius, whatIsPushable);

        print("kaboom incoming on " + objectsInRange.Length);

        for (int i = 0; i < objectsInRange.Length; i++)
        {
            if (objectsInRange[i].gameObject == gameObject)
            {
                // don't break or return please, thanks
            }
            else
            {
                // check if object is enemy
                if (objectsInRange[i].TryGetComponent(out IDamageable damageable))
                {
                    print("Explosive: Initiating " + explosionDamage + " explosion damage to " + objectsInRange[i].gameObject.name);
                    projectile.DealDamage(damageable, explosionDamage, false);
                }

                // check if object has a rigidbody
                if (objectsInRange[i].GetComponent<Rigidbody>() != null)
                {
                    ///objectsInRange[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);

                    // custom explosionForce
                    Vector3 objectPos = objectsInRange[i].transform.position;
                    Vector3 forceDirection = (objectPos - explosionPoint).normalized;

                    ///objectsInRange[i].GetComponent<Rigidbody>().AddForce(forceDirection * explosionForce + Vector3.up * explosionForce, ForceMode.Impulse);
                    objectsInRange[i].GetComponent<Rigidbody>().AddForceAtPosition(forceDirection * explosionForce + Vector3.up * explosionForce, explosionPoint + new Vector3(0, -0.5f, 0), ForceMode.Impulse);

                    Debug.Log("Kabooom " + objectsInRange[i].name);
                }
            }
        }

        print("Working!!");
        projectile.InvokeDestruction(0.1f);
    }

    private void LoadPremadeSettings()
    {
        explosionRadius = premadeSettings.explosionRadius;
        explosionForce = premadeSettings.explosionForce;
        explosionDamage = premadeSettings.explosionDamage;
        autoExplosionDelay = premadeSettings.autoExplosionDelay;
    }

    private void OnDrawGizmosSelected()
    {
        if (!enableMine)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, mineDetectionRange);
        }
    }
}