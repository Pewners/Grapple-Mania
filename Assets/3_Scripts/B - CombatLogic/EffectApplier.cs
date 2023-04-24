using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for now effectApplier is a simple class while the effects themselves are scriptable objects
// maybe I should write my own editor to edit scriptable objects inside of the inspector

[Serializable]
public class EffectApplier
{
    public string effectName;
}

[Serializable]
public class ProjectileEffectApplier : EffectApplier
{
    public Event applyOnEvent = Event.OnDamage;
    public enum Event
    {
        OnSpawn,
        OnDamage,
        OnExplosionDamage,
        OnLaserDamage
    }

    public Target applyToTarget = Target.Enemy;
    public enum Target
    {
        Player,
        Enemy
    }

    public List<StatEffect> effectsToApply;
}

[Serializable]
public class WeaponEffectApplier : EffectApplier
{
    public Event applyOnEvent = Event.WhileEquipped;
    public enum Event
    {
        WhileEquipped,
        OnEquipped,
        OnUnequipped,
        OnReload,
        OnReloadFinished,
        OnMagazineEmpty,
        OnCombo
    }

    public Target applyToTarget = Target.Player;
    public enum Target
    {
        Player,
        LastEnemyIfAvailable
    }

    public List<StatEffect> effectsToApply;
}

