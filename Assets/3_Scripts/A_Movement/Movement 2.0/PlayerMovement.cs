using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // references
    public Transform orientation;

    // ground movement
    private const string identifier_groundMovement = "GroundMovement";
    public float moveSpeed;
    [Range(1,100)] public float groundAccel;
    private Vector3 lastInputVector;

    // air movement
    private const string identifier_airMovement = "Jump";
    public float jumpSpeed;

    // scripts
    private PlayerInput input;
    private MovementCore core;

    private void Start()
    {
        input = PlayerReferences.instance.input;
        core = PlayerReferences.instance.movementCore;

        core.InitializeForce(identifier_groundMovement, groundAccel, moveSpeed, false, orientation);
        core.InitializeForce(identifier_airMovement, -1, -1, true, null);
    }

    private void Update()
    {
        GroundMovement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void GroundMovement()
    {
        Vector3 inputVector = new Vector3(input.horizontalInput, 0f, input.verticalInput);

        if(inputVector != lastInputVector)
        {
            core.UpdateForce(identifier_groundMovement, new Direction(Direction.Type.LocalDirection, inputVector));
        }

        lastInputVector = inputVector;
    }

    private void Jump()
    {
        Vector3 jumpDir = new Vector3(0f, 1f, 0f);

        core.ResetGravity();
        core.UpdateForce(identifier_airMovement, new Direction(Direction.Type.FixedVector, jumpDir), jumpSpeed);
    }
}
