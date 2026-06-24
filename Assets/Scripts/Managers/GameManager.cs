using UnityEngine;

public enum GameState
{
    Gameplay,
    Dialogue,
    Paused,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState state = GameState.Gameplay;

    public GameState State => state;
    public bool ControlsBlocked => state != GameState.Gameplay;
    public bool IsPaused => state == GameState.Paused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetGameplay()
    {
        state = GameState.Gameplay;
        Time.timeScale = 1f;
    }

    public void SetDialogue()
    {
        state = GameState.Dialogue;
        Time.timeScale = 1f;
    }

    public void SetResult()
    {
        state = GameState.Result;
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        state = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        state = GameState.Gameplay;
        Time.timeScale = 1f;
    }
}
