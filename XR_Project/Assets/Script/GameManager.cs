using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public GameObject ceiling;

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
}
