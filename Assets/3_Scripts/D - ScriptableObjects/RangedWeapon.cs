using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using Dave;
using DG.Tweening;

/// RangedWeapon - RangedCombatLab
/// 
/// Content:
/// - store primary and secondary ability (left/right click)
/// - store up to 3 special abilities
/// - execute abilities based on player input
/// 
/// Note:
/// This class is a ScriptableObject, you can create and store it like as an asset



[Serializable]
[CreateAssetMenu(fileName = "New Weapon", menuName = "RcLab/Weapons/RangedWeapon")]
public class RangedWeapon : ScriptableObject
{
    #region Variables

    public string weaponName;

    public enum AttackMode
    {
        Projectile,
        ProjectileAndRaycast,
        RaycastOnly
    }

    /* Section 0 - Base stats */

    [SerializeField] public BaseWeaponStats baseStats;
    public bool spawnOnlyOnMouseDown = false;
    public bool countSalveAsOneShot = false;


    /* Section 1 - Changeable stats */

    [HideInInspector] public float barrelSize = 100f;

    [HideInInspector] public float force = 20f; // in Newton
    [HideInInspector] public float damageMultiplier;

    public bool enableDamageFalloff;
    /// only works for weapons with raycast
    [HideInInspector] public DamageFalloffStats damageFalloff;

    [HideInInspector] public float chargeTime = 0f;
    [HideInInspector] public float cooldown = 0f;

    [HideInInspector] public float recoilRigidbodyForce;
    [HideInInspector] public float recoilRigidbodyMaxSpeed;

    [Header("Not implemented yet...")]
    [HideInInspector] public float verticalAngle = 0f;
    [HideInInspector] public float horizontalAngle = 0f;
    [HideInInspector] public int ammoFireAmount = 1;
    [HideInInspector] public float timeBetweenShots = 0f;
    private int currIndexOfSalve;

    // needs to be implemented with weapon effects
    [HideInInspector] public StabilityStats stabilityNormal;
    [HideInInspector] public StabilityStats stabilityWithAds;

    [HideInInspector] public HandlingStats handling;



    /* Section 2 - Calculated stats */

    public Magazine magazine;

    /* Section 2.5 haha - Effects */

    public bool enableWeaponEffects;
    public List<WeaponEffectApplier> weaponEffectAppliers;
    public bool enableProjectileEffects;
    public List<ProjectileEffectApplier> projectileEffectAppliers;


    /* Section 3 - Settings */

    /// 3.1 - Spawn Points
    public bool useCustomSpawnPoints;
    [Tooltip("This will only take effect if useCustomSpawnPoints is enabled.")]
    public SpawnPosData spawnPosData;
    public bool useCameraAsSpawnPoint = false;
    [Tooltip("Projectiles will fly straight forward instead of aiming at the middle of the sreen.")]
    public bool useStrictCameraForward = false;

    /// 3.2 - Ammo Consumption
    public bool enableCustomAmmoConsumption;
    public bool consumeAmmoOnMouseHold = false;

    /// 3.3 - Fast Fire Buildup
    public bool enableFastFireBuildup;
    [Range(0.01f, 10f)] public float minCooldown;
    public float cooldownDecreaseOnFire;
    public float timerUntilReset = 1f;

    /// 3.4 - Overheating
    public bool enableOverheating;
    public float heatIncreaseOnFire;
    public float timeUntilHeatDecrease = 0.5f;
    public float heatDecreaseSpeed = 1f;
    public AnimationCurve heatEffectOnCooldown;
    [HideInInspector] public float heat;



    /* Section 4 - Effects */
    /// 4.1 - Weapon Sound
    public bool enableWeaponSound;
    public WeaponSounds weaponSounds;

    public bool enableCameraEffects;
    public Effect.CameraEffectType shakePreset;

    public bool enableMuzzleEffects;
    public MuzzleEffectModifiable muzzleEffect;
    [HideInInspector] public int selcted_muzzleEffect;

    public bool enablePostProcessingEffects;
    public List<PostProcessingEffect> postProcessingEffects;

    // old system
    private List<WeaponStatEffect> weaponEffects;

    #endregion



    #region Definitions

    /* All changeable weapon stats */
    public enum ChangeableWeaponStat
    {
        Force, // newton
        ChargeTime, // seconds
        Cooldown, // seconds
        AmmoFireAmount, // amount
        VerticalAngle, // degree
        HorizontalAngle, // degree
        MinimalSpread, // amount
        Spread, // amount
        /// RecoilCam, Vector3???
        RecoilRigidbodyForce // newton
    }

    #endregion


    #region WeaponEffects & Potions

    public void AddEffect(WeaponStatEffect weaponEffect)
    {
        if (weaponEffects == null)
            weaponEffects = new List<WeaponStatEffect>();

        weaponEffects.Add(weaponEffect);
        ApplyAllEffects(); /// for now just apply everything every time
    }

