using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

[CreateAssetMenu(fileName = "New Action", menuName = "RcLab/Abilities/Actions/Movement")]
public class MovementAction : Action
{
    public new const ActionType actionType = ActionType.Movement;
    public MovementActionType movementActionType;

    public enum MovementActionType
    {
        Dash,
        Jump
    }

    public override float GetDuration()
    {
        return base.GetDuration();
    }

    public override HandsNeeded GetHandsNeeded() { return HandsNeeded.None; }
}

//[CreateAssetMenu(fileName = "New Action", menuName = "RcLab/Abilities/Actions/Movement/Dash")]
public class DashMovementAction : MovementAction
{
    public new const MovementActionType movementActionType = MovementActionType.Dash;

    // public bool enableForceBounce;
    // [Range(0f,1f)] public float forceBounceStrength;
}
