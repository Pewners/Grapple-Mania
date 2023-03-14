using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {

        yRotation = transform.rotation.eulerAngles.y;
        xRotation = transform.rotation.eulerAngles.x;

        //Debug.Log(transform.rotation.eulerAngles);

        // make cursor invisible and unmoveable
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        //yRotation += Mathf.Clamp(yRotation, -120f, -60f);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 40f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, xRotation);
    }
}