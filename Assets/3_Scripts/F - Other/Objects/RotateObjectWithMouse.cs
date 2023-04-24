using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjectWithMouse : MonoBehaviour
{
    public float roationSpeed = 100;
    public float angularDrag = 5;
    private Rigidbody rb;
    private bool dragging;

    public void Setup(Vector3 dragColliderSize, float _rotationSpeed)
    {
        // rigidbody setup
        if(GetComponent<Rigidbody>() == null)
            rb = gameObject.AddComponent<Rigidbody>();
        else
            rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.angularDrag = angularDrag;

        // collider setup
        BoxCollider dragCollider = gameObject.AddComponent<BoxCollider>();
        dragCollider.size = dragColliderSize;

        roationSpeed = _rotationSpeed;
    }

    private void OnMouseDrag()
    {
        dragging = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }

    private void FixedUpdate()
    {
        if (dragging)
        {
            float x = Input.GetAxis("Mouse X") * roationSpeed * Time.fixedDeltaTime;
            float y = Input.GetAxis("Mouse Y") * roationSpeed * Time.fixedDeltaTime;

            rb.AddTorque(Vector3.down * x);
            rb.AddTorque(Vector3.right * y);
        }
    }
}