    public void ApplyAllEffects()
    {
        if (weaponEffects == null)
            weaponEffects = new List<WeaponStatEffect>();

        /* Step 0 - Set all real values to their base value */
        ResetAllRealValues();

        /* Step 1 - Apply all fixed amounts */
        for (int i = 0; i < weaponEffects.Count; i++)
        {
            if (weaponEffects[i].changeType == WeaponStatEffect.ChangeType.FixedAmount)
                ApplyEffect(weaponEffects[i]);
        }

        /* Step 2 - Apply all percentages */
        for (int i = 0; i < weaponEffects.Count; i++)
        {
            if (weaponEffects[i].changeType == WeaponStatEffect.ChangeType.Percentage)
                ApplyEffect(weaponEffects[i]);
        }
    }

    public void ApplyEffect(WeaponStatEffect effect)
    {
        float baseValue = GetBaseValue(effect.changeableWeaponStat);

        float valueChange = 0;

        if (effect.changeType == WeaponStatEffect.ChangeType.FixedAmount)
            valueChange = effect.value;

        if (effect.changeType == WeaponStatEffect.ChangeType.Percentage)
            valueChange = baseValue * effect.value * 0.01f;

        AddToRealValue(effect.changeableWeaponStat, valueChange);
    }

    private void ResetAllRealValues()
    {
        //barrelSize = baseStats.barrelSize;
        force = baseStats.force;
        chargeTime = baseStats.chargeTime;
        cooldown = baseStats.cooldown;
        ammoFireAmount = baseStats.projectileSpawnAmount;
        verticalAngle = baseStats.verticalAngle;
        horizontalAngle = baseStats.horizontalAngle;
        recoilRigidbodyForce = baseStats.recoilRigidbodyForce;
        recoilRigidbodyMaxSpeed = baseStats.recoilRigidbodyMaxSpeed;
        stabilityNormal = baseStats.stabilityNormal;
        stabilityWithAds =  baseStats.stabilityWithAds;
        handling = baseStats.handling;
        damageMultiplier = baseStats.damageMultiplier;
        damageFalloff = baseStats.damageFalloff;

        ///magazine.reloadTime = baseStats.reloadTime;

        // unchangeable through effects
        timeBetweenShots = baseStats.timeBetweenSpawns;
    }

    // recoilCam not implemented!
    private float GetBaseValue(ChangeableWeaponStat stat)
    {
        switch (stat)
        {
            case ChangeableWeaponStat.Force:
                return baseStats.force;

            case ChangeableWeaponStat.ChargeTime:
                return baseStats.chargeTime;

            case ChangeableWeaponStat.AmmoFireAmount:
                return baseStats.projectileSpawnAmount;

            case ChangeableWeaponStat.Cooldown:
                return baseStats.cooldown;

            case ChangeableWeaponStat.VerticalAngle:
                return baseStats.verticalAngle;

            case ChangeableWeaponStat.HorizontalAngle:
                return baseStats.horizontalAngle;

            case ChangeableWeaponStat.MinimalSpread:
                return baseStats.stabilityNormal.minimalSpread;

            case ChangeableWeaponStat.Spread:
                return baseStats.stabilityNormal.spread;

            case ChangeableWeaponStat.RecoilRigidbodyForce:
                return baseStats.recoilRigidbodyForce;

            default:
                Debug.LogError("Weapon base value could not be found");
                return 0f;
        }
    }

    private void AddToRealValue(ChangeableWeaponStat stat, float value)
    {
        switch (stat)
        {
            case ChangeableWeaponStat.Force:
                force += value;
                break;

            case ChangeableWeaponStat.ChargeTime:
                chargeTime += value;
                break;

            case ChangeableWeaponStat.AmmoFireAmount:
                ammoFireAmount += Mathf.RoundToInt(value);
                break;

            case ChangeableWeaponStat.Cooldown:
                cooldown += value;
                break;

            case ChangeableWeaponStat.VerticalAngle:
                verticalAngle += value;
                break;

            case ChangeableWeaponStat.HorizontalAngle:
                horizontalAngle += value;
                break;

            case ChangeableWeaponStat.MinimalSpread:
                stabilityNormal.minimalSpread += value;
                break;

            case ChangeableWeaponStat.Spread:
                stabilityNormal.spread += value;
                break;

            case ChangeableWeaponStat.RecoilRigidbodyForce:
                recoilRigidbodyForce += value;
                break;

            default:
                Debug.LogError("Weapon stat type could not be found");
                break;
        }
    }

    #endregion



    #region WeaponSettings - Functions

    public void IncreaseCurrIndexOfSalve()
    {
        currIndexOfSalve++;
        if(currIndexOfSalve == spawnPosData.GetSpawnPointAmount())
            currIndexOfSalve = 0;
    }

