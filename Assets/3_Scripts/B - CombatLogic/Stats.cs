using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RcLab;
using TMPro;
using Dave;

/// The stats script should contain all variables that are going to be affected by the effect system
/// 
/// all variables exist as base and real variant, the real one is getting changed by effects
/// the real one is then also used by other scripts such as PlayerMovement
/// 
/// WeaponEffects are applied through the RangedWeapon class

namespace RcLab
{
    public class Stats : MonoBehaviour, IDamageable
    {
        public StatsType statsType;
        public enum StatsType
        {
            Player,
            Enemy
        }

        // base stats
        [SerializeField] private BasePlayerStats baseStats = new BasePlayerStats()
        {
            health = 100f,
            maxHealth = 100f,
            naturalRegeneration = 0f,
            shield = 0f,
            overShield = 0f,
            resistance = 0f,
            moveSpeed = 7f,
            strength = 0f,
            power = 0f,
        };

        // current stats as variables (dictionary would be better, but makes the code longer)
        // changeable stats
        [HideInInspector] public float health;
        [HideInInspector] public float maxHealth;
        [HideInInspector] public float naturalRegeneration;
        [HideInInspector] public float shield;
        [HideInInspector] public float overShield;
        [HideInInspector] public float resistance;
        [HideInInspector] public float moveSpeed;
        [HideInInspector] public float strength;
        [HideInInspector] public float power;

        // ragdoll explosion
        public bool useRagdollExplosion;
        public float ragdollExplosionForce;
        public AudioClipData destructionSound;
        public List<CameraEffect> explosionVisualEffects;

        // ingame values
        public string ingameBro;

        // references
        public Healthbar healthbar;
        private Inventory inv;

        // effects
        public List<ActiveEffect> playerEffects;
        public List<WeaponStatEffect> weaponEffects;

        // events
        public event UnityAction OnDestruction;

        public enum ChangeablePlayerStat
        {
            instantHealth, // value
            maxHealth, // value
            healthRegen, // health per second || percentage of max health per second
            moveSpeed, // value || percentage of maxSpeed -> change the basic walk and sprint speed
            overallMaxSpeedBoost, // value || percentage of maxSpeed -> add or remove speed from all sources (affects max speed)
            burning, // damage per second
            carryStrength // value (kg)
        }

        private void Awake()
        {
            ResetAllRealValues();
        }

        private void Start()
        {
            health = baseStats.health;
            inv = PlayerReferences.instance.inventory;

            playerEffects = new List<ActiveEffect>();
        }

        private void Update()
        {
            for (int i = 0; i < playerEffects.Count; i++)
            {
                playerEffects[i].Update();
            }
        }

        #region Damage and Health

        // -> TakeDamage(DamageType damageType)
        /// implement for fireDamage increase etc.
        public void TakeDamage(float damage, Color _color = default)
        {
            if (health <= 0) return;

            health -= damage;

            healthbar.UpdateHealthBar(maxHealth, health);

            print("damage dealt to: " + gameObject.name + " amount: " + damage);

            if (health <= 0)
            {
                if (statsType == StatsType.Player)
                    Death();
                else
                    Destruction();
            }

            // enemy stuff
            if (statsType == StatsType.Enemy)
            {
                Color color = _color == default ? Color.red : _color;

                float roundedDamage = MathsExtension.Round(damage, RcLabSettings.roundDamageToDecimals);

                if(roundedDamage > 0)
                    FindObjectOfType<DebugExtensionManager>().SpawnPopup(transform.position, DebugExtensionManager.PopupType.Damage, roundedDamage.ToString(), color);

                // make sure enemy reacts to damage
                if (GetComponent<EnemyAiTutorial>() != null)
                    GetComponent<EnemyAiTutorial>().sightRange = 100f;
                StopAllCoroutines();
                StartCoroutine(nameof(DelayedReset), 3f);
            }
        }
        private void DelayedReset()
        {
            if(TryGetComponent(out EnemyAiTutorial enemyAiTutorial))
                enemyAiTutorial.ResetSightRange();
        }

