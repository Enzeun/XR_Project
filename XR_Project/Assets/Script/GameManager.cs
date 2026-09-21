using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ChasePlayer[] entityAIs; // 씬에 존재하는 모든 ChasePlayer AI를 배열로 관리

    public GameObject ceiling;

    private bool started = false;

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
}
