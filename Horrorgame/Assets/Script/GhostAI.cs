using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    public enum GhostState { Idle, Patrol, Suspicious, Chase, Search, Return }
    public GhostState currentState = GhostState.Idle;

    public Transform[] patrolPoints;
    public float chaseDistance = 4f;
    public float suspiciousDistance = 8f;
    public float searchTime = 5f;

    private int patrolIndex = 0;
    private float searchTimer = 0f;

    public LanternShakeEffect proximityShake;
    public float scareDistance = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private Vector3 lastKnownPlayerPosition;
    private bool playerInSight = false;

    public RedLightPulse redLight;

    public static GhostAI Instance;
    private bool playerHidden = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetPlayerHidden(bool state) => playerHidden = state;
    public bool IsPlayerHidden() => playerHidden;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        ChangeState(GhostState.Idle);
    }

    void Update()
    {
        if (IsPlayerHidden())
        {
            // ปิดกล้องสั่น + แสง เมื่อผู้เล่นซ่อน
            proximityShake?.SetShakePower(0);
            redLight?.SetLightIntensity(0);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= scareDistance)
        {
            float power = Mathf.Clamp01(1 - (distance / scareDistance));
            proximityShake?.SetShakePower(power);  // กล้องสั่น
            redLight?.SetLightIntensity(power);       // แสงกระพริบแรงขึ้น
        }
        else
        {
            proximityShake?.SetShakePower(0);
            redLight?.SetLightIntensity(0);
        }

        switch (currentState)
        {
            case GhostState.Idle:
                if (distance <= suspiciousDistance) ChangeState(GhostState.Suspicious);
                break;

            case GhostState.Patrol:
                if (distance <= chaseDistance) ChangeState(GhostState.Chase);
                else Patrol();
                break;

            case GhostState.Suspicious:
                if (distance <= chaseDistance) ChangeState(GhostState.Chase);
                else LookAtPlayer();
                break;

            case GhostState.Chase:
                ChasePlayer();
                break;

            case GhostState.Search:
                Search();
                break;

            case GhostState.Return:
                ReturnToPatrol();
                break;
        }
    }

    void ChangeState(GhostState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case GhostState.Idle:
                agent.isStopped = true;
                animator.Play("Idle");
                break;

            case GhostState.Patrol:
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[patrolIndex].position);
                animator.Play("Walk");
                break;

            case GhostState.Suspicious:
                agent.isStopped = true;
                animator.Play("LookAround");
                break;

            case GhostState.Chase:
                agent.isStopped = false;
                animator.Play("Run");
                break;

            case GhostState.Search:
                searchTimer = 0f;
                agent.SetDestination(lastKnownPlayerPosition);
                animator.Play("LookAround");
                break;

            case GhostState.Return:
                agent.SetDestination(patrolPoints[patrolIndex].position);
                animator.Play("Walk");
                break;
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    void LookAtPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        if (lookDir.magnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 2f);
        }
    }

    void ChasePlayer()
    {
        // agent.SetDestination(player.position);
        if ((agent.destination - player.position).sqrMagnitude > 0.2f)
            agent.SetDestination(player.position);

        lastKnownPlayerPosition = player.position;

        if (Vector3.Distance(transform.position, player.position) > suspiciousDistance)
        {
            ChangeState(GhostState.Search);
        }
    }

    void Search()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            searchTimer += Time.deltaTime;
            if (searchTimer >= searchTime)
            {
                ChangeState(GhostState.Return);
            }
        }
    }

    void ReturnToPatrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            ChangeState(GhostState.Patrol);
        }
    }
}

