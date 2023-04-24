using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using RcLab;
using static WeaponHolder;
using System.Runtime.InteropServices;

[CreateAssetMenu(fileName = "New Ability", menuName = "RcLab/Abilities/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName = "New Ability";

    // weapon
    /// if true
    /// - ability will be looped
    /// - ability can be played if hand carries weapon
    public bool isWeapon = false;

    public WeaponType weaponType;
    public enum WeaponType
    {
        /// - directly save weapon in ability, no actions or anything required
        simpleWeapon,
        /// - use rangedActions, but use one general magazine,
        /// anything else would be way to complicated
        complexWeapon
    }
    [SerializeField] private RangedWeapon weapon;
    public ComplexWeaponData complexWeaponData;

    // actions
    public List<RcLab.Action> actions;
    [SerializeField] private float cooldown;

    // effects
    public EffectPlaylist effectPlaylist;

    // combo system (not implemented)
    public bool enableComboSystem = false;
    public ComboContinuation comboContinuation = ComboContinuation.ButtonHold;
    public float stopComboTimer = 1f;
    public enum ComboContinuation
    {
        ButtonHold,
        SeperateClicks
    }

    // inputs
    public InputHandling inputHandling;
    public enum InputHandling
    {
        ClickOnce,
        StopOnKeyUp
    }

    // interruptions
    /// what happens when an ability get's called during this ability
    public InterruptionHandling interruptionHandling = InterruptionHandling.StopAndPlay;
    public enum InterruptionHandling
    {
        Stop, // stop current ability, don't play next one
        StopAndPlay, // stop current ability, directly play next one
        PlaySimultaneously, // don't stop current ability, play next one simultaneously
        QueueUntilEnd, // don't stop current ability, play next one afterwards
        RefuseInterruption // don't stop current ability, don't play next one
    }
    public bool overrideInterruptionSettings;
    public bool canAlwaysInterrupt = false; // Stop or StopAndPlay = StopAndPlay, PlaySimultaneously or QueueUntilEnd or RefuseInterruption = Play Simultaneously
    public bool canNeverInterrupt = false; // All = RefuseInterruption, except QueueUntilEnd = QueueUntilEnd

    public int priority = 1; // maybe usefull but idk, 1 would be able to interrupt all weapons (priority = 0)

    // handsNeeded
    public bool overrideHandsNeeded;
    public HandsNeeded fixedHandsNeeded;

    // tool used
    public UltimateBlaster.BlasterType blasterType;

    // ui
    public bool enableUiDisplay;
    public Sprite abilityIcon;
    public Color abilityColor;

    /// internal
    private int _currActionIndex;
    private int _timerUntilNextAction;

    public bool StopOnKeyUp()
    {
        return inputHandling == InputHandling.StopOnKeyUp || isWeapon;
    }

    #region Getters

    public RcLab.Action GetNextActionInAbility(int currIndex)
    {
        currIndex++;
        if (currIndex == actions.Count)
            currIndex = 0;
        return actions[currIndex];
    }

    public bool IsSpawnOnlyOnMouseDown()
    {
        if (weaponType == WeaponType.simpleWeapon)
            return weapon.spawnOnlyOnMouseDown;
        else
            return complexWeaponData.spawnOnlyOnMouseDown;
    }

    public bool IsComplexWeaponAbility()
    {
        if(!isWeapon) return false;
        return weaponType == WeaponType.complexWeapon;
    }

    // just return the highest value
    public HandsNeeded GetHandsNeeded()
    {
        if (overrideHandsNeeded)
            return fixedHandsNeeded;

        HandsNeeded handsNeeded = HandsNeeded.None;

        if (isWeapon)
            handsNeeded = weaponType == WeaponType.simpleWeapon ? weapon.GetHandsNeeded() : complexWeaponData.handsNeeded;

        else
        {
            for (int i = 0; i < actions.Count; i++)
            {
                // set handsNeeded to both
                if (actions[i].GetHandsNeeded() == HandsNeeded.Both)
                    handsNeeded = HandsNeeded.Both;

                // set handsNeeded to one, if action needs one hand
                else if (actions[i].GetHandsNeeded() == HandsNeeded.One)
                    handsNeeded = HandsNeeded.One;

                // if both hands are needed, return value, no further checks needed
                if (handsNeeded == HandsNeeded.Both)
                    return handsNeeded;
            }
        }

        return handsNeeded;
    }

    public Magazine GetMag()
    {
        if (!isWeapon)
            return null;

        if (weaponType == WeaponType.simpleWeapon)
            return weapon.magazine;

        else if (weaponType == WeaponType.complexWeapon)
            return complexWeaponData.magazine;

        return null;
    }

    public RangedWeapon GetFirstRangedWeapon()
    {
        if (!isWeapon)
            return null;

        RangedWeapon firstWeapon = null;

        if (weaponType == WeaponType.simpleWeapon)
            firstWeapon = weapon;
        else if (weaponType == WeaponType.complexWeapon)
            firstWeapon = FindFirstRangedWeaponInActions();

        return firstWeapon;
    }

    private RangedWeapon FindFirstRangedWeaponInActions()
    {
        if (!isWeapon)
            return null;

        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].actionType == RcLab.Action.ActionType.Ranged)
                return actions[i].GetRangedWeapon();
        }

        Debug.LogError("This ability doesn't have any ranged weapons within the actions list");
        return null;
    }

    public List<RangedWeapon> GetWeapons()
    {
        if(!isWeapon) return null;

        List<RangedWeapon> weapons = new List<RangedWeapon>();

        if (weaponType == WeaponType.simpleWeapon)
            weapons.Add(weapon);
        else if (weaponType == WeaponType.complexWeapon)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].actionType == RcLab.Action.ActionType.Ranged)
                    weapons.Add(actions[i].GetRangedWeapon());
            }
        }

        return weapons;
    }

    public float GetCooldown() { return weaponType == WeaponType.simpleWeapon ? weapon.cooldown : cooldown; }

    #endregion

    #region Setters

    public void SetWeapon(RangedWeapon _weapon) { weapon = _weapon; }

    #endregion
}

public enum HandsNeeded
{
    None,
    One,
    Both
}

[Serializable]
public class ComplexWeaponData
{
    public Magazine magazine;
    public HandsNeeded handsNeeded;
    public bool spawnOnlyOnMouseDown;
}
