using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PhysicMaterialCreator : ProjectileAddon
{
    // material settings

    [Range(0f,1f)]
    public float bounciness = 0;
    public float extraBounceForce = 0f;

    [Range(0f,1f)]
    public float friction = 0;

    public bool stickToSurface = false;

    private Rigidbody rb;

    void Start()
    {
        if (stickToSurface)
        {
            gameObject.AddComponent(typeof(StickToSurface));
            return;
        }

        Collider collider = GetComponent<Collider>();

        collider.material = NewPhysicMaterial();

        rb = GetComponent<Rigidbody>();

        GetComponent<Projectile>().OnCollision += Bounce;
    }

    private PhysicMaterial NewPhysicMaterial()
    {
        PhysicMaterial physicMaterial = new PhysicMaterial();

        physicMaterial.bounciness = bounciness;
        physicMaterial.staticFriction = friction;
        physicMaterial.dynamicFriction = friction;

        if(friction > bounciness)
        {
            physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        }
        if (bounciness > friction)
        {
            physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        }

        return physicMaterial;
    }

    private void Bounce()
    {
        if (extraBounceForce <= 0) 
            return;

        rb.AddForce(Vector3.up * extraBounceForce, ForceMode.Impulse);
    }
}
