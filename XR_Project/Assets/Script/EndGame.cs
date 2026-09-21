using UnityEngine;

public class EndGame : MonoBehaviour
{
    public void RestartGame()
    {
        GameManager.Instance.StopAllEntityAI(); // 모든 AI를 중지

        // 현재 씬을 다시 로드하여 게임을 재시작
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
