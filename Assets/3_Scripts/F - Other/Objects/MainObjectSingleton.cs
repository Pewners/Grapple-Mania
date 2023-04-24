using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectSingleton : MonoBehaviour
{
    public static MainObjectSingleton instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
