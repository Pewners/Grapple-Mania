using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementCore : MonoBehaviour
{
    // references
    private Rigidbody rb;
    private Detector detector;

    // forces
    public bool useGravity = true;
    public float gravityStrength = 30;

    private const string identifier_gravity = "GravityVelocity";
    private Vector3 gravity;
    private float currentCombinedYVel;

    // forces
    public List<Force> forces = new List<Force>();
    private List<int> forcesToRemoveNextFrame = new List<int>();

    // debugging
    public bool debuggingEnabled;
    private const string debug_prefix = "MovementCore: ";
    /// optimizing
    private float timeTillNextSecond;
    private int addForceCalls;

    private void Start()
    {
        rb = PlayerReferences.instance.rb;
        detector = PlayerReferences.instance.detector;

        Invoke(nameof(DelayedStart), 0.05f);
    }

    private void DelayedStart()
    {
        // initialize debug line
        if (debuggingEnabled) /// debug
        {
            DebugExtensionManager.instance.CreateLine("TotalVelocity", Vector3.zero, Vector3.zero, "red", true);
            DebugExtensionManager.instance.CreateLine(identifier_gravity, Vector3.zero, Vector3.zero, "green", true);
        }
    }

    private void Update()
    {
        if (debuggingEnabled) /// debug
        {
            timeTillNextSecond -= Time.deltaTime;

            if(timeTillNextSecond <= 0)
            {
                timeTillNextSecond = 1;
                print(debug_prefix + addForceCalls + " AddForceCalls last second ");
                addForceCalls = 0;
            }
        }
    }

    #region Force Calculations

    // Here all the velocity calculations happen
    private void FixedUpdate()
    {
        detector.Check();

        RemoveExpiredForces();

        Vector3 velocity = Vector3.zero;
        velocity += CalculateVelocityOfAllForces();

        rb.velocity = velocity;
    }

    // usually this will never be more than one force per frame, so this should work just fine
    private void RemoveExpiredForces()
    {
        if (forcesToRemoveNextFrame.Count == 0) return;

        forces.RemoveAt(forcesToRemoveNextFrame[0]);
        forcesToRemoveNextFrame.RemoveAt(0);
    }

    private Vector3 CalculateVelocityOfAllForces()
    {
        Vector3 totalVelocity = Vector3.zero;

        for (int i = 0; i < forces.Count; i++)
        {
            Force force = forces[i];

            // update the force (this will set the _velocity of the force)
            force.FixedUpdate();

            // check force timer
            if (force.UpdateTimer())
            {
                forcesToRemoveNextFrame.Add(i);
                continue;
            }

            // get the force velocity
            Vector3 forceVelocity = force.GetVelocity();

            print("forceVel = " + forceVelocity);

            if (debuggingEnabled) /// debug
                DebugExtensionManager.instance.UpdateLine(force.identifier, transform.position, transform.position + forceVelocity);

            // add force velocity to total velocity
            totalVelocity += forceVelocity;

            print("totalVel = " + totalVelocity);
        }

        // gravity
        totalVelocity += CalculateGravity();
        CheckGravity();

        if (debuggingEnabled) /// debug
        {
            DebugExtensionManager.instance.UpdateLine("TotalVelocity", transform.position, transform.position + totalVelocity);
            DebugExtensionManager.instance.UpdateLine(identifier_gravity, transform.position, transform.position + gravity);
        } 

        return totalVelocity;
    }

    private Vector3 CalculateGravity()
    {
        if (!useGravity) return Vector3.zero;

        if (!detector.groundSensor.HasDetectedHit())
            gravity += Vector3.down * gravityStrength * Time.deltaTime;
        else
            gravity = Vector3.zero;

        return gravity;
    }

    public void ResetGravity() { gravity = Vector3.zero; }

    // check if current gravity is high enough to cancle out current upward forced
    private void CheckGravity()
    {
        print("currentYVel: " + rb.velocity.y + "combinedVel: " + currentCombinedYVel);

        if (currentCombinedYVel <= 0)
            return;

        if (Mathf.Abs(gravity.y) >= currentCombinedYVel)
        {
            RemoveGravityAffectedUpwardsVelocities();
            ResetGravity();
        }
    }

    #endregion

    #region AddForce

    // this will add a force with only an identifier and duration = -1 (default value)
    /// later this force can be updated using the same identifier and AddForce() <summary>
    /// Note: InitializeForce links directly to AddForce, it's just named more conveniently
    
    // full
    public void InitializeForce(string identifier, float accelleration = -1, float maxSpeed = -1, bool affectedByGravity = false, Transform transform = null) 
    { 
        AddForce(new Force(identifier, accelleration, maxSpeed, affectedByGravity, transform));
    }


    // full
    public void AddForce(string identifier, float accelleration = -1, float speed = 0, Direction direction = default, float duration = -1, bool affectedByGravity = false, Transform transform = null) 
    { 
        AddForce(new Force(identifier, accelleration, speed, direction, duration, affectedByGravity, transform)); 
    }
    public void AddForce(Force force)
    {
        if (ForceExists(force.identifier))
        {
            Debug.LogError("The force you're trying to add already exists, please use UpdateForce() instead");
            return;
        }

        forces.Add(force);

        if (debuggingEnabled) /// debug
        {
            print(debug_prefix + " Added new Force -> " + force.identifier + force.GetVelocity());
            DebugExtensionManager.instance.CreateLine(force.identifier, transform.position, transform.position + force.GetVelocity(), "blue", true);
            addForceCalls++;
        }

        UpdateCurrentCombinedYVel();
    }

    public void UpdateForce(string identifier, Direction direction, float speed = -1, float acceleration = -1)
    {
        if (!ForceExists(identifier))
        {
            Debug.LogError("You are trying to update a force that hasn't been added yet");
            return;
        }

        int index = GetForceIndex(identifier);
        forces[index].UpdateValues(direction, speed, acceleration);

        UpdateCurrentCombinedYVel();
    }

    private void UpdateCurrentCombinedYVel()
    {
        currentCombinedYVel = GetCombinedYVelocity();

        if (debuggingEnabled) // debug
            print(debug_prefix + " currentCombinedYVel has changed to " + currentCombinedYVel);
    }

    #endregion

    #region Setters

    public void DisableGravity() 
    {
        useGravity = false; 
    }
    public void EnableGravity(bool resetGravity = true) 
    {
        if(resetGravity) ResetGravity();
        useGravity = true; 
    }

    public void RemoveGravityAffectedUpwardsVelocities()
    {
        for (int i = 0; i < forces.Count; i++)
        {
            if (forces[i].affectedByGravity)
            {
                forces[i].ResetUpwardsVelocity();
            }
        }

        currentCombinedYVel = 0;
    }

    // this will reset all velocities of all forces to 0
    public void ResetVelocity()
    {
        for (int i = 0; i < forces.Count; i++)
        {
            forces[i].ResetVelocity();
        }

        currentCombinedYVel = 0;
    }

    #endregion

    #region Getters

    // get the current yVel of all forces (excluding gravity)
    public float GetCombinedYVelocity()
    {
        float yVel = 0f;

        for (int i = 0; i < forces.Count; i++)
        {
            yVel += forces[i]._velocity.y;
        }

        return yVel;
    }

    private int GetForceIndex(string identifier)
    {
        for (int i = 0; i < forces.Count; i++)
            if (forces[i].identifier == identifier)
                return i;
        return -1;
    }

    private bool ForceExists(string identifier)
    {
        for (int i = 0; i < forces.Count; i++)
            if (forces[i].identifier == identifier)
                return true;
        return false;
    }

    #endregion
}
