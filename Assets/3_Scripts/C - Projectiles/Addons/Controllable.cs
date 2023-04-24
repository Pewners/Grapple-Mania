using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RcLab;

/// makes objects controllable with normal player inputs (+ sets camera pos to transform.position)
/// works with directly setting the rb.velocity

[RequireComponent(typeof(Rigidbody))]
public class Controllable : ProjectileAddon
{
    // settings
    public float activationDelay;
    public bool moveCameraWithProjectile = true;
    public bool lockPlayerMovement = true;

    // movement
    public ControlMode controlMode;
    public enum ControlMode
    {
        Mouse,
        Keyboard2D,
        Keyboard3D,
        FullControl
    }

    public float maxSpeed;
    private float currMaxSpeed;
    public float moveForce;
    public float forwardThrust;
    public float timeUntillMaxSpeed;

    public AllowedDirections allowedDirections;

    // references
    public LayerMask whatIsPlayer;
    private PlayerMovement_MLab pm;
    private PlayerCam_MLab cam;
    private MoveCamera_MLab moveCam;
    private PlayerInput input;
    private Rigidbody rb;

    private bool controlActive;
    private Vector3 initialVelocity;
    private CamRotation initialCamRotation;

    private bool headbobWasEnabled;

    private void Start()
    {
        // check for player (directly besides spawnPoint) and get pm script reference
        pm = Physics.OverlapSphere(transform.position, 3, whatIsPlayer)[0].GetComponentInParent<PlayerMovement_MLab>();

        cam = pm.cam;

        moveCam = cam.cam.GetComponentInParent<MoveCamera_MLab>(); // clean code I know

        input = PlayerReferences.instance.input;

        rb = GetComponent<Rigidbody>();
        initialVelocity = GetComponent<Projectile>().GetInitialVelocity();

        Invoke(nameof(StartControl), activationDelay);
    }

    private void Update()
    {
        if (controlActive)
            LimitVelocity();
    }

    private void FixedUpdate()
    {
        if (controlActive)
            RemoteMovement();
    }

    private void StartControl()
    {
        if(lockPlayerMovement) pm.LockPlayerMovement(true);

        controlActive = true;

        DOVirtual.Float(0, maxSpeed, timeUntillMaxSpeed, v => { maxSpeed = v; });

        if (moveCameraWithProjectile)
        {
            moveCam.SetRemoteTarget(transform);
            UiManager.instance.ShowIngameUi(ViewIngameUi.IngameUiType.RocketControl);

            // save cam rotation
            initialCamRotation = cam.SaveCamRotation();
        }
        if (cam.hbEnabled)
        {
            headbobWasEnabled = true;
            cam.SetHeadBobEnabled(false);
        }
    }

    private void RemoteMovement()
    {
        if(controlMode == ControlMode.Keyboard3D || controlMode == ControlMode.FullControl)
        {
            // Step 1 - Calculate input direction
            Vector3 direction = new Vector3();

            if (allowedDirections.forward && input.verticalInput > 0)
                direction += moveCam.transform.forward;

            if (allowedDirections.backward && input.verticalInput < 0)
                direction += moveCam.transform.forward * -1;

            if (allowedDirections.sideways)
                direction += input.horizontalInput * moveCam.transform.right;

            if (allowedDirections.upAndDown)
            {
                if (input.spaceInput > 0) direction += moveCam.transform.up;
                if (input.shiftInput > 0) direction += moveCam.transform.up * -1;
            }

            // Step 2 - Add force
            rb.AddForce(direction * moveForce * 10f, ForceMode.Force);
        }

        if (controlMode == ControlMode.Keyboard2D)
        {
            // Step 1 - Calculate input direction
            Vector3 direction = new Vector3();

            if (allowedDirections.sideways)
                direction += input.horizontalInput * moveCam.transform.right;

            if (allowedDirections.upAndDown)
            {
                if (input.verticalInput > 0) direction += moveCam.transform.up;
                if (input.verticalInput < 0) direction += moveCam.transform.up * -1;
            }

            // Step 2 - Add force
            rb.AddForce(direction * moveForce * 10f, ForceMode.Force);
        }

        // continuous forward thrust (missiles)
        Vector3 forwardDirection = initialVelocity;

        if (controlMode == ControlMode.Mouse || controlMode == ControlMode.FullControl)
            forwardDirection = moveCam.transform.forward;

        rb.AddForce(forwardDirection * forwardThrust * 10f, ForceMode.Force);
    }

    private void LimitVelocity()
    {
        // not working currently, no clue why
        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
            print("Limited velocity to " + rb.velocity.magnitude);
        }
    }

    private void StopControl()
    {
        if(lockPlayerMovement) pm.UnlockPlayerMovement();

        controlActive = false;

        // reset cam position
        if (moveCameraWithProjectile)
        {
            moveCam.RemoveRemoteTarget();
            UiManager.instance.ShowIngameUi(ViewIngameUi.IngameUiType.Main);

            // reset cam rotation
            cam.LoadCamRotation(initialCamRotation, true);
        }
        if (headbobWasEnabled) cam.SetHeadBobEnabled(true);
    }

    private void OnDestroy()
    {
        StopControl();
    }
}

[Serializable]
public class AllowedDirections
{
    public bool forward = true;
    public bool backward = false;
    public bool sideways = false;
    public bool upAndDown = false;
}
