using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [Header("Riferimenti")]
    public QuestManager questManager;

    [Header("Pannello quest (in alto a destra)")]
    public GameObject pannelloQuest;
    public TMP_Text titoloQuest;
    public TMP_Text avanzamentoQuest;

    [Header("Pannello vittoria (al centro)")]
    public GameObject pannelloVittoria;
    public TMP_Text testoVittoria;

    [Header("Pannello sconfitta (al centro)")]
    public GameObject pannelloSconfitta;
    public TMP_Text testoSconfitta;

    void Start()
    {
        if (pannelloQuest != null) pannelloQuest.SetActive(false);
        if (pannelloVittoria != null) pannelloVittoria.SetActive(false);
        if (pannelloSconfitta != null) pannelloSconfitta.SetActive(false);
    }

    private void OnEnable()
    {
        if (questManager != null)
        {
            questManager.OnQuestAggiornata += Aggiorna;
            questManager.OnQuestCompletata += MostraVittoria;
            questManager.OnQuestFallita += MostraSconfitta;
        }
    }

    private void OnDisable()
    {
        if (questManager != null)
        {
            questManager.OnQuestAggiornata -= Aggiorna;
            questManager.OnQuestCompletata -= MostraVittoria;
            questManager.OnQuestFallita -= MostraSconfitta;
        }
    }

    private void Aggiorna()
    {
        if (questManager == null) return;

        if (questManager.Stato == QuestState.InCorso)
        {
            if (pannelloQuest != null) pannelloQuest.SetActive(true);

            if (titoloQuest != null && questManager.questAttiva != null)
                titoloQuest.text = questManager.questAttiva.titolo;

            if (avanzamentoQuest != null)
                avanzamentoQuest.text =
                    "Nemici: " + questManager.NemiciUccisi + " / " + questManager.NemiciTotali + "\n" +
                    "Monaci Salvi: " + questManager.MonaciSalvi + " / " + questManager.MonaciTotali;

        }
        else if (questManager.Stato == QuestState.Completata)
        {
            if (pannelloQuest != null) pannelloQuest.SetActive(false);
        }
    }

    private void MostraVittoria()
    {
        if (pannelloVittoria != null) pannelloVittoria.SetActive(true);

        if (testoVittoria != null && questManager != null && questManager.questAttiva != null)
        {
            testoVittoria.text =
                "Quest completata!\n+" + 
                "Hai protetto tutti i monaci.\n" +
                " + " + questManager.questAttiva.xpRicompensa + " XP";

        }
    }

    private void MostraSconfitta()
    {
        if (pannelloQuest != null) pannelloQuest.SetActive(false);
        if (pannelloVittoria != null) pannelloVittoria.SetActive(false);
        if (pannelloSconfitta != null) pannelloSconfitta.SetActive(true);

        if(testoSconfitta != null)
        {
            testoSconfitta.text =
                "Quest Fallita!\n" +
                "Un monaco é stato ucciso.\n" +
                "Difendi meglio e riprova!."; 
        }
    }
}
