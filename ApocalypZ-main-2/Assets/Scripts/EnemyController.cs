using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] public int maxLife = 100;
    [SerializeField] public int lifePoint = 100;
    [SerializeField] public float viewRadius;
    [Range(0, 360)]
    [SerializeField] public float viewAngle;
    private bool isDead;

    public GameObject playerRef;


    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstaclesMask;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] public bool canSeePlayer;



    private Animator animator;
    [SerializeField] NavMeshAgent navmeshAgent;
    [SerializeField] float patrollingRange;

    bool isWalkPoitSet;
    Vector3 walkPoint;
    private bool alreadyAttacking;
    [SerializeField] float timeBetweenAttacks;

    // UI
    [SerializeField] Image lifeBarImg;

    void Start()
    {
        playerRef = GameObject.Find("PlayerArmature (1)");
        rb = GetComponent<Rigidbody>();
        navmeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        StartCoroutine(FOVRoutine());
        UpdateHealthBar();
    }

    void Update()
    {
        //Patrolling();
        ChaseTarget();
        if (isWalkPoitSet)
        {
            if (canSeePlayer)
            {
                if (Vector3.Distance(transform.position, playerRef.transform.position) < 1.2)
                {
                    animator.SetFloat("walkSpeed", 0, 0.3f, Time.deltaTime);
                    AttackPlayer();
                    navmeshAgent.speed = 0;

                }
                else
                {
                    animator.SetFloat("walkSpeed", 2, 0.3f, Time.deltaTime);
                    navmeshAgent.speed = 2;
                }
            }
            else
            {
                animator.SetFloat("walkSpeed", 0.5f, 0.3f, Time.deltaTime);
                navmeshAgent.speed = 1;

            }
        }
        else
        {
            animator.SetFloat("walkSpeed", 0, 0.3f, Time.deltaTime);
            navmeshAgent.speed = 0;
        }
    }


    IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(.1f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0, 5f, 0), viewRadius, targetMask);

        if (colliders.Length != 0)
        {
            Transform target = colliders[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position + new Vector3(0, 5f, 0), directionToTarget, distanceToTarget, obstaclesMask))
                {
                    canSeePlayer = true;
                    isWalkPoitSet = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            isWalkPoitSet = false;
        }
    }


    void Patrolling()
    {
        if (canSeePlayer) return;
        if (!isWalkPoitSet)
        {
            StartCoroutine(SearchWalkPoint());
        }
        else
        {
            navmeshAgent.SetDestination(walkPoint);

            Vector3 distance2WalkPoint = transform.position - walkPoint;

            if (distance2WalkPoint.magnitude < .3f) isWalkPoitSet = false;
        }
    }

    void ChaseTarget()
    {
        if (canSeePlayer)
        {
            navmeshAgent.SetDestination(playerRef.transform.position);
        }
    }

    void AttackPlayer()
    {
        navmeshAgent.SetDestination(transform.position);
        transform.LookAt(playerRef.transform);

        if (!alreadyAttacking)
        {
            animator.SetBool("isAttacking", true);

            alreadyAttacking = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacking = false;
        animator.SetBool("isAttacking", false);
        GetComponentInChildren<EnemyWeapon>().ResetAffectedObjFromHit();
    }

    IEnumerator SearchWalkPoint()
    {
        animator.SetFloat("walkSpeed", 0, 0.3f, Time.deltaTime);
        yield return new WaitForSeconds(5);
        float randomZ = Random.Range(-patrollingRange, patrollingRange);
        float randomX = Random.Range(-patrollingRange, patrollingRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            isWalkPoitSet = true;
        }
    }

    public void EnemyLifeCheck()
    {
        if(isDead) return;
        UpdateHealthBar();
        if (lifePoint <= 0)
        {
            gameObject.GetComponent<Animator>().SetTrigger("trDeath");
            navmeshAgent.isStopped = true;
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            SoundManager.instance.PlayClipAtPoint("Zombie-Death", transform.position, 1.0f, 1.0f);
            StartCoroutine(AfterZombieDeath());
            isDead = true;
        }
        else
        {
            gameObject.GetComponent<Animator>().SetTrigger("trHitting");
            SoundManager.instance.PlayClipAtPoint("Enemy-Hit", transform.position, 1f, 2f);
        }
    }


    IEnumerator AfterZombieDeath()
    {
        yield return new WaitForSeconds(3);

        AmmoScript ammoObj = GetComponentInChildren<AmmoScript>();
        if (ammoObj)
        {
            ammoObj.transform.parent = null;
            ammoObj.GetComponentInChildren<Collider>().enabled = true;
            ammoObj.GetComponentInChildren<Rigidbody>().isKinematic = false;
            ammoObj.GetComponentInChildren<Rigidbody>().useGravity = true;
        }
        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        float fillAmount = (float)lifePoint / (float)maxLife;
        lifeBarImg.fillAmount = fillAmount;
    }
}
