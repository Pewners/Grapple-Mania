using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{
    //this makes the bullet disappear on contact
    private void OnCollisionEnter(Collision CollisionInfo)
    {
        Destroy(gameObject);

        if (CollisionInfo.collider.tag == "Bullet")
        {
            Destroy(gameObject);
        }
    }
}