using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Pannelli")]
    [SerializeField] private GameObject pauseMenuPanel;

    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private GameObject levelSelectPanel;

    [Header("Scene")]

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [SerializeField] private int mainMenubuildIndex = 0;

    [Header("Opzioni")]

    [SerializeField] private bool pauseWithEsc = true;

    private bool isPaused = false;

    private void Start()
    {
        CloseAllPanels();
        ResumeGame();
    }

    private void Update()
    {
        if (!pauseWithEsc) return;
        if (Keyboard.current == null) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        CloseAllPanels();
    }

    public void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void OpenLevelSelect()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }
        LoadSceneByBuildIndex(mainMenubuildIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {

            Debug.LogWarning("Nome scena non valido");
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }


    public void LoadSceneByBuildIndex(int buildIndex)
    {
        if(buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Build Index scena non valido: " + buildIndex);
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    private void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        if(nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Non esiste una scena successiva");
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void CloseAllPanels()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }
}
