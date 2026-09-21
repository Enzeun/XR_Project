using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChasePlayer : MonoBehaviour
{
    [Header("Tracking Target")]
    [SerializeField] private Transform targetPlayer; // 추적할 플레이어 Transform

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.2f; // NavMesh 경로 갱신 주기 (초)
    [SerializeField] private float stopDistance = 1.5f;   // 플레이어에게 접근했을 때 멈추는 거리

    private NavMeshAgent agent;
    private Coroutine trackingCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // 멈춤 거리 설정
        agent.stoppingDistance = stopDistance;

        // 플레이어 트랜스폼을 인스펙터에서 안 넣었을 경우 태그로 자동 탐색
        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                targetPlayer = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Target Player가 설정되지 않았습니다.");
            }
        }

        // 추적 코루틴 시작
        StartTracking();
    }

    public void StartTracking()
    {
        if (trackingCoroutine == null && targetPlayer != null)
        {
            trackingCoroutine = StartCoroutine(CoTrackPlayer());
        }
    }

    public void StopTracking()
    {
        if (trackingCoroutine != null)
        {
            StopCoroutine(trackingCoroutine);
            trackingCoroutine = null;
        }

        if (agent.isOnNavMesh && agent.hasPath)
        {
            agent.ResetPath();
        }
    }

    private IEnumerator CoTrackPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(updateInterval);

        while (targetPlayer != null)
        {
            // NavMesh 위에 정상적으로 배치되어 있고, 활성화된 상태인지 확인
            if (agent.isOnNavMesh && agent.enabled)
            {
                agent.SetDestination(targetPlayer.position);
            }

            yield return wait;
        }
    }

    private void OnDisable()
    {
        StopTracking();
    }
}

