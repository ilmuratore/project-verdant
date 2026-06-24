using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StatsPanelUI : MonoBehaviour
{
    [Header("Riferimenti")]
    public PlayerStats stats;
    public GameObject panel;

    [Header("Nomi oggetti nel prefab HUD")]
    [SerializeField] private string panelName = "StatsPanel";
    [SerializeField] private string levelTextName = "Leveltext";
    [SerializeField] private string xpTextName = "XPText";
    [SerializeField] private string pointsTextName = "PointsText";
    [SerializeField] private string attaccoTextName = "AttaccoText";
    [SerializeField] private string difesaTextName = "DifesaText";
    [SerializeField] private string vitaTextName = "VitaText";
    [SerializeField] private string attaccoButtonName = "AttaccoButton";
    [SerializeField] private string difesaButtonName = "DifesaButton";
    [SerializeField] private string vitaButtonName = "VitaButton";

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
    private bool statsSubscribed = false;
    private bool buttonsBound = false;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        ResolveReferences();
        BindButtons();
        SubscribeToStats();

        if (panel != null)
        {
            panel.SetActive(false);
        }

        Aggiorna();
    }

    private void OnEnable()
    {
        ResolveReferences();
        BindButtons();
        SubscribeToStats();
    }

    private void OnDisable()
    {
        UnsubscribeFromStats();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            ResolveReferences();

            isOpen = !isOpen;

            if (panel != null)
            {
                panel.SetActive(isOpen);
            }

            if (isOpen)
            {
                Aggiorna();
            }
        }
    }

    private void ResolveReferences()
    {
        if (stats == null || !SceneReferenceFinder.IsSceneInstance(stats))
        {
            stats = SceneReferenceFinder.FindComponentInActiveScene<PlayerStats>();
        }

        panel = SceneReferenceFinder.ResolveSceneObject(panel, transform, panelName);

        Transform root = panel != null ? panel.transform : transform;

        levelText = SceneReferenceFinder.ResolveComponentInChildren(levelText, root, levelTextName);
        xpText = SceneReferenceFinder.ResolveComponentInChildren(xpText, root, xpTextName);
        pointsText = SceneReferenceFinder.ResolveComponentInChildren(pointsText, root, pointsTextName);
        attaccoText = SceneReferenceFinder.ResolveComponentInChildren(attaccoText, root, attaccoTextName);
        difesaText = SceneReferenceFinder.ResolveComponentInChildren(difesaText, root, difesaTextName);
        vitaText = SceneReferenceFinder.ResolveComponentInChildren(vitaText, root, vitaTextName);

        attaccoButton = SceneReferenceFinder.ResolveComponentInChildren(attaccoButton, root, attaccoButtonName);
        difesaButton = SceneReferenceFinder.ResolveComponentInChildren(difesaButton, root, difesaButtonName);
        vitaButton = SceneReferenceFinder.ResolveComponentInChildren(vitaButton, root, vitaButtonName);
    }

    private void BindButtons()
    {
        if (buttonsBound) return;

        if (attaccoButton != null)
        {
            attaccoButton.onClick.RemoveAllListeners();
            attaccoButton.onClick.AddListener(() => Spendi(StatType.Attacco));
        }

        if (difesaButton != null)
        {
            difesaButton.onClick.RemoveAllListeners();
            difesaButton.onClick.AddListener(() => Spendi(StatType.Difesa));
        }

        if (vitaButton != null)
        {
            vitaButton.onClick.RemoveAllListeners();
            vitaButton.onClick.AddListener(() => Spendi(StatType.Vita));
        }

        buttonsBound = attaccoButton != null && difesaButton != null && vitaButton != null;
    }

    private void SubscribeToStats()
    {
        if (statsSubscribed) return;
        if (stats == null) return;

        stats.OnStatsChanged += Aggiorna;
        statsSubscribed = true;
    }

    private void UnsubscribeFromStats()
    {
        if (!statsSubscribed) return;
        if (stats == null) return;

        stats.OnStatsChanged -= Aggiorna;
        statsSubscribed = false;
    }

    private void Spendi(StatType tipo)
    {
        if (stats != null)
        {
            stats.SpendiPunto(tipo);
        }
    }

    private void Aggiorna()
    {
        if (stats == null) return;

        if (levelText != null) levelText.text = "Livello: " + stats.level;
        if (xpText != null) xpText.text = "XP: " + stats.currentXp + " / " + stats.XpNecessari;
        if (pointsText != null) pointsText.text = "Punti disp.: " + stats.puntiDisponibili;
        if (attaccoText != null) attaccoText.text = "Attacco: " + stats.AttaccoEffettivo;
        if (difesaText != null) difesaText.text = "Difesa: " + stats.DifesaEffettivo;
        if (vitaText != null) vitaText.text = "Vita max: " + stats.vitaMassimaEffettiva;

        bool puoiSpendere = stats.puntiDisponibili > 0;

        if (attaccoButton != null) attaccoButton.interactable = puoiSpendere;
        if (difesaButton != null) difesaButton.interactable = puoiSpendere;
        if (vitaButton != null) vitaButton.interactable = puoiSpendere;
    }
}
