using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panelli")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("Scene")]
    [SerializeField] private string firstGameSceneName = "Scene#1";
    [SerializeField] private int firsGameSceneBuildIndex = 1;

    private void Start()
    {
        Time.timeScale = 1f;
        OpenMainPanel();
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrWhiteSpace(firstGameSceneName))
        {
            SceneManager.LoadScene(firstGameSceneName);
            return;
        }
        LoadSceneByBuildIndex(firsGameSceneBuildIndex);
    }
    

    public void OpenMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);

    }

    public void OpenSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void OpenLevelSelect()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
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
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Build Index scena non valido: " + buildIndex);
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
