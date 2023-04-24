using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testinghoming : MonoBehaviour
{
    [Header("Find Target")]
    public Transform target;

    [Header("References")]
    private Rigidbody rb;

    [Header("Movement")]
    public float speed = 15f;
    public float rotateSpeed = 95f;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;

        //PredictMovement(leadTimePercentage);
        //AddDeviation(leadTimePercentage);
        RotateRocket();
    }

    private void RotateRocket()
    {
        Vector3 heading = target.position - transform.position;

        //transform.forward = Vector3.Lerp(transform.forward, heading, rotateSpeed * Time.deltaTime);
        //return;

        Quaternion rotation = Quaternion.LookRotation(heading);

        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime));
    }

    private void OnDrawGizmosSelected()
    {
        
    }
}
