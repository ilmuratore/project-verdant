using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestUI : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private Transform hudRoot;
    [SerializeField] private string hudRootName = "HUD";
    public QuestManager questManager;

    [Header("Pannello avanzamento quest")]
    public GameObject pannelloQuest;
    public TMP_Text titoloQuest;
    public TMP_Text avanzamentoQuest;

    [Header("Pannello vittoria")]
    public GameObject pannelloVittoria;
    public TMP_Text testoVittoria;

    [Header("Pannello sconfitta")]
    public GameObject pannelloSconfitta;
    public TMP_Text testoSconfitta;

    [Header("Nomi oggetti nel prefab HUD")]
    [SerializeField] private string pannelloQuestName = "QuestUI";
    [SerializeField] private string titoloQuestName = "TitoloQuest";
    [SerializeField] private string avanzamentoQuestName = "AvanzamentoQuest";
    [SerializeField] private string pannelloVittoriaName = "Panel_Victory";
    [SerializeField] private string testoVittoriaName = "TestoVittoria";
    [SerializeField] private string pannelloSconfittaName = "Panel_Defeat";
    [SerializeField] private string testoSconfittaName = "TestoSconfitta";

    [Header("Chiusura pannelli finali")]
    public bool chiudiConTasto = true;

    private bool eventSubscribed;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        ResolveReferences();
        SubscribeToQuestManager();
        NascondiTuttiIPannelli();
    }

    private void Update()
    {
        if (!chiudiConTasto) return;
        if (!PannelloFinaleAperto()) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ChiudiPannelliFinali();
        }
    }

    private void OnEnable()
    {
        ResolveReferences();
        SubscribeToQuestManager();
    }

    private void OnDisable()
    {
        UnsubscribeFromQuestManager();
    }

    private void ResolveReferences()
    {
        if (questManager == null || !SceneReferenceFinder.IsSceneInstance(questManager))
        {
            questManager = SceneReferenceFinder.FindComponentInActiveScene<QuestManager>();
        }

        hudRoot = SceneReferenceFinder.ResolveSceneTransform(hudRoot, hudRootName);
        Transform root = hudRoot != null ? hudRoot : transform;

        pannelloQuest = SceneReferenceFinder.ResolveSceneObject(pannelloQuest, root, pannelloQuestName);
        pannelloVittoria = SceneReferenceFinder.ResolveSceneObject(pannelloVittoria, root, pannelloVittoriaName);
        pannelloSconfitta = SceneReferenceFinder.ResolveSceneObject(pannelloSconfitta, root, pannelloSconfittaName);

        Transform questRoot = pannelloQuest != null ? pannelloQuest.transform : root;
        Transform victoryRoot = pannelloVittoria != null ? pannelloVittoria.transform : root;
        Transform defeatRoot = pannelloSconfitta != null ? pannelloSconfitta.transform : root;

        titoloQuest = SceneReferenceFinder.ResolveComponentInChildren(titoloQuest, questRoot, titoloQuestName);
        avanzamentoQuest = SceneReferenceFinder.ResolveComponentInChildren(avanzamentoQuest, questRoot, avanzamentoQuestName);
        testoVittoria = SceneReferenceFinder.ResolveComponentInChildren(testoVittoria, victoryRoot, testoVittoriaName);
        testoSconfitta = SceneReferenceFinder.ResolveComponentInChildren(testoSconfitta, defeatRoot, testoSconfittaName);
    }

    private void SubscribeToQuestManager()
    {
        if (eventSubscribed) return;
        if (questManager == null) return;

        questManager.OnQuestAggiornata += Aggiorna;
        questManager.OnQuestCompletata += MostraVittoria;
        questManager.OnQuestFallita += MostraSconfitta;
        eventSubscribed = true;
    }

    private void UnsubscribeFromQuestManager()
    {
        if (!eventSubscribed) return;
        if (questManager == null) return;

        questManager.OnQuestAggiornata -= Aggiorna;
        questManager.OnQuestCompletata -= MostraVittoria;
        questManager.OnQuestFallita -= MostraSconfitta;
        eventSubscribed = false;
    }

    private void Aggiorna()
    {
        ResolveReferences();

        if (questManager == null) return;

        if (questManager.Stato == QuestState.InCorso)
        {
            if (pannelloQuest != null) pannelloQuest.SetActive(true);
            if (pannelloVittoria != null) pannelloVittoria.SetActive(false);
            if (pannelloSconfitta != null) pannelloSconfitta.SetActive(false);

            if (titoloQuest != null && questManager.questAttiva != null)
            {
                titoloQuest.text = questManager.questAttiva.titolo;
            }

            if (avanzamentoQuest != null)
            {
                avanzamentoQuest.text =
                    "Nemici: " + questManager.NemiciUccisi + " / " + questManager.NemiciTotali + "\n" +
                    "Monaci salvi: " + questManager.MonaciSalvi + " / " + questManager.MonaciTotali;
            }
        }
        else
        {
            if (pannelloQuest != null) pannelloQuest.SetActive(false);
        }
    }

    private void MostraVittoria()
    {
        ResolveReferences();

        if (pannelloQuest != null) pannelloQuest.SetActive(false);
        if (pannelloVittoria != null) pannelloVittoria.SetActive(true);
        if (pannelloSconfitta != null) pannelloSconfitta.SetActive(false);

        if (testoVittoria != null && questManager != null && questManager.questAttiva != null)
        {
            testoVittoria.text =
                "Quest completata!\n" +
                "Hai protetto tutti i monaci.\n" +
                "+" + questManager.questAttiva.xpRicompensa + " XP\n\n" +
                "Premi ESC, Invio o Spazio per chiudere.";
        }
    }

    private void MostraSconfitta()
    {
        ResolveReferences();

        if (pannelloQuest != null) pannelloQuest.SetActive(false);
        if (pannelloVittoria != null) pannelloVittoria.SetActive(false);
        if (pannelloSconfitta != null) pannelloSconfitta.SetActive(true);

        if (testoSconfitta != null)
        {
            testoSconfitta.text =
                "Quest fallita!\n" +
                "Un monaco è stato ucciso.\n" +
                "Difendi meglio e riprova.\n\n" +
                "Premi ESC, Invio o Spazio per chiudere.";
        }
    }

    public void ChiudiPannelliFinali()
    {
        if (pannelloVittoria != null) pannelloVittoria.SetActive(false);
        if (pannelloSconfitta != null) pannelloSconfitta.SetActive(false);
    }

    private bool PannelloFinaleAperto()
    {
        bool vittoriaAperta = pannelloVittoria != null && pannelloVittoria.activeSelf;
        bool sconfittaAperta = pannelloSconfitta != null && pannelloSconfitta.activeSelf;

        return vittoriaAperta || sconfittaAperta;
    }

    private void NascondiTuttiIPannelli()
    {
        if (pannelloQuest != null) pannelloQuest.SetActive(false);
        if (pannelloVittoria != null) pannelloVittoria.SetActive(false);
        if (pannelloSconfitta != null) pannelloSconfitta.SetActive(false);
    }
}