        public void Destruction()
        {
            // call events
            GameLogic.instance.OnEnemyDefeated();

            if (FindObjectOfType<CombatHallManager>() != null)
                FindObjectOfType<CombatHallManager>().OnEnemyDefeated();

            // remove whatIsEnemyLayer
            gameObject.layer = 0;

            if(!useRagdollExplosion)
                Invoke(nameof(DelayedDestruction), 0.1f);

            if (useRagdollExplosion)
                RagdollExplosion();

            if (OnDestruction != null) OnDestruction();

            // audio
            SoundManager.PlayEffect(destructionSound);

            // visuals
            EffectPlayer effectPlayer = PlayerReferences.instance.effectPlayer;
            for (int i = 0; i < explosionVisualEffects.Count; i++)
            {
                effectPlayer.PlayEffect(explosionVisualEffects[i]);
            }
        }

        private void RagdollExplosion()
        {
            Transform enemyModel = transform.Find("Enemy Model");
            ///Transform healthBar = transform.Find("EnemyCanvas").Find("Healthbar");

            for (int i = 0; i < enemyModel.childCount; i++)
            {
                enemyModel.GetChild(i).gameObject.AddComponent<BoxCollider>();
                Rigidbody rb = enemyModel.GetChild(i).gameObject.AddComponent<Rigidbody>();
                if(rb != null)
                    rb.AddExplosionForce(ragdollExplosionForce, transform.position, 3f);
            }

            healthbar.gameObject.SetActive(false);

            ///healthbar.GetComponent<BoxCollider>().enabled = true;
            ///Rigidbody rb1 = healthbar.gameObject.AddComponent<Rigidbody>();
            ///rb1.AddExplosionForce(ragdollExplosionForce, transform.position, 3f);

            Invoke(nameof(DelayedDestruction), 3f);
        }

        public void DelayedDestruction()
        {
            Destroy(gameObject);
        }

        public void Heal(float healing, bool exceedMaxHealth = false)
        {
            if (health + healing <= maxHealth || exceedMaxHealth)
                health += healing;

            else if (health + healing > maxHealth)
                health = maxHealth;

            healthbar.UpdateHealthBar(maxHealth, health);
        }

        public void Death()
        {
            GameLogic.instance.EndRun();
        }

        #endregion

        #region Player Effects

        public void InterpretEffectTick(StatEffectType type, int stacks)
        {
            EffectApplyType applyType = EffectDatabase.instance.GetEffectApplyType(type);

            float stackValue = EffectDatabase.instance.GetStackValue(type);
            float value = stackValue * stacks;

            print("Stats: interpreting " + type + " effect with " + stacks + " stacks and applyType " + applyType);

            // for constant effects without ticks -> just change according stat
            if (applyType == EffectApplyType.Constant)
            {
                ChangeStat(type, value);
            }

            // for non-constant effects with ticks -> call the corresponding function
            else if (applyType == EffectApplyType.Ticks)
            {
                // Fire
                if (type == StatEffectType.Fire)
                    TakeDamage(value, Color.yellow);

                // Posion
                else if (type == StatEffectType.Poison)
                    TakeDamage(value, Color.green);

                // Regeneration
                else if (type == StatEffectType.Regeneration)
                    Heal(value);

                // for anything else just assume a simple stat change is wanted
                else 
                    ChangeStat(type, value);
            }
        }

        public void AddEffect(StatEffect effect)
        {
            // add stacks and duration to already existing effect
            if (EffectExists(effect.statEffectType))
            {
                int maxStacks = effect.enableValueStacking ? effect.maxReacheableStacks : effect.stacks;
                float maxDuration = effect.enableDurationStacking ? effect.maxReacheableDuration : effect.duration;

                for (int i = 0; i < playerEffects.Count; i++)
                {
                    if (playerEffects[i].statEffectType == effect.statEffectType)
                    {
                        playerEffects[i].AddStack(effect.stacks, maxStacks);
                        playerEffects[i].AddDuration(effect.duration, maxDuration);
                    }
                }

                print("StatEffectStep 1: Added To Existing Effect " + effect.statEffectType);
            }

            // add new effect
            else
            {
                ActiveEffect newActiveEffect = new ActiveEffect(effect.statEffectType, effect.stacks, effect.duration);

                // subscribe to events
                newActiveEffect.OnEffectTick += InterpretEffectTick;
                newActiveEffect.OnEffectExpiration += RemoveEffect;

                playerEffects.Add(newActiveEffect);

                print("StatEffectStep 1: Added New Effect " + effect.statEffectType);
            }
        }

