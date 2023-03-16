using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public int hits = 5;

    private void OnCollisionEnter(Collision CollisionInfo)
    {
        if (CollisionInfo.gameObject.tag == "Bullet")
        {
            hits = hits - 1;
        }
    }

    private void Update()
    {
        if (hits == 0)
        {
            Destroy(gameObject);
        }
    }
}
