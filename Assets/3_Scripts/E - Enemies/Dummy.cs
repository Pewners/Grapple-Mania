using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dummy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    [Header("Start Pos <-> End Pos")]
    public Vector3 endPos;
    private Vector3 startPos;
    private bool walkToEndPos;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startPos = transform.position;
        endPos = transform.position + endPos;
    }

    private void FixedUpdate()
    {
        MoveToPosition();
    }

    private void MoveToPosition()
    {
        Vector3 distance = walkToEndPos ? endPos - transform.position : startPos - transform.position;

        if (rb.velocity.magnitude < moveSpeed)
            rb.AddForce(distance.normalized * moveSpeed * 10f, ForceMode.Force);

        if (distance.magnitude < 0.1f)
            walkToEndPos = !walkToEndPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + endPos);
    }
}
