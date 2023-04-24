using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time;

    private void Start()
    {
        Invoke(nameof(DestroyGameObject), time);
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
