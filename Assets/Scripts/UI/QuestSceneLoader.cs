using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestSceneLoader : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestManager questManager;

    [Header("Cambio scena")]
    [SerializeField] private bool caricaAutomaticamente = true;
    [SerializeField] private float attesaPrimaDelCambio = 5f;
    [SerializeField] private int buildIndexScenaSuccessiva = 2;
    [SerializeField] private string nomeScenaSuccesiva = "Scene#2";

    [Header("UI Opzionale")]
    [Tooltip("Pannello da mostrare a quest completata per far avanzare eventualmente alla prossima scena.")]
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private string nextLevelPanelName = "NextLevelPanel";

    private bool eventoCollegato = false;
    private bool cambioInCorso = false;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        ResolveReferences();

        if (nextLevelPanel != null)
        {
            nextLevelPanel.SetActive(false);
        }

        CollegaQuestManager();
    }

    private void OnEnable()
    {
        ResolveReferences();
        CollegaQuestManager();
    }

    private void OnDisable()
    {
        ScollegaQuestManager();
    }

    private void ResolveReferences()
    {
        if (questManager == null || !SceneReferenceFinder.IsSceneInstance(questManager))
        {
            questManager = SceneReferenceFinder.FindComponentInActiveScene<QuestManager>();
        }

        nextLevelPanel = SceneReferenceFinder.ResolveSceneObject(nextLevelPanel, null, nextLevelPanelName);
    }

    private void CollegaQuestManager()
    {
        if (eventoCollegato) return;

        if (questManager == null)
        {
            ResolveReferences();
        }

        if (questManager == null) return;

        questManager.OnQuestCompletata += GestisciQuestCompletata;
        eventoCollegato = true;
    }

    private void ScollegaQuestManager()
    {
        if (!eventoCollegato) return;
        if (questManager == null) return;

        questManager.OnQuestCompletata -= GestisciQuestCompletata;
        eventoCollegato = false;
    }

    private void GestisciQuestCompletata()
    {
        if (cambioInCorso) return;

        ResolveReferences();

        if (nextLevelPanel != null)
        {
            nextLevelPanel.SetActive(true);
        }

        if (caricaAutomaticamente)
        {
            StartCoroutine(CaricaDopoAttesa());
        }
    }

    private IEnumerator CaricaDopoAttesa()
    {
        cambioInCorso = true;
        yield return new WaitForSecondsRealtime(attesaPrimaDelCambio);
        CaricaScenaSuccessiva();
    }

    public void CaricaScenaSuccessiva()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(nomeScenaSuccesiva))
        {
            SceneManager.LoadScene(nomeScenaSuccesiva);
            return;
        }

        if (buildIndexScenaSuccessiva >= 0 && buildIndexScenaSuccessiva < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(buildIndexScenaSuccessiva);
            return;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex >= 0 && nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
            return;
        }

        Debug.LogWarning("Nessuna scena successiva valida. Controlla nome scena e Build Settings.");
    }
}
