
using UnityEngine;
using UnityEngine.AI;
using RcLab;

public class EnemyAiTutorial : MonoBehaviour
{
    [Header("Settings")]
    public MovementMode movementMode;
    public float agentOffset;
    public enum MovementMode
    {
        Agent,
        Rigidbody
    }

    public NavMeshAgent agent;
    private Rigidbody rb;
    public float rigidbodySpeed;
    private bool rigidbodyMoving;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float walkPointReachThreshold = 1f;
    public float minTimeBetweenPatroling;
    public float maxTimeBetweenPatroling;
    private float idleTimer;

    //Attacking
    public RangedWeapon weapon;
    public Magazine magazine;
    public Transform hand;
    public Combat combat;
    public bool attacking;

    //States
    public float sightRange, attackRange;
    private float baseSightRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        baseSightRange = sightRange;

        player = GameObject.Find("PlayerObj").transform;
        combat = GetComponent<Combat>();

        if(GetComponent<NavMeshAgent>() == null)
            agent = GetComponent<NavMeshAgent>();

        if (GetComponent<Rigidbody>() == null)
            rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        if (idleTimer > 0) idleTimer -= Time.deltaTime;

        if (rigidbodyMoving) RigidbodyMovement();
    }

    private void Patroling()
    {
        if (idleTimer > 0) return;

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            MoveToPoint(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < walkPointReachThreshold)
        {
            idleTimer = Random.Range(minTimeBetweenPatroling, maxTimeBetweenPatroling);
            walkPointSet = false;
        }

        // stop attacking
        if (attacking)
        {
            //combat.StopRangedWeapon(weapon);
            attacking = false;
        }
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y - agentOffset, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        MoveToPoint(player.position);

        hand.LookAt(player);

        // start attacking
        if (!attacking && Vector3.Distance(transform.position, player.position) < baseSightRange)
        {
            combat.PlayRangedWeapon(weapon);
            attacking = true;
        }
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        MoveToPoint(transform.position);

        transform.LookAt(player);
        hand.LookAt(player);

        // start attacking
        if (!attacking)
        {
            combat.PlayRangedWeapon(weapon);
            attacking = true;
        }
    }

    private void MoveToPoint(Vector3 worldPoint)
    {
        if(movementMode == MovementMode.Agent)
        {
            agent.SetDestination(worldPoint);
        }
        if(movementMode == MovementMode.Rigidbody)
        {
            rigidbodyMoving = true;
        }
    }

    private void RigidbodyMovement()
    {
        Vector3 distance = walkPoint - transform.position;

        rb.velocity = distance.normalized * rigidbodySpeed;

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < walkPointReachThreshold)
        {
            rigidbodyMoving = true;
        }
    }

    #region Setters

    public void ResetSightRange()
    {

    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
