using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

[CreateAssetMenu(fileName = "New Action", menuName = "RcLab/Abilities/Actions/MeleeAction")]
public class MeleeAction : Action
{
    public new const ActionType actionType = ActionType.Melee;

    public MeleeWeapon weapon;
    public int iterations;

    public override float GetDuration()
    {
        float duration = 0f;
        duration += weapon.baseStats.chargeTime;
        duration += weapon.baseStats.hitAmount * weapon.baseStats.timeBetweenHits;
        duration += weapon.baseStats.cooldown;
        return duration;
    }

    public override float GetDurationWithoutCooldown()
    {
        float duration = 0f;
        duration += weapon.baseStats.chargeTime;
        duration += weapon.baseStats.hitAmount * weapon.baseStats.timeBetweenHits;
        return duration;
    }

    public override HandsNeeded GetHandsNeeded() { return weapon.GetHandsNeeded(); }
}
