using System;
using UnityEngine;

public enum QuestState
{
    NonIniziata,
    InCorso,
    Completata,
    Fallita
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public QuestData questAttiva;
    public EnemySpawner spawner;

    [SerializeField] private QuestState stato = QuestState.NonIniziata;
    [SerializeField] private int nemiciUccisi;
    [SerializeField] private int monaciMorti;

    private MonkHealth[] monaciDaProteggere;

    public event Action OnQuestAggiornata;
    public event Action OnQuestCompletata;
    public event Action OnQuestFallita;

    public QuestState Stato => stato;
    public int NemiciUccisi => nemiciUccisi;
    public int NemiciTotali => questAttiva != null ? questAttiva.TotaleNemici : 0;
    public int MonaciTotali => monaciDaProteggere != null ? monaciDaProteggere.Length : 0;
    public int MonaciMorti => monaciMorti;
    public int MonaciSalvi => Mathf.Max(0, MonaciTotali - monaciMorti);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResolveReferences();
    }

    private void OnEnable()
    {
        EnemyStats.OnAnyEnemyDied += HandleEnemyDied;
        MonkHealth.OnAnyMonkDied += HandleMonkDied;
    }

    private void OnDisable()
    {
        EnemyStats.OnAnyEnemyDied -= HandleEnemyDied;
        MonkHealth.OnAnyMonkDied -= HandleMonkDied;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void IniziaQuest()
    {
        if (stato != QuestState.NonIniziata) return;
        if (questAttiva == null) return;

        ResolveReferences();

        monaciDaProteggere = FindObjectsByType<MonkHealth>(FindObjectsSortMode.None);
        nemiciUccisi = 0;
        monaciMorti = 0;
        stato = QuestState.InCorso;

        spawner?.AvviaQuest(questAttiva);
        NotifyUpdated();
    }

    public void ResetQuestRuntime()
    {
        nemiciUccisi = 0;
        monaciMorti = 0;
        stato = QuestState.NonIniziata;
        spawner?.FermaSpawner(true);
        NotifyUpdated();
    }

    private void ResolveReferences()
    {
        if (spawner == null || !SceneReferenceFinder.IsSceneInstance(spawner))
        {
            spawner = FindFirstObjectByType<EnemySpawner>();
        }
    }

    private void HandleEnemyDied(EnemyStats enemy)
    {
        if (stato != QuestState.InCorso) return;

        nemiciUccisi++;
        NotifyUpdated();

        if (nemiciUccisi >= NemiciTotali && monaciMorti == 0)
        {
            CompleteQuest();
        }
    }

    private void HandleMonkDied(MonkHealth monk)
    {
        if (stato != QuestState.InCorso) return;

        monaciMorti++;
        NotifyUpdated();
        FailQuest();
    }

    private void CompleteQuest()
    {
        if (stato != QuestState.InCorso) return;

        stato = QuestState.Completata;
        spawner?.FermaSpawner(false);

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null && questAttiva != null)
        {
            playerStats.AddXp(questAttiva.xpRicompensa);
        }

        GameManager.Instance?.SetResult();
        OnQuestCompletata?.Invoke();
        NotifyUpdated();
    }

    private void FailQuest()
    {
        if (stato != QuestState.InCorso) return;

        stato = QuestState.Fallita;
        spawner?.FermaSpawner(true);
        GameManager.Instance?.SetResult();
        OnQuestFallita?.Invoke();
        NotifyUpdated();
    }

    private void NotifyUpdated()
    {
        OnQuestAggiornata?.Invoke();
    }
}
