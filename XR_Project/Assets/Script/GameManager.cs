using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ChasePlayer[] entityAIs; // 씬에 존재하는 모든 ChasePlayer AI를 배열로 관리

    public GameObject ceiling;

    private bool started = false;

    public GameObject JumpScare;

    private GameManager()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Awake()
    {
        ceiling.SetActive(true);
    }

    public void StartAllEntityAI()
    {
        if (started) return; // 이미 시작된 경우 중복 실행 방지

        started = true;

        foreach (var ai in entityAIs)
        {
            ai.StartAI();
        }
    }

    public void StopAllEntityAI()
    {
        if (!started) return; // 이미 중지된 경우 중복 실행 방지

        started = false;

        foreach (var ai in entityAIs)
        {
            ai.StopAI();
        }
    }

    public void OnPlayerCaught()
    {
        StopAllEntityAI();

        JumpScare.SetActive(true);

        Debug.Log("플레이어가 잡혔습니다!");

        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine()
    {
        yield return new WaitForSeconds(2f); // 2초 대기

        // 게임 재시작 로직 추가 가능
        SceneManager.LoadScene("MazeScene"); // 현재 씬을 다시 로드하여 게임 재시작
    }
}
