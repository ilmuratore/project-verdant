using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Transform menuRoot;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject levelSelectPanel;
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
        menuRoot = menuRoot != null ? menuRoot : SceneReferenceFinder.ResolveSceneTransform(null, "Menus");
        Transform root = menuRoot != null ? menuRoot : transform;
        mainPanel = SceneReferenceFinder.ResolveSceneObject(mainPanel, root, "MainPanel");
        settingsPanel = SceneReferenceFinder.ResolveSceneObject(settingsPanel, root, "SettingsPanel");
        levelSelectPanel = SceneReferenceFinder.ResolveSceneObject(levelSelectPanel, root, "LevelSelectPanel");
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
        SetActive(mainPanel, true);
        SetActive(settingsPanel, false);
        SetActive(levelSelectPanel, false);
    }

    public void OpenSettings()
    {
        ResolveReferences();
        SetActive(mainPanel, false);
        SetActive(settingsPanel, true);
        SetActive(levelSelectPanel, false);
    }

    public void OpenLevelSelect()
    {
        ResolveReferences();
        SetActive(mainPanel, false);
        SetActive(settingsPanel, false);
        SetActive(levelSelectPanel, true);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByBuildIndex(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings) return;
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

    private void SetActive(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }
}
