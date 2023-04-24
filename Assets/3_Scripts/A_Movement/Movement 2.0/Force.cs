
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


[Serializable]
public class Force
{
    public string identifier;

    // force
    /// public AnimationCurve accelerationCurve;
    public float acceleration = -1;
    /// 100 means instantaneously (or -1 means the same thing)

    public float maxSpeed;
    public Direction direction;
    public Transform masterTransform = null;

    // duration
    public float duration = -1;
    /// -1 means as long as called
    /// the duration that force is actively being added (after that starts the force falloff)

    public bool affectedByGravity = false;

    // internal //
    public Vector3 _velocity = Vector3.zero;
    private Vector3 _targetVelocity = Vector3.zero;

    private Vector3 _direction = Vector3.zero;

    private State state = State.Accelerating;
    public enum State
    {
        Accelerating,
        MaxSpeed
    }

    public bool debuggingEnabled = true;

    #region UpdateForce

    // update tick
    /// -> curr speed is getting calculated
    public void FixedUpdate()
    {
        _direction = direction.GetNormalizedVector(masterTransform);
        _targetVelocity = _direction * maxSpeed;

        if (acceleration == -1)
            EnterState(State.MaxSpeed);

        // set velocity
        if (state == State.Accelerating)
        {
            _velocity = Vector3.Lerp(_velocity, _targetVelocity, (acceleration / 100));

            // to save processing power switching to MaxSpeed state at some point would be useful here

            // if _velocity is identical to target velocity, switch states
            /// if (_velocityInterpolateDelta >= 1)
            ///    EnterState(State.MaxSpeed);
        }
        else if (state == State.MaxSpeed)
        {
            _velocity = _targetVelocity;
        }

        if (debuggingEnabled) /// debug
            Debug.Log("Force Update: " + identifier + " state: " + state);
    }

    public void EnterState(State stateToEnter)
    {
        state = stateToEnter;

        if(debuggingEnabled) /// debug
            Debug.Log("Force: " + identifier + " enteredState: " + state);
    }

    public void UpdateValues(Direction direction = default, float maxSpeed = -1, float acceleration = -1)
    {
        this.direction = direction;
        if(maxSpeed != -1) this.maxSpeed = maxSpeed;
        if(acceleration != -1) this.acceleration = acceleration;
        // run update loop just to set the _velocity correctly
        FixedUpdate();
    }

    public bool UpdateTimer()
    {
        if (duration == -1) return false;

        duration -= Time.fixedDeltaTime;

        return duration <= 0;
    }

    #endregion

    #region Constructors

    /// sets all defaults that can't be set directly in the variable
    public void SetDefaults()
    {
        direction = new Direction(Direction.Type.FixedVector, Vector3.zero);
    }

    // minimum
    public Force(string identifier, float acceleration, float maxSpeed, bool affectedByGravity, Transform transform)
    {
        SetDefaults();
        this.identifier = identifier;
        this.acceleration = acceleration;
        this.maxSpeed = maxSpeed;
        this.affectedByGravity = affectedByGravity;
        this.masterTransform = transform;
        EnterState(0);
        FixedUpdate();
    }


    // full
    public Force(string identifier, float acceleration, float maxSpeed, Direction direction, float duration, bool affectedByGravity, Transform transform)
    {
        this.identifier = identifier;
        // this.accelerationCurve = accelerationCurve;
        this.acceleration = acceleration;
        this.maxSpeed = maxSpeed;
        this.direction = direction;
        this.duration = duration;
        this.affectedByGravity = affectedByGravity;
        this.masterTransform = transform;
        EnterState(0);
        FixedUpdate();
    }

    #endregion

    #region Setters

    public void ResetUpwardsVelocity()
    {
        // don't change _velocity, instead remove upwards values out of direction
        direction.RemoveUpwardsAxis();
    }

    public void ResetVelocity()
    {
        direction.ResetToZero();
    }

    #endregion

    #region Getters

    public Vector3 GetDirection(Transform transform = null) { return direction.GetNormalizedVector(masterTransform); }

    public Vector3 GetVelocity() { return _velocity; }

    #endregion
}
