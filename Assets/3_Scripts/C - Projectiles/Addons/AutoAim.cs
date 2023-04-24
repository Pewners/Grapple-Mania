using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

/// used to aim at targets while moving -> it's a bit of copied code from projectileSpawnerConnection
/// doing it this way allows for more customization on which projectiles to use when, so I guess that's worth it...

/// Update:
/// I would like this to be the main component used for projectiles, turrets and enemy attacking (basic attacks only)
/// It's kind of like the alternative of the playerInput and playerCam script for Npcs

[RequireComponent(typeof(Combat))]
public class AutoAim : ProjectileAddon
{
    // references
    private Transform attackPoint;

    // settings
    public float detectionRange;
    public LayerMask whatIsTarget;

    // projectiles to spawn
    public List<RangedWeaponContainer> rangedWeaponContainer;
    public RangedWeapon weapon;
    private Combat combat;
    /// public int totalSpawnAmount;

    // attacking
    public State state;
    private State lastState;
    public enum State
    {
        Attacking,
        Searching,
        Deactivated
    }

    // limitations
    public float maxLookUpAngle = 90f;
    public float maxLookDownAngle = 90f;

    /// private int spawnsLeft;

    private GlobalProjectileSpawner gps;

    private GameObject bannedTarget;

    private void Start()
    {
        combat = GetComponent<Combat>();
        attackPoint = PlayerReferences.instance.weaponHolder.GetCurrAttackPointTransform();

        ///spawnsLeft = totalSpawnAmount;

        ///if (totalSpawnAmount == -1) spawnsLeft = 999999;

        gps = FindObjectOfType<GlobalProjectileSpawner>();

        // check for player (directly besides spawnPoint) and bann him as target
        bannedTarget = FindObjectOfType<PlayerMovement_MLab>().gameObject;
        print("Banned target: " + bannedTarget.name);
    }


    private bool targetInRange;
    private void Update()
    {
        StateHandler();
    }

    private void StateHandler()
    {
        // State 0 - Deactivated
        if (state == State.Deactivated)
        {
            /* Do Nothing */
            /// Gets controlled by other scripts
        }

        // State 1 - Attacking
        else if (state == State.Attacking)
        {
            // Attacking -> Searching
            if (FindTargetPosition() == Vector3.zero || !SimpleValidateAngle()) state = State.Searching;

            // update attack point
            attackPoint.LookAt(FindTargetPosition());
        }

        // State 2 - Searching
        else if(state == State.Searching)
        {
            // Searching -> Attacking
            if (FindTargetPosition() != Vector3.zero && SimpleValidateAngle()) state = State.Attacking;
        }

        bool stateHasChanged = state != lastState;

        if (stateHasChanged)
        {
            if (state == State.Attacking)
            {
                combat.PlayRangedWeapon(weapon);
            }
            else
            {
                //combat.StopRangedWeapon(weapon);
            }
        }

        lastState = state;
    }

    private Vector3 FindTargetPosition()
    {
        Collider[] targetColliders = Physics.OverlapSphere(transform.position, detectionRange, whatIsTarget);

        float closestDistance = detectionRange;
        int closestTargetIndex = -1;

        for (int i = 0; i < targetColliders.Length; i++)
        {
            if (targetColliders[i].gameObject == bannedTarget) { /* do nothing */ }

            else
            {
                float distance = Vector3.Distance(transform.position, targetColliders[i].transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = detectionRange;
                    closestTargetIndex = i;
                }
            }
        }

        if (closestTargetIndex != -1 && targetColliders[closestTargetIndex].gameObject != bannedTarget)
            return targetColliders[closestTargetIndex].transform.position;

        else 
            return Vector3.zero;
    }

    private bool SimpleValidateAngle()
    {
        Quaternion oldRotation = transform.rotation;
        transform.LookAt(FindTargetPosition());

        bool invalid = false;

        // check up rotation
        if(transform.eulerAngles.x > 270)
            if ((360f - transform.eulerAngles.x) > maxLookUpAngle)
                invalid = true;

        // check down rotation
        if(transform.eulerAngles.x < 90)
            if (transform.eulerAngles.x > maxLookDownAngle)
                invalid = true;

        transform.rotation = oldRotation;

        return !invalid;
    }

    // to fancy, I'm doing it the stupid way
    private bool ValidateAngle(Vector3 targetPos)
    {
        Vector3 targetDir = targetPos - transform.position;

        print("Angle: " + Vector3.Angle(targetDir, transform.up));

        if (Vector3.Angle(targetDir, transform.forward) < maxLookUpAngle)
            return true;

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
