using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Root menu nella scena")]
    [SerializeField] private Transform menuRoot;
    [SerializeField] private string menuRootName = "Menus";

    [Header("Nomi pannelli")]
    [SerializeField] private string mainPanelName = "MainPanel";
    [SerializeField] private string settingsPanelName = "SettingsPanel";
    [SerializeField] private string levelSelectPanelName = "LevelSelectPanel";

    [Header("Pannelli")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("Scene")]
    [SerializeField] private string firstGameSceneName = "Scene#1";
    [SerializeField] private int firstGameSceneBuildIndex = 1;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        OpenMainPanel();
    }

    private void ResolveReferences()
    {
        menuRoot = SceneReferenceFinder.ResolveSceneTransform(menuRoot, menuRootName);

        mainPanel = SceneReferenceFinder.ResolveSceneObject(mainPanel, menuRoot, mainPanelName);
        settingsPanel = SceneReferenceFinder.ResolveSceneObject(settingsPanel, menuRoot, settingsPanelName);
        levelSelectPanel = SceneReferenceFinder.ResolveSceneObject(levelSelectPanel, menuRoot, levelSelectPanelName);
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(firstGameSceneName))
        {
            SceneManager.LoadScene(firstGameSceneName);
            return;
        }

        LoadSceneByBuildIndex(firstGameSceneBuildIndex);
    }

    public void OpenMainPanel()
    {
        ResolveReferences();

        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        ResolveReferences();

        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void OpenLevelSelect()
    {
        ResolveReferences();

        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Nome scena non valido.");
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
