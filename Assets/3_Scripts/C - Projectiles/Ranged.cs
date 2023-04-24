using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ranged : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject projectile;
    public TextMeshProUGUI text_info;
    public string projectileName;

    [Header("Stats")]
    public float timeBetweenSpawns;

    [Header("Throwing")]
    public float forwardForce;
    public float upwardForce;
    public float spread;
    public int spawnsAmount;

    private int spawnsToExecute;

    [Header("RayCasting")]
    public bool useRaycasts;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //Graphics
    ///public CamShake camShake;
    ///public float camShakeMagnitude, camShakeDuration;

    private void Start()
    {
        cam = Camera.main.transform;
        attackPoint = GameObject.Find("ThrowAttackPoint").transform;
    }

    public void InvokeRangedAttack(float delay)
    {
        Invoke(nameof(SpawnProjectileCall), delay);

        print("ranged attack invoked");
    }

    public void SpawnProjectileCall()
    {
        spawnsToExecute = spawnsAmount;

        print("spawn is being executed...");

        SpawnProjectile();
    }

    public void SpawnProjectile()
    {
        // spread
        float xSpread = UnityEngine.Random.Range(-spread, spread) * 0.01f;
        float ySpread = UnityEngine.Random.Range(-spread, spread) * 0.01f;

        Vector3 spreadDirectionOffset = cam.transform.right * xSpread + cam.transform.up * ySpread;

        // instantiate projectile

        GameObject _projectile = Instantiate(projectile, attackPoint.position, cam.rotation);

        // add force to your projectile (+ calculate direction)
        Rigidbody _projectileRb = _projectile.GetComponent<Rigidbody>();

        // direciton with spread
        Vector3 forceDirection = cam.transform.forward + spreadDirectionOffset;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized + spreadDirectionOffset;
        }

        Vector3 forceToAdd = forceDirection.normalized * forwardForce + transform.up * upwardForce;

        print(forceToAdd.magnitude + " Force to add " + _projectileRb);

        _projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        print("force added -> " + _projectileRb.velocity.magnitude);

        // use raycasts if needed
        if (useRaycasts)
        {
            if (Physics.Raycast(cam.transform.position, forceDirection, out rayHit, whatIsEnemy))
            {
                Debug.Log(rayHit.collider.name);

                ///if (rayHit.collider.CompareTag("Enemy"))
                ///    rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
            }
        }

        // camerashake
        ///camShake.Shake(camShakeDuration, camShakeMagnitude);
        spawnsToExecute--;

        // execute multiple throws per tap
        if (spawnsToExecute > 0)
            Invoke(nameof(SpawnProjectile), timeBetweenSpawns);
    }

    private float EstimateTimeToImpact()
    {
        return 0.5f;
    }

    public void LoadSkillData(RangedData data)
    {
        projectileName = data.projectileName;

        timeBetweenSpawns = data.timeBetweenSpawns;

        forwardForce = data.forwardForce;
        upwardForce = data.upwardForce;
        spread = data.spread;
        spawnsAmount = data.spawnsAmount;

        useRaycasts = data.useRaycasts;

        // set new projectile
        projectile = Resources.Load<GameObject>(projectileName);

        ///if (data.triggerInstantly) 
        ///    ThrowCall();
    }
}

[Serializable]
public class RangedData
{
    ///public bool triggerInstantly;

    public string projectileName = "DefaultProjectile";

    public float timeBetweenSpawns;

    public float forwardForce;
    public float upwardForce;
    public float spread;
    public int spawnsAmount;

    public bool useRaycasts;
}