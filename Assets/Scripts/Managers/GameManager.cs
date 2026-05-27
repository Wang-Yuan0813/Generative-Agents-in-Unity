
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // DontDestroyOnLoad(gameObject);

    }

    private void OnEnable()
    {
        Debug.Log("[GameManager] Enabled");
    }

    private void OnDisable()
    {

        Debug.Log("[GameManager] Disabled");
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        Debug.Log("[GameManager] Destroyed");
    }

    public void Initialize()
    {
        if (IsPlaying) return; 
        StartGame();
    }

    private void StartGame()
    {
        IsPlaying = true;
        Resume();
        Debug.Log("[GameManager] Game started");
    }

    public void GameOver()
    {
        IsPlaying = false;
        Pause();
        Debug.Log("[GameManager] Game over");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        Debug.Log("[GameManager] Paused");
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Debug.Log("[GameManager] Resumed");
    }

}
