using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public void DelayedDestroy(float timer)
    {
        Invoke(nameof(Destruction), timer);
    }

    public void Destruction()
    {
        Destroy(gameObject);
    }
}