    public int GetCurrIndexOfSalve()
    {
        return currIndexOfSalve;
    }

    public void FastFireBuildupStep()
    {
        // make sure cooldown doesn't cross minimum
        if (cooldown - cooldownDecreaseOnFire < minCooldown) return;

        AddToRealValue(ChangeableWeaponStat.Cooldown, -cooldownDecreaseOnFire);
        Debug.LogError("ChangeNote: FastFireBuildup is not running with proper effects system");
    }

    public void FastFireBuildupReset()
    {
        cooldown = baseStats.cooldown;
    }

    public void HeatBuildupStep()
    {
        // make sure heat doesn't exceed maximum (100%, 1)
        if (heat + heatIncreaseOnFire > 1) return;

        heat += heatIncreaseOnFire;
        cooldown = baseStats.cooldown * heatEffectOnCooldown.Evaluate(heat);

        Debug.Log("heat " + heat + " heatEffectOnCooldown " + heatEffectOnCooldown);
        Debug.LogError("ChangeNote: HeatBuildup is not running with proper effects system");
    }

    public bool HeatDecreaseStep(float value)
    {
        if (heat - value < 0) return true;

        heat -= value;
        cooldown = baseStats.cooldown * heatEffectOnCooldown.Evaluate(heat);

        Debug.Log("heat " + heat + " heatEffectOnCooldown " + heatEffectOnCooldown);
        return false;
    }

    #endregion



    #region Getters

    public HandsNeeded GetHandsNeeded()
    {
        return baseStats.handling.handsNeeded;
    }

    public float GetWeaponCycleDuration()
    {
        float duration = 0f;
        Debug.Log("testNumbers: " + weaponName + " " + chargeTime + " " + ammoFireAmount + " " + cooldown);
        duration += chargeTime;
        duration += GetWeaponCycleActiveDuration();
        duration += cooldown;
        return duration;
    }

    public float GetWeaponCycleDurationWithoutCooldown()
    {
        float duration = 0f;
        duration += chargeTime;
        duration += GetWeaponCycleActiveDuration();
        return duration;
    }

    public float GetWeaponCycleActiveDuration()
    {
        return ammoFireAmount * timeBetweenShots;
    }

    #endregion



    #region Private SubClasses

    [Serializable]
    public class BaseWeaponStats
    {
        ///[Range(1f, 100f)] public float barrelSize = 100;
        public float force = 40;
        public float damageMultiplier = 1;

        [SerializeField] public DamageFalloffStats damageFalloff;

        [Range(0f, 10f)] public float chargeTime;
        [Range(0.01f, 10f)] public float cooldown;

        public int projectileSpawnAmount = 1;
        public float timeBetweenSpawns = 0f;

        public float verticalAngle = 0f;
        public float horizontalAngle = 0f; // implementation needed

        [SerializeField] public StabilityStats stabilityNormal;
        [SerializeField] public StabilityStats stabilityWithAds;
        public float adsFovZoomPercent = 5f;
        public float recoilRigidbodyForce = 0f;
        public float recoilRigidbodyMaxSpeed = 15f;

        [SerializeField] public HandlingStats handling;
    }

    [Serializable]
    public class DamageFalloffStats
    {
        [Range(0, 150)] public int startDistance = 150;
        [Range(0, 150)] public int endDistance = 150;
        public AnimationCurve damageMultiplierCurve;
    }

    [Serializable]
    public class StabilityStats
    {
        public float minimalSpread = 0f;
        public float spread = 0f;
        public Vector3 recoilCam = new Vector3(-2, 2, 0.35f);
    }

    [Serializable]
    public class HandlingStats
    {
        public float readyTime = 0.25f;
        public float awayTime = 0.25f;
        public float adsTime = 0.25f;
        public float mobility = 50;
        public HandsNeeded handsNeeded = HandsNeeded.Both;
    }

    [Serializable]
    public class WeaponSounds
    {
        public AudioClipData equipSound;
        public AudioClipData fireSound;
    }

    #endregion

}

#region Public Data Classes

[Serializable]
public class RangedWeaponContainer
{
    public string name;
    public List<RangedWeaponCycle> weaponCycles;
}

[Serializable]
public class RangedWeaponCycle
{
    public RangedWeapon weapon;
    public int iterations = 1;
}

[Serializable]
public class WeaponStatEffect : StatEffect
{
    public RangedWeapon.ChangeableWeaponStat changeableWeaponStat;
    public float value;
    public ChangeType changeType;

    public enum ChangeType
    {
        FixedAmount,
        Percentage
    }
}

#endregion

public static class ScriptableObjectExtension
{
    /// <summary>
    /// Creates and returns a clone of any given scriptable object.
    /// </summary>
    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        T instance = UnityEngine.Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }
}