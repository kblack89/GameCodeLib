using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;

    [SerializeField] LayerMask groundLayer, playerLayer;
    Animator animator;

    BoxCollider boxCollider;

    //patrol
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;

    //state change

    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!isAttacking)
        {
            if (!playerInSight && !playerInAttackRange) Patrol();
            if (playerInSight && !playerInAttackRange) Chase();
        }
        if (playerInSight && playerInAttackRange) Attack();
    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        if (!isAttacking)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
            {
                animator.SetTrigger("Attack");
                isAttacking = true;
            }
        }
        agent.SetDestination(transform.position); // Stop moving during attack
    }

    void Patrol()
    {
        if (!walkpointSet) SearchForDest();
        if (walkpointSet) agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10) walkpointSet = false;
    }

    void SearchForDest()
    {
        float z = UnityEngine.Random.Range(-range, range);
        float x = UnityEngine.Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    void EnableAttack()
    {
        boxCollider.enabled = true;
    }

    void DisableAttack()
    {
        boxCollider.enabled = false;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<FPSController>();

        if (player != null)
        {
            Debug.Log("Hit Player");
        }
    }
}
