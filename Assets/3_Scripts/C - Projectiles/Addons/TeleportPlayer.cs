using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [Header("Settings")]
    public bool teleportOnHit = true;
    public int allowedTeleports = 1;

    public LayerMask whatIsDetectable;

    private void OnCollisionEnter(Collision collision)
    {
        if (allowedTeleports <= 0) return;

        if (whatIsDetectable == (whatIsDetectable | (1 << collision.gameObject.layer)))
        {
            print("detection");
            Teleport(collision.contacts[0].normal);
            allowedTeleports--;
        }
    }

    private void Teleport(Vector3 normal)
    {
        print("teleport");

        Transform player = GameObject.Find("Player (NoPhoton)").transform;

        player.position = transform.position + normal;
    }
}
