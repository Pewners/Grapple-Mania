using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : ProjectileAddon
{
    // settings
    public float timeBeforeReturn = 0.75f;

    public float returnDuration = 1f;

    public Interpolation interpolation = Interpolation.Lerp;
    public enum Interpolation
    {
        Lerp,
        Slerp
    }

    // extras
    public bool returnOnCollision = true;
    public bool destroyOnReturn = true;
    // public bool restoreAmmoOnReturn = false;

    private Projectile projectile;
    private Transform player;
    private bool returning;

    private Vector3 startPosition;

    private float timeElapsed;

    private void Start()
    {
        projectile = GetComponent<Projectile>();
        player = projectile.originScript.transform;

        if (returnOnCollision)
        {
            projectile.OnCollision += Return;
        }

        Invoke(nameof(Return), timeBeforeReturn);
    }

    private void Return()
    {
        returning = true;

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (returning)
        {
            timeElapsed += Time.deltaTime;

            if (interpolation == Interpolation.Lerp)
                 transform.position = Vector3.Lerp(startPosition, player.position, timeElapsed / returnDuration);

            if (interpolation == Interpolation.Slerp)
                 transform.position = Vector3.Slerp(startPosition, player.position, timeElapsed / returnDuration);

            if(timeElapsed >= returnDuration)
            {
                if(destroyOnReturn) projectile.InvokeSilentDestruction(0.05f);
                returning = false;
            }
        }
    }
}
