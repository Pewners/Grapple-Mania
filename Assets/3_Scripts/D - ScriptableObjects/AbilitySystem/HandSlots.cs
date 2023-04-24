using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using UnityEngine.ProBuilder.MeshOperations;

[Serializable]
public class HandSlots
{
    public HandUsage rightHand; // slot 0 -> weaponIndex 0
    public HandUsage leftHand; // slot 1 -> weaponIndex 1

    #region Setters

    public void SetUsage(HandsNeeded handsNeeded, HandUsage usage = HandUsage.Ability, HandType preferredHand = HandType.Left)
    {
        if(handsNeeded == HandsNeeded.Both)
        {
            SetUsage(HandType.Left, usage);
            SetUsage(HandType.Right, usage);
        }
        else if (handsNeeded == HandsNeeded.One)
        {
            int freeHand = Find(HandUsage.Free, preferredHand);
            SetUsage(freeHand, usage);
        }
    }

    public void SetUsage(int handIndex, HandUsage usage)
    {
        HandType handType = handIndex == 0 ? HandType.Left : HandType.Right;
        SetUsage(handType, usage);
    }

    public void SetUsage(HandType handType, HandUsage usage)
    {
        if (handType == HandType.Left)
            leftHand = usage;
        else
            rightHand = usage;
    }

    #endregion

    #region Getters

    public int Find(HandUsage criteria, HandType preference)
    {
        if (preference == HandType.Right)
        {
            if (rightHand == criteria)
                return 0;
            else if (leftHand == criteria)
                return 1;
        }
        else if (preference == HandType.Left)
        {
            if (leftHand == criteria)
                return 1;
            else if (rightHand == criteria)
                return 0;
        }

        return -1;
    }

    public bool Check(HandsNeeded handsNeeded, bool isWeaponAbility)
    {
        if (handsNeeded == HandsNeeded.None)
            return true;

        if (handsNeeded == HandsNeeded.One)
        {
            bool oneHandCouldUseWeapon = leftHand == HandUsage.Weapon || rightHand == HandUsage.Weapon;
            bool oneHandIsFree = leftHand == HandUsage.Free || rightHand == HandUsage.Free;
            return isWeaponAbility ? oneHandCouldUseWeapon : oneHandIsFree;
        }

        else if (handsNeeded == HandsNeeded.Both)
        {
            bool bothHandsCouldUseWeapon = leftHand == HandUsage.Weapon && rightHand == HandUsage.Weapon;
            bool bothHandsAreFree = leftHand == HandUsage.Free && rightHand == HandUsage.Free;
            return isWeaponAbility ? bothHandsCouldUseWeapon : bothHandsAreFree;
        }

        return false;
    }

    public HandUsage GetUsage(HandType handType)
    {
        if(handType == HandType.Left)
            return leftHand;
        else
            return rightHand;
    }

    public bool CheckUsage(HandType handType, HandUsage usage)
    {
        if (handType == HandType.Left)
            return leftHand == usage;
        else
            return rightHand == usage;
    }

    public bool IsBothHandsFree()
    {
        return leftHand == HandUsage.Free && rightHand == HandUsage.Free;
    }

    #endregion
}

public enum HandType
{
    Left,
    Right,
    None
}

public enum HandUsage
{
    Free, // any weapon or ability could be played with this hand
    Weapon, // weapon can be used, no abilities
    Ability, // no weapon can be used, ability can only be played if the current one get's cancelled
    ///Carrying // carrying a weapon, can't be used unless weapon get's equipped or unequipped
}
