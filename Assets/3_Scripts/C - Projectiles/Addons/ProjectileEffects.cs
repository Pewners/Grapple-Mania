using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

[RequireComponent(typeof(Projectile))]
public class ProjectileEffects : MonoBehaviour
{
    public List<ProjectileEffectApplier> effectAppliers;

    private List<ProjectileEffectApplier.Event> eventsSubscribedTo;

    private Projectile projectile;

    private void Start()
    {
        projectile = GetComponent<Projectile>();

        SubscribeToEvents();
    }

    #region Events

    private void SubscribeToEvents()
    {
        eventsSubscribedTo = new List<ProjectileEffectApplier.Event>();

        for (int i = 0; i < effectAppliers.Count; i++)
        {
            if (!IsSubscribedToEvent(effectAppliers[i].applyOnEvent))
            {
                if (effectAppliers[i].applyOnEvent == ProjectileEffectApplier.Event.OnSpawn)
                {
                    projectile.OnSpawn += OnSpawn;
                    eventsSubscribedTo.Add(ProjectileEffectApplier.Event.OnSpawn);
                }

                if (effectAppliers[i].applyOnEvent == ProjectileEffectApplier.Event.OnDamage)
                {
                    projectile.OnDamage += OnDamage;
                    eventsSubscribedTo.Add(ProjectileEffectApplier.Event.OnDamage);
                }
            }
        }
    }

    private bool IsSubscribedToEvent(ProjectileEffectApplier.Event _event)
    {
        for (int i = 0; i < eventsSubscribedTo.Count; i++)
        {
            if (eventsSubscribedTo[i] == _event)
                return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void UnsubscribeToEvents()
    {
        for (int i = 0; i < eventsSubscribedTo.Count; i++)
        {
            if (eventsSubscribedTo[i] == ProjectileEffectApplier.Event.OnSpawn)
                projectile.OnSpawn -= OnSpawn;
            if (eventsSubscribedTo[i] == ProjectileEffectApplier.Event.OnDamage)
                projectile.OnDamage -= OnDamage;
        }
    }

    #endregion

    #region Applying Effects

    // pass all OnSpawn effectAppliers to apply function
    public void OnSpawn()
    {
        for (int i = 0; i < effectAppliers.Count; i++)
            if (effectAppliers[i].applyOnEvent == ProjectileEffectApplier.Event.OnSpawn)
                ApplyEffectsToTarget(i);
    }

    // Filter - Event
    // pass all OnDamage effectAppliers to apply function
    public void OnDamage(Stats target)
    {
        for (int i = 0; i < effectAppliers.Count; i++)
            if (effectAppliers[i].applyOnEvent == ProjectileEffectApplier.Event.OnDamage)
                ApplyEffectsToTarget(i, target);
    }

    // Filter - Target
    public void ApplyEffectsToTarget(int effectApplierIndex, Stats enemyTarget = null)
    {
        Stats target = null;

        // Target - Player
        if (effectAppliers[effectApplierIndex].applyToTarget == ProjectileEffectApplier.Target.Player)
            target = PlayerReferences.instance.stats;

        // Target - Enemy
        else if (effectAppliers[effectApplierIndex].applyToTarget == ProjectileEffectApplier.Target.Enemy)
            target = enemyTarget;

        if (target == null)
        {
            Debug.LogError("Can't apply effects since no target was found");
            return;
        }

        // Apply effects
        for (int i = 0; i < effectAppliers[effectApplierIndex].effectsToApply.Count; i++)
        {
            print("applying " + effectAppliers[effectApplierIndex].effectsToApply[i].statEffectType + " to " + target.gameObject.name);
            target.AddEffect(effectAppliers[effectApplierIndex].effectsToApply[i]);
        }
    }

    #endregion
}