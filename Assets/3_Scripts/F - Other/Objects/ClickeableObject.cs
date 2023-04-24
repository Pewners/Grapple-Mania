using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickeableObject : MonoBehaviour
{
    private float doubleClickThreshold = 0.25f;
    private int clicks;

    private bool reset;

    private void Start()
    {
        if(GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void OnMouseUp()
    {
        IncreaseClicks();

        print("Click");
    }

    private void IncreaseClicks()
    {
        clicks++;
        reset = false;

        if (clicks >= 2)
        {
            DoubleClick();
            clicks = 0;
            reset = true;
        }

        Invoke(nameof(SingleClick), doubleClickThreshold);
    }

    private void SingleClick()
    {
        if (reset) return;

        clicks = 0;

        //transform.parent.GetComponent<WeaponModificationSlot>().OnSingleClick();
    }

    private void DoubleClick()
    {
        //transform.parent.GetComponent<WeaponModificationSlot>().OnDoubleClick();
    }
}
