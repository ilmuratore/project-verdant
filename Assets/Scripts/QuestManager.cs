using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.LightTransport;


public enum QuestState
{
    NonIniziata,
    InCorso,
    Completata
}

public class QuestManager : MonoBehaviour
{
    
    public static QuestManager Instance { get; private set; }

    [Header("Quest attiva")]
    public QuestData questAttiva;

    [Header("Riferimenti")]
    public EnemySpawner spawner;

    [Header("Runtime")]
    [SerializeField] private QuestState stato = QuestState.NonIniziata;
    [SerializeField] private int nemiciUccisi = 0;

    public event Action OnQuestAggiornata;
    public event Action OnQuestCompletata;

    public QuestState Stato => stato;
    public int NemiciUccisi => nemiciUccisi;
    public int NemiciTotali => questAttiva != null ? questAttiva.TotaleNemici : 0;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.tKey.wasPressedThisFrame)
        {
            IniziaQuest();
        }
    }
    private void OnEnable()
    {
        EnemyStats.OnAnyEnemyDied += GestisciNemicoUcciso;
    }

    private void OnDisable()
    {
        EnemyStats.OnAnyEnemyDied -= GestisciNemicoUcciso;
    }

    public void IniziaQuest()
    {
        if (stato != QuestState.NonIniziata) return;
        if(questAttiva == null)
        {
            Debug.LogWarning("Nessun QuestData assegnata");
            return;
        }
        stato = QuestState.InCorso;
        nemiciUccisi = 0;

        if(spawner != null)
        {
            spawner.AvviaQuest(questAttiva);
        }
        NotificaAggiornamento();

    }

    private void GestisciNemicoUcciso(EnemyStats nemico)
    {
        if (stato != QuestState.InCorso) return;
        nemiciUccisi++;
        NotificaAggiornamento();

        if(nemiciUccisi >= NemiciTotali)
        {
            CompletaQuest();
        }
    }

    private void CompletaQuest()
    {
        stato = QuestState.Completata;
        PlayerStats ps = UnityEngine.Object.FindFirstObjectByType<PlayerStats>();
        if(ps != null && questAttiva != null)
        {
            ps.AddXp(questAttiva.xpRicompensa);
        }
        if(OnQuestCompletata != null)
        {
            OnQuestCompletata.Invoke();

        }
        NotificaAggiornamento();
    }


    private void NotificaAggiornamento()
    {
        if(OnQuestCompletata != null)
        {
            OnQuestAggiornata.Invoke();
        }
    }

}
