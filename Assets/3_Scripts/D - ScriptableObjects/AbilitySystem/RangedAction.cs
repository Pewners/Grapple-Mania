using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

[CreateAssetMenu(fileName = "New Action", menuName = "RcLab/Abilities/Actions/RangedAction")]
public class RangedAction : Action
{
    /// confusing with the editor
    //public new string actionName = "New Ranged Action";

    public new const ActionType actionType = ActionType.Ranged;

    public RangedWeapon weapon;

    public override RangedWeapon GetRangedWeapon()
    {
        return weapon;
    }

    public override float GetDuration()
    {
        return weapon.GetWeaponCycleDuration();
    }

    public override float GetDurationWithoutCooldown()
    {
        return weapon.GetWeaponCycleDurationWithoutCooldown();
    }

    public override float GetActiveDuration()
    {
        return weapon.GetWeaponCycleActiveDuration();
    }

    public override float GetCooldown()
    {
        return weapon.cooldown;
    }

    public override HandsNeeded GetHandsNeeded() { return weapon.GetHandsNeeded(); }
}
