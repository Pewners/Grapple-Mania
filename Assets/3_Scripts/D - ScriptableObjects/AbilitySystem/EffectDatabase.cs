
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDatabase : MonoBehaviour
{
    public static EffectDatabase instance;

    //public Dictionary<StatEffectType, StatEffectData> effects;
    public CustomDictionary<StatEffectType, StatEffectData> cd_effects;

    
    // public CustomDictionary<StatEffectType, ChangeableStat> cd_changeableStatLookup;

    private void Awake()
    {
        instance = this;

        /// don't know how much faster this would be
        // create a quick lookup table for statEffectTypes and affectedStats
        //foreach (StatEffectType type in effects.Keys)
        //{
        //    changeableStatLookup.Add(type, effects[type].affectedStat);
        //}
    }

    #region Getters

    public ChangeableStat GetChangeableStat(StatEffectType type)
    {
        return cd_effects[type].affectedStat;
    }

    public float GetTickDuration(StatEffectType type)
    {
        return cd_effects[type].tickDuration;
    }

    public bool IsInstantFirstTick(StatEffectType type) 
    {
        return cd_effects[type].instantFirstTick;
    }

    public StatChangeType GetStatChangeType(StatEffectType type)
    {
        return cd_effects[type].changeType;
    }

    public float GetStackValue (StatEffectType type)
    {
        return cd_effects[type].stackValue;
    }

    public EffectApplyType GetEffectApplyType (StatEffectType type)
    {
        return cd_effects[type].applyType;
    }

    public bool GetRevertChangesOnExipration (StatEffectType type)
    {
        return cd_effects[type].revertChangesOnExpiration;
    }

    public int GetOverrideMaxStacks(StatEffectType type)
    {
        return cd_effects[type].enableMaxStackAmounts ? cd_effects[type].overrideMaxStacks : -1;
    }

    public float GetOverrideMaxDuration (StatEffectType type)
    {
        return cd_effects[type].enableMaxDuration ? cd_effects[type].overrideMaxDuration : -1;
    }

    #endregion

    //[Button]
    //public void CopyDictionary()
    //{
     //   foreach (KeyValuePair<StatEffectType, ChangeableStat> keyValuePair in changeableStatLookup)
     //   {
     //       cd_changeableStatLookup.Add(keyValuePair.Key, keyValuePair.Value);
     //   }
    //}
}
