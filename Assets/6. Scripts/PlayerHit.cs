using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public int hits = 5;
    public GameObject explosion;
    public AudioSource source;
    public AudioClip boom;

    private void Start()
    {
        explosion.SetActive(false);
    }

    private void OnCollisionEnter(Collision CollisionInfo)
    {
        if (CollisionInfo.gameObject.tag == "Bullet")
        {
            TakeDamage();
        }
    }

    public void TakeDamage ()
    {
        hits = hits - 1;
    }

    private void Update()
    {
        if (hits == 0)
        {
            Destroy(gameObject);
            explosion.SetActive(true);
            source.PlayOneShot(boom);
        }
    }
}
