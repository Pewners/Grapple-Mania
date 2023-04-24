using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatEffectData", menuName = "RcLab/System/Stats/StatEffectData")]
public class StatEffectData : ScriptableObject
{
    [Header("Definition")]

    public StatEffectType statEffectType;

    public List<EffectTarget> effectTargets;

    public ChangeableStat affectedStat;

    public EffectApplyType applyType = EffectApplyType.Constant;

    public StatChangeType changeType = StatChangeType.Percentage;

    public string negativeName = "";

    public bool revertChangesOnExpiration = true;


    [Header("Settings")]

    // only for EfffectApplyType.Ticks
    [Tooltip("-1 means constantly applied")]
    public float tickDuration = -1;
    [Tooltip("automatically true if tickDuration is -1")]
    public bool instantFirstTick = false;

    public float stackValue;

    public bool enableMaxStackAmounts = false;
    public int overrideMaxStacks = 1;

    public bool enableMaxDuration = false;
    public float overrideMaxDuration;


    [Header("Graphics")]

    [Tooltip("displayed ingame")]
    public Sprite icon;
    [Tooltip("displayed on information screen, can be left empty if icon should be used")]
    public Sprite artwork;
}

#region Enum Definitions

    // the effect names, fire, poision, speed etc.
    public enum StatEffectType
    {
        Fire, // ticks -> TakeDamage()
        Poison, // ticks -> TakeDamage()
        Rooted, // constant -> moveSpeed to 0
        Hacked, // constant 
        Bound, // constant 
        Disoriented, // constant 
        Shield, // constant -> shield
        Regeneration, // ticks -> Heal()
        InstantHealth, // constant -> health
        Overshield, // constant -> overShield
        Vitality, // constant -> maxHealth
        Resistance, // constant -> resistance
        Speed, // constant -> moveSpeed
        Strength, // constant -> strength
        Inflamed,
        Power // constant -> power
    }

    // which stat should be changed
    public enum ChangeableStat
    {
        /// Player Only
        Health,
        MaxHealth,
        NaturalRegeneration,
        Regeneration,
        Shield,
        OverShield,
        Resistance,
        MoveSpeed,
        Strength,
        Power,

        /// Weapons General
        Force,
        ChargeTime,
        Cooldown,
        ReloadTime,
        RecoilRigidbodyForce,
        Spread
    }

    // when the effect is applied
    public enum EffectApplyType
    {
        Constant,
        Ticks
    }

    // how the stats are changed
    public enum StatChangeType
    {
        FixedAmount,
        Percentage
    }

    // to what objects this effect can be applied
    public enum EffectTarget
    {
        Player,
        MeleeWeapon,
        RangedWeapon
    }

#endregion