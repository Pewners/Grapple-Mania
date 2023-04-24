using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    // section 1 - base stats
    public BaseWeaponStats baseStats;
    public BaseWeaponStats stats;

    // section 2 - magazine
    public Magazine magazine;

    // section 3 - sound, vfx, camera
    public float cameraFollowStrength;
    /// copy from RangedWeapon

    /* All changeable weapon stats */
    public List<WeaponStatEffect> weaponStatEffects;
    public enum ChangeableWeaponStat
    {
        Damage, // newton
        ChargeTime, // seconds
        Cooldown, // seconds
        HitAmount, // amount
        KnockbackForce, // newton
        DashForce // newton
    }

    #region Getters

    public HandsNeeded GetHandsNeeded() { return baseStats.handsNeeded; }

    #endregion

    [Serializable]
    public class BaseWeaponStats
    {
        public float hitDamage = 40;
        [Range(0f, 10f)] public float chargeTime;
        [Range(0.01f, 10f)] public float cooldown;

        public HitAngle hitAngle;

        public float knockbackForce = 20f;
        public float dashForce = 0f;

        public HandsNeeded handsNeeded;

        public int hitAmount = 1;
        [Range(0.1f, 1f)] public float timeBetweenHits = 0f;
    }

    [Serializable]
    public class HitAngle
    {
        [Range(-90f, 90f)] public float minAngle;
        [Range(-90f, 90f)] public float maxAngle;
        public HitDirection hitDirection;
        public enum HitDirection
        {
            AlternateLeftRight,
            RandomLeftRight,
            OnlyLeft,
            OnlyRight
        }
    }

    [Serializable]
    public class WeaponStatEffect : StatEffect
    {
        public MeleeWeapon.ChangeableWeaponStat changeableWeaponStat;
        public float value;
        public ChangeType changeType;

        public enum ChangeType
        {
            FixedAmount,
            Percentage
        }
    }
}
