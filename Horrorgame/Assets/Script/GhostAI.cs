using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class GhostAI : MonoBehaviour
{
    public enum GhostState
    {
        Idle,         // ยืนนิ่งเฉย
        Patrol,       // เดินตรวจตามจุด
        Suspicious,   // เห็นผู้เล่นจากระยะไกล → หันไปดู
        Chase,        // ไล่ผู้เล่น
        Search,       // เดินไปหาจุดสุดท้ายที่เห็นผู้เล่น
        Return        // กลับไปเดิน patrol ตามเดิม
    }

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
    private bool isWaitingAtCabinet = false;

    [Header("Ghost Movement Speeds")]
    public float walkSpeed = 1f;
    public float runSpeed = 1.5f;
    public float patrolSpeed = 1f;

    private DoorController playerTargetDoor = null;

    private bool justVisitedCabinet = false;

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

        ChangeState(GhostState.Patrol);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // 🎯 ให้ update เอฟเฟกต์ทุกเฟรม ไม่ว่า Player ซ่อนหรือไม่
        UpdateScareEffects(distance);

        if (IsPlayerHidden())
        {
            if (!isWaitingAtCabinet &&
                (currentState == GhostState.Chase || currentState == GhostState.Suspicious || currentState == GhostState.Search))
            {
                StartCoroutine(WaitBeforeReturn());
            }

            // ✅ ยอมให้เดิน patrol ได้ถ้าอยู่ในสถานะ Idle/Patrol
            if (currentState == GhostState.Idle || currentState == GhostState.Patrol)
            {
                Patrol();
            }

            return;
        }

        switch (currentState)
        {
            case GhostState.Idle:
            case GhostState.Patrol:
                if (CanSeePlayer())
                    ChangeState(GhostState.Chase);
                else
                    Patrol();
                break;

            case GhostState.Suspicious:
                if (CanSeePlayer())
                    ChangeState(GhostState.Chase);
                else
                    LookAtPlayer();
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

        TryClosePlayerDoor();
    }

    private IEnumerator WaitBeforeReturn()
    {
        isWaitingAtCabinet = true;

        agent.isStopped = true;
        animator.Play("LookAround");
        Debug.Log("👻 Ghost is waiting outside cabinet...");

        float waitTime = 0f;
        while (waitTime < 2f)
        {
            // ❗ ถ้าผู้เล่นออกจากตู้ระหว่างรอ
            if (!IsPlayerHidden())
            {
                // เช็กว่ามองเห็นจริง
                Vector3 toPlayer = (player.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, toPlayer);

                if (angle < 60f) // มุมมอง 120° (60° ซ้ายขวา)
                {
                    Debug.Log("👀 Player came out of cabinet! Ghost starts chasing!");
                    agent.isStopped = false;
                    ChangeState(GhostState.Chase);
                    isWaitingAtCabinet = false;
                    yield break; // ออกจาก Coroutine ทันที
                }
            }

            waitTime += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        agent.ResetPath();
        agent.SetDestination(patrolPoints[patrolIndex].position);
        ChangeState(GhostState.Patrol);
        isWaitingAtCabinet = false;
    }

    void UpdateScareEffects(float distance)
    {
        if (distance <= scareDistance)
        {
            float power = Mathf.Clamp01(1 - (distance / scareDistance));
            proximityShake?.SetShakePower(power);
            redLight?.SetLightIntensity(power);

            // ✅ เพิ่มการสั่นตาม power
            if (Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(power * 0.5f, power); // low freq, high freq
        }
        else
        {
            proximityShake?.SetShakePower(0);
            redLight?.SetLightIntensity(0);

            // ✅ หยุดการสั่นเมื่ออยู่ไกล
            if (Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(0f, 0f);
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
                agent.speed = patrolSpeed;
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[patrolIndex].position);
                animator.Play("Walk");
                break;

            case GhostState.Suspicious:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                animator.Play("LookAround");
                break;

            case GhostState.Chase:
                agent.speed = runSpeed;
                agent.isStopped = false;
                animator.Play("Run");
                break;

            case GhostState.Search:
                agent.speed = walkSpeed;
                searchTimer = 0f;
                agent.SetDestination(lastKnownPlayerPosition);
                animator.Play("LookAround");
                break;

            case GhostState.Return:
                agent.speed = patrolSpeed;
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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 👇 เพิ่มเช็คการชนก่อนเคลื่อนที่
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, 0.6f))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // ✨ แก้ตรงนี้
                Vector3 avoidanceDir = Quaternion.Euler(0, 60f, 0) * transform.forward;
                Vector3 newTarget = transform.position + avoidanceDir;
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(newTarget, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetPath(path);
                    agent.isStopped = false;
                    animator.Play("Run");
                    Debug.Log("↩ ผีเลี้ยวหลบกำแพง (Path ok)");
                }
                else
                {
                    agent.isStopped = true;
                    animator.Play("Idle");
                    Debug.Log("🛑 ผีเลี้ยวไม่ได้ (ไม่มี NavMesh Path)");
                }

                return;
            }
        }

        if (distanceToPlayer > 1.2f)
        {
            Vector3 destination = player.position;

            // ป้องกันวาร์ปทะลุผนัง
            if (NavMesh.SamplePosition(player.position, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
            {
                destination = navHit.position;
            }

            agent.isStopped = false;
            agent.SetDestination(destination);

            if (playerTargetDoor != null)
            {
                float distanceToDoor = Vector3.Distance(transform.position, playerTargetDoor.transform.position);
                if (!playerTargetDoor.IsOpen() && playerTargetDoor.IsUnlocked() && distanceToDoor < 1.5f)
                {
                    playerTargetDoor.OpenByGhost(); // 💥 force เปิดประตู + ปิด blocker
                    Debug.Log("👻 ผีเปิดประตูเป้าหมาย");
                }
            }

            animator.Play("Run");
        }
        else
        {
            if (!agent.isStopped)
            {
                agent.ResetPath();
                agent.isStopped = true;
            }

            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0;
            if (lookDir.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 2f);
            }

            animator.Play("Idle");
        }

        lastKnownPlayerPosition = player.position;

        if (distanceToPlayer > suspiciousDistance)
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
        TryClosePlayerDoor();

        if (justVisitedCabinet && playerTargetDoor != null)
        {
            playerTargetDoor.CloseByGhost();
            justVisitedCabinet = false;
            Debug.Log("🚪 ผีปิดประตูหลังจากออกจากหน้าตู้");
        }
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            CloseNearbyDoors();

            // ✅ รีเซ็ต path ก่อนเปลี่ยนปลายทางใหม่
            agent.ResetPath();
            agent.SetDestination(patrolPoints[patrolIndex].position);
            ChangeState(GhostState.Patrol);
        }

        if (playerTargetDoor != null && playerTargetDoor.IsOpen())
        {
            float dist = Vector3.Distance(transform.position, playerTargetDoor.transform.position);
            if (dist > 2.5f) // ออกจากห้องแล้ว
            {
                playerTargetDoor.CloseByGhost();
                playerTargetDoor = null;
            }
        }
    }

    bool CanSeePlayer()
    {
        if (IsPlayerHidden()) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > 60f) return false;

        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, directionToPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, suspiciousDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void CloseNearbyDoors()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (var hit in hits)
        {
            DoorController door = hit.GetComponent<DoorController>();
            if (door != null && door.IsOpen())
            {
                door.CloseByGhost();
            }
        }
    }

    public void SetPlayerTargetDoor(DoorController door)
    {
        playerTargetDoor = door;
    }

    public bool IsChasing()
    {
        return currentState == GhostState.Chase;
    }

    public void GoToCabinet(Vector3 targetPosition, DoorController door = null)
    {
        float dist = Vector3.Distance(transform.position, targetPosition);

        // ✅ สนใจเฉพาะเห็นผู้เล่นก่อนหน้านี้
        if (currentState != GhostState.Chase && currentState != GhostState.Suspicious)
        {
            Debug.Log("👻 ผีไม่สนตู้เพราะอยู่ไกลและไม่ไล่");
            return;
        }

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.SetDestination(targetPosition);
        animator.Play("Walk");

        justVisitedCabinet = true;

        if (door != null)
            playerTargetDoor = door;

        Debug.Log("👻 ผีกำลังเดินมาหน้าตู้...");
    }

    private void TryClosePlayerDoor()
    {
        if (justVisitedCabinet && playerTargetDoor != null)
        {
            float dist = Vector3.Distance(transform.position, playerTargetDoor.transform.position);

            if (dist > 2.5f) // ✅ เดินห่างออกมาพอแล้ว
            {
                playerTargetDoor.CloseByGhost();
                playerTargetDoor = null;
                justVisitedCabinet = false;

                Debug.Log("🚪 ผีปิดประตูหลังจากออกห่างตู้แล้ว");
            }
        }
    }

    public void CancelCabinetMemory()
    {
        justVisitedCabinet = false;
    }
}

