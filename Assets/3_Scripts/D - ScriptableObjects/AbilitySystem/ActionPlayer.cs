using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionPlayer : MonoBehaviour
{
    // references
    private Combat combat;

    private void Start()
    {
        combat = PlayerReferences.instance.combat;
    }

    public void PlayAction(Action action)
    {
        // ranged action
        if (action.actionType == Action.ActionType.Ranged)
        {
            RangedAction rangedAction = action as RangedAction;
            combat.PlayRangedWeapon(rangedAction.weapon);
        }

        // melee action
        if (action.actionType == Action.ActionType.Melee)
        {
            MeleeAction meleeAction = action as MeleeAction;
            // combat.PlayRangedAction(meleeAction);
        }

        // movement action
        if (action.actionType == Action.ActionType.Movement)
        {
            MovementAction movementAction = action as MovementAction;

            if (movementAction.movementActionType == MovementAction.MovementActionType.Dash)
            {
                DashMovementAction dashMovementAction = movementAction as DashMovementAction;
                // dashing.PerformDash(dashMovementAction);
            }
        }
    }
}
