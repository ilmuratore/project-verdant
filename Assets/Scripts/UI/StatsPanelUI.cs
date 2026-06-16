using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StatsPanelUI : MonoBehaviour
{

    [Header("Riferimenti")]
    public PlayerStats stats;
    public GameObject panel;

    [Header("Testi")]
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text pointsText;
    public TMP_Text attaccoText;
    public TMP_Text difesaText;
    public TMP_Text vitaText;

    [Header("Bottoni")]
    public Button attaccoButton;
    public Button difesaButton;
    public Button vitaButton;


    private bool isOpen = false;


    void Start()
    {
        attaccoButton.onClick.AddListener(() => Spendi(StatType.Attacco));
        difesaButton.onClick.AddListener(() => Spendi(StatType.Difesa));
        vitaButton.onClick.AddListener(() => Spendi(StatType.Vita));

        if(stats != null)
        {
            stats.OnStatsChanged += Aggiorna;
        }
        panel.SetActive(false);
        Aggiorna();
    }
    

    private void OnDestroy()
    {
        if(stats != null)
        {
            stats.OnStatsChanged -= Aggiorna;
        }
    }



    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            isOpen = !isOpen;
            panel.SetActive(isOpen);
            if (isOpen)
            {
                Aggiorna();
            }
        }
    }

    private void Spendi(StatType tipo)
    {
        if(stats != null)
        {
            stats.SpendiPunto(tipo);
        }
    }

    private void Aggiorna()
    {
        if (stats == null) return;
        levelText.text = "Livello: " + stats.level;
        xpText.text = "XP: " + stats.currentXp + " / " + stats.XpNecessari;
        pointsText.text = "Punti disp.: " + stats.puntiDisponibili;
        attaccoText.text = "Attacco: " + stats.AttaccoEffettivo;
        difesaText.text = "Difesa: " + stats.DifesaEffettivo;
        vitaText.text = "Vita max: " + stats.vitaMassimaEffettiva;

        bool puoiSpendere = stats.puntiDisponibili > 0;
        attaccoButton.interactable = puoiSpendere;
        difesaButton.interactable = puoiSpendere;
        vitaButton.interactable = puoiSpendere;
    }
}
