using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChasePlayer : MonoBehaviour
{
    private enum AIState { Patrol, Chase, Locked }

    [Header("Tracking & Detection")]
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private float detectionRange = 10.0f; // 플레이어 감지 거리
    [SerializeField] private float stopDistance = 1.5f;     // 공격/접근 멈춤 거리

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 15.0f;    // 랜덤 순찰 반경
    [SerializeField] private float patrolWaitTime = 2.0f;    // 목적지 도착 후 대기 시간

    [Header("System Settings")]
    [SerializeField] private float updateInterval = 0.2f;   // AI 판단 및 경로 갱신 주기

    public bool isLocked = false; // AI가 잠금 상태인지 여부

    private NavMeshAgent agent;
    private AIState currentState = AIState.Patrol;
    private Coroutine aiCoroutine;
    private Vector3 initialPosition;
    private Animator animator;
    private float animSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animSpeed = animator.speed;
    }

    private void Start()
    {
        agent.stoppingDistance = stopDistance;
        initialPosition = transform.position; // 시작 위치를 기준으로 순찰 영역 설정

        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                targetPlayer = playerObj.transform;
            }
        }

        animator.speed = 0f; // AI 시작 시 애니메이션 속도를 0으로 설정

        //StartAI();
    }

    public void StartAI()
    {
        Debug.Log($"{gameObject.name} AI 시작");

        if (aiCoroutine == null)
        {
            aiCoroutine = StartCoroutine(CoAILoop());
        }
    }

    public void StopAI()
    {
        if (aiCoroutine != null)
        {
            StopCoroutine(aiCoroutine);
            aiCoroutine = null;
        }

        if (agent.isOnNavMesh && agent.hasPath)
        {
            agent.ResetPath();
        }
    }

    private IEnumerator CoAILoop()
    {

        WaitForSeconds wait = new WaitForSeconds(updateInterval);

        while (true)
        {
            if (isLocked)
            {
                animator.speed = 0f; // AI가 잠금 상태이면 애니메이션 속도를 0으로 설정
                //StopAI(); // AI가 잠금 상태이면 AI 동작을 중지
                //yield break; // AI가 잠금 상태이면 코루틴 종료
                currentState = AIState.Locked; // 상태를 Locked로 설정

                agent.ResetPath();
            }

            else
            {
                if (animator.speed != animSpeed)
                {
                    animator.speed = animSpeed; // AI가 잠금 상태가 아니면 애니메이션 속도를 원래대로 설정
                }

                if (agent.isOnNavMesh && agent.enabled)
                {
                    // 1. 플레이어와의 거리 체크
                    float distanceToPlayer = float.MaxValue;

                    if (targetPlayer != null)
                    {
                        distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
                    }



                    // 2. 상태 전환 판단
                    if (distanceToPlayer <= detectionRange)
                    {
                        currentState = AIState.Chase;
                    }

                    else
                    {
                        currentState = AIState.Patrol;
                    }

                    // 3. 상태별 행동 실행
                    switch (currentState)
                    {
                        case AIState.Patrol:
                            HandlePatrol();
                            break;

                        case AIState.Chase:
                            HandleChase();
                            break;

                        case AIState.Locked:
                            agent.ResetPath(); // 잠금 상태에서는 이동을 멈춤
                            break;
                    }
                }
            }

            yield return wait;
        }
    }

    private void HandleChase()
    {
        // 추적 시에는 stoppingDistance 적용
        agent.stoppingDistance = stopDistance;
        agent.SetDestination(targetPlayer.position);
    }

    private void HandlePatrol()
    {
        // 순찰 시에는 목적지 지점까지 바짝 가도록 설정
        agent.stoppingDistance = 0.5f;

        // 이미 이동 중이거나 경로를 계산 중이면 대기
        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            return;
        }

        // 목적지에 도착했으면 랜덤 위치를 다시 탐색
        Vector3 randomPoint;
        if (GetRandomNavMeshPoint(initialPosition, patrolRadius, out randomPoint))
        {
            agent.SetDestination(randomPoint);
        }
    }

    // NavMesh 위의 유효한 랜덤 좌표 구하기
    private bool GetRandomNavMeshPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range + center;
        NavMeshHit hit;

        // 지정한 반경 내에서 가장 가까운 NavMesh 샘플링
        if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void OnDisable()
    {
        StopAI();
    }

    // 에디터 씬 뷰에서 감지/순찰 범위 시각화
    private void OnDrawGizmosSelected()
    {
        // 감지 범위 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 순찰 범위 (녹색 - 시작 위치 기준)
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? initialPosition : transform.position;
        Gizmos.DrawWireSphere(center, patrolRadius);
    }
}