        public void RemoveEffect(StatEffectType type)
        {
            for (int i = 0; i < playerEffects.Count; i++)
            {
                if (playerEffects[i].statEffectType == type)
                {
                    // unsubscribe to events
                    playerEffects[i].OnEffectTick -= InterpretEffectTick;
                    playerEffects[i].OnEffectExpiration -= RemoveEffect;

                    playerEffects.RemoveAt(i);
                }
            }
        }

        public bool EffectExists(StatEffectType type)
        {
            for (int i = 0; i < playerEffects.Count; i++)
            {
                if (playerEffects[i].statEffectType == type)
                    return true;
            }

            return false;
        }

        public void ChangeStat(StatEffectType effectType, float value)
        {
            ChangeableStat changeableStat = EffectDatabase.instance.GetChangeableStat(effectType);
            StatChangeType statChangeType = EffectDatabase.instance.GetStatChangeType(effectType);

            float valueChange = 0;

            if (statChangeType == StatChangeType.FixedAmount)
                valueChange = value;

            if (statChangeType == StatChangeType.Percentage)
            {
                float baseValue = baseStats.GetValue(changeableStat);
                valueChange = baseValue * value * 0.01f;
            }

            AddToRealValue(changeableStat, valueChange);

            print("stat changed: " + changeableStat + " -> " + valueChange);
        }

        private void ResetAllRealValues()
        {
            health = baseStats.health;
            maxHealth = baseStats.maxHealth;
            naturalRegeneration = baseStats.naturalRegeneration;
            shield = baseStats.shield;
            overShield = baseStats.overShield;
            resistance = baseStats.resistance;
            moveSpeed = baseStats.moveSpeed;
            strength = baseStats.strength;
            power = baseStats.power;
        }

        public void AddToRealValue(ChangeableStat changeableStat, float value)
        {
            switch (changeableStat)
            {
                case ChangeableStat.Health:
                    health += value; break;

                case ChangeableStat.MaxHealth:
                    maxHealth += value; break;

                case ChangeableStat.NaturalRegeneration:
                    naturalRegeneration += value; break;

                case ChangeableStat.OverShield:
                    overShield += value; break;

                case ChangeableStat.Resistance:
                    resistance += value; break;

                case ChangeableStat.MoveSpeed:
                    moveSpeed += value; break;

                case ChangeableStat.Strength:
                    strength += value; break;

                case ChangeableStat.Power:
                    power += value; break;

                default:
                    Debug.LogError("Stat type " + changeableStat + " cannot be changed here");
                    break;
            }
        }

        #endregion

        #region Getters

        public Stats GetPlayerStatsObj()
        {
            return this;
        }

        #endregion
    }
}

[Serializable]
public class BasePlayerStats
{
    public float health;
    public float maxHealth;
    public float naturalRegeneration;
    public float shield;
    public float overShield;
    public float resistance;
    public float moveSpeed;
    public float strength;
    public float power;

    public float GetValue(ChangeableStat statType)
    {
        float value = 0f;

        switch (statType)
        {
            case ChangeableStat.Health:
                value = health; break;

            case ChangeableStat.MaxHealth:
                value = maxHealth; break;

            case ChangeableStat.NaturalRegeneration:
                value = naturalRegeneration; break;

            case ChangeableStat.Shield:
                value = shield; break;

            case ChangeableStat.OverShield:
                value = overShield; break;

            case ChangeableStat.Resistance:
                value = resistance; break;

            case ChangeableStat.MoveSpeed:
                value = moveSpeed; break;

            case ChangeableStat.Strength:
                value = strength; break;

            case ChangeableStat.Power:
                value = power; break;

            default:
                Debug.LogError("Value Type not implemented"); break;
        }

        return value;
    }
}
