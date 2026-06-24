using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Quest attiva")]
    public QuestData questAttiva;

    [Header("Riferimenti")]
    public EnemySpawner spawner;

    [Header("Avvio quest")]
    public bool avviaConTasto = true;
    public Key tastoAvvioQuest = Key.T;

    [Header("Runtime")]
    [SerializeField] private QuestState stato = QuestState.NonIniziata;
    [SerializeField] private int nemiciUccisi = 0;
    [SerializeField] private int monaciMorti = 0;

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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!avviaConTasto) return;
        if (Keyboard.current == null) return;

        var keyControl = Keyboard.current[tastoAvvioQuest];

        if (keyControl != null && keyControl.wasPressedThisFrame)
        {
            IniziaQuest();
        }
    }

    private void OnEnable()
    {
        EnemyStats.OnAnyEnemyDied += GestisciNemicoUcciso;
        MonkHealth.OnAnyMonkDied += GestisciMonacoMorto;
    }

    private void OnDisable()
    {
        EnemyStats.OnAnyEnemyDied -= GestisciNemicoUcciso;
        MonkHealth.OnAnyMonkDied -= GestisciMonacoMorto;
    }

    private void ResolveReferences()
    {
        if (spawner == null || !SceneReferenceFinder.IsSceneInstance(spawner))
        {
            spawner = SceneReferenceFinder.FindComponentInActiveScene<EnemySpawner>();
        }
    }

    public void IniziaQuest()
    {
        if (stato != QuestState.NonIniziata) return;

        ResolveReferences();

        if (questAttiva == null)
        {
            Debug.LogWarning("Nessun QuestData assegnata.");
            return;
        }

        monaciDaProteggere = FindObjectsByType<MonkHealth>(FindObjectsSortMode.None);

        stato = QuestState.InCorso;
        nemiciUccisi = 0;
        monaciMorti = 0;

        if (spawner != null)
        {
            spawner.AvviaQuest(questAttiva);
        }
        else
        {
            Debug.LogWarning("EnemySpawner non assegnato.");
        }

        NotificaAggiornamento();
    }

    private void GestisciNemicoUcciso(EnemyStats nemico)
    {
        if (stato != QuestState.InCorso) return;

        nemiciUccisi++;
        NotificaAggiornamento();

        if (nemiciUccisi >= NemiciTotali && monaciMorti == 0)
        {
            CompletaQuest();
        }
    }

    private void GestisciMonacoMorto(MonkHealth monaco)
    {
        if (stato != QuestState.InCorso) return;

        monaciMorti++;
        NotificaAggiornamento();
        FallisciQuest();
    }

    private void CompletaQuest()
    {
        if (stato != QuestState.InCorso) return;

        stato = QuestState.Completata;

        if (spawner != null)
        {
            spawner.FermaSpawner(false);
        }

        PlayerStats ps = SceneReferenceFinder.FindComponentInActiveScene<PlayerStats>();
        if (ps != null && questAttiva != null)
        {
            ps.AddXp(questAttiva.xpRicompensa);
        }

        OnQuestCompletata?.Invoke();
        NotificaAggiornamento();
    }

    private void FallisciQuest()
    {
        if (stato != QuestState.InCorso) return;

        stato = QuestState.Fallita;

        if (spawner != null)
        {
            spawner.FermaSpawner(true);
        }

        OnQuestFallita?.Invoke();
        NotificaAggiornamento();
    }

    private void NotificaAggiornamento()
    {
        OnQuestAggiornata?.Invoke();
    }
}
