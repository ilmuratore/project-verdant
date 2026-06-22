using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestUI : MonoBehaviour
{
    [Header("Riferimenti")]
    public QuestManager questManager;

    [Header("Pannello quest")]
    public GameObject pannelloQuest;
    public TMP_Text titoloQuest;
    public TMP_Text avanzamentoQuest;

    [Header("Pannello vittoria")]
    public GameObject pannelloVittoria;
    public TMP_Text testoVittoria;

    [Header("Pannello sconfitta")]
    public GameObject pannelloSconfitta;
    public TMP_Text testoSconfitta;

    [Header("Chiusura pannelli finali")]
    public bool chiudiConTasto = true;

    private void Start()
    {
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
