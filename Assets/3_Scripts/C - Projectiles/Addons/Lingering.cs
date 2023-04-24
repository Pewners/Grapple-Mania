using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Lingering : ProjectileAddon
{
    private bool lingeringActive;

    [Header("Settings")]
    public float damagePerTick = 5f;
    public float damageInterval = 0.1f;
    public float maxDuration = 999f;
    public LayerMask whatIsEnemy;

    [Header("Event - OnExpired")]
    public OnExpiredAction onExpiredAction;
    public enum OnExpiredAction
    {
        Destroy,
        Explode
    }

    private bool damageTick;
    private Projectile projectile;

    private void Awake()
    {
        lingeringEnemies = new List<IDamageable>();
    }

    private void Start()
    {
        // make sure the collider is a trigger
        GetComponent<Collider>().isTrigger = true;

        projectile = GetComponent<Projectile>();

        StartLingering();
    }

    public void StartLingering()
    {
        lingeringActive = true;

        Invoke(nameof(NextDamageTick), damageInterval);
        Invoke(nameof(OnExpired), maxDuration);
    }

    public void StopLingering()
    {
        lingeringActive = false;
    }

    private void Update()
    {
        if (!lingeringActive) return;

        if (damageTick)
        {
            for (int i = 0; i < lingeringEnemies.Count; i++)
            {
                projectile.DealDamage(lingeringEnemies[i], damagePerTick, false);
            }

            damageTick = false;
            Invoke(nameof(NextDamageTick), damageInterval);
        }
    }

    private void NextDamageTick()
    {
        damageTick = true;
    }

    private List<IDamageable> lingeringEnemies;
    private void OnTriggerEnter(Collider other)
    {
        // filter out non-enemies
        if (other.gameObject.layer != (other.gameObject.layer | (1 << whatIsEnemy))) return;

        print("lingering enemy entered...");

        // filter out non-damageable-enemies
        if (!other.transform.TryGetComponent(out IDamageable damageable)) return;

        lingeringEnemies.Add(damageable);
    }

    private void OnTriggerExit(Collider other)
    {
        // filter out non-enemies
        if (other.gameObject.layer != (other.gameObject.layer | (1 << whatIsEnemy))) return;

        // filter out non-damageable-enemies
        if (!other.transform.TryGetComponent(out IDamageable damageable)) return;

        for (int i = 0; i < lingeringEnemies.Count; i++)
        {
            if (damageable == lingeringEnemies[i])
                lingeringEnemies.RemoveAt(i);
        }
    }

    private void OnExpired()
    {
        if (onExpiredAction == OnExpiredAction.Destroy)
            projectile.InvokeSilentDestruction(0.05f);

        if (onExpiredAction == OnExpiredAction.Explode)
        {
            // not bad dave!
            if(TryGetComponent(out Explosive explosive))
                explosive.Explode();
        }
    }
}
