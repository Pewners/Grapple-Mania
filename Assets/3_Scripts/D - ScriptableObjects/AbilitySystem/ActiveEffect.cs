using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

/// 
// The ActiveEffect class takes care of:
/// - storing all needed data for an active effect
/// - handling stack and duration amounts
/// 
// Important:
/// - the Update() function needs to be called every frame
/// - the OnEffectTick Action needs to be interpreted
/// - the ActiveEffect needs to be removed OnEffectExpiration 
///

[Serializable]
public class ActiveEffect
{
    // general info
    public StatEffectType statEffectType;
    public EffectApplyType applyType;
    public float tickDuration;
    private float timeTillNextTick;
    public int overrideMaxStacks = -1; // only overriden if enabled in corresponding StatEffectData
    public float overrideMaxDuration = -1f; // ""

    // internal
    public int stacks;
    public int stacksAdded;
    public float duration;
    public UnityAction<StatEffectType, int> OnEffectTick;
    public UnityAction<StatEffectType> OnEffectExpiration;

    #region Constructors

    public ActiveEffect(StatEffectType statEffectType, int stacks, float duration)
    {
        this.statEffectType = statEffectType;
        this.tickDuration = EffectDatabase.instance.GetTickDuration(statEffectType);
        this.applyType = EffectDatabase.instance.GetEffectApplyType(statEffectType);

        this.stacks = stacks;
        this.duration = duration;

        this.overrideMaxStacks = EffectDatabase.instance.GetOverrideMaxStacks(statEffectType);
        this.overrideMaxDuration = EffectDatabase.instance.GetOverrideMaxDuration(statEffectType);

        // instantly apply effect
        if (applyType == EffectApplyType.Constant || EffectDatabase.instance.IsInstantFirstTick(statEffectType))
            timeTillNextTick = 0.05f;
        // start timer
        else
            timeTillNextTick = tickDuration;
    }

    #endregion

    #region Update, Effect Tick, Expiration

    public void Update()
    {
        // -1 means the timer has been turned off
        if(timeTillNextTick != -1f)
        {
            timeTillNextTick -= Time.deltaTime;

            if (timeTillNextTick <= 0)
            {
                EffectTick();
            }
        }

        duration -= Time.deltaTime;

        if (duration <= 0f)
        {
            EffectExpired();
        }
    }

    // applies the effect
    private void EffectTick(bool forceConstantBehaviour = false)
    {
        // calculate how many stacks need to be applied
        // constant -> only add the stacks that haven't been added yet
        // tick -> always add all stacks (for example fire damage)
        bool constantBehaviour = applyType == EffectApplyType.Constant || forceConstantBehaviour;
        int stacksToAdd = constantBehaviour ? stacks - stacksAdded : stacks;

        if(OnEffectTick != null)
            OnEffectTick(statEffectType, stacksToAdd);
        else
            Debug.LogError("OnEffectTick Action couldn't be called, there is at least one subscriber needed");

        // store how many stacks are now applied
        if (constantBehaviour)
            stacksAdded = stacks;
        else
            stacksAdded += stacks;

        if(applyType == EffectApplyType.Constant)
            timeTillNextTick = -1f;
        else
            timeTillNextTick = tickDuration;
    }

    public void EffectExpired()
    {
        // make sure to revert changes of effect if needed
        bool revertChanges = EffectDatabase.instance.GetRevertChangesOnExipration(statEffectType);
        if (revertChanges)
        {
            stacks = 0;
            EffectTick(true);
        }

        if (OnEffectExpiration != null)
            OnEffectExpiration(statEffectType);
        else
            Debug.LogError("OnEffectExpiration Action couldn't be called, there is at least one subscriber needed");
    }

    #endregion

    #region Setters

    public void AddStack(int amount, int maxStacks)
    {
        if(overrideMaxStacks != -1)
            maxStacks = overrideMaxStacks;

        if(stacks + amount <= maxStacks)
            stacks += amount;
        else
            stacks = maxStacks;

        // for constant effects make sure to reapply new values
        if(applyType == EffectApplyType.Constant)
            EffectTick();

        Debug.Log("StatEffect: " + statEffectType + " new stack amount: " + stacks);
    }

    public void AddDuration(float amount, float maxDuration)
    {
        if(overrideMaxDuration != -1f)
            maxDuration = overrideMaxDuration;

        if (duration + amount <= maxDuration)
            duration += amount;
        else
            duration = maxDuration;

        Debug.Log("StatEffect: " + statEffectType + " new duration: " + duration);
    }

    #endregion
}
