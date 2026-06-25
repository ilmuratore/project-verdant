using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private GameObject healthPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject minimapPanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject questProgressPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private TMP_Text healthValueText;
    [SerializeField] private Animator healthValueAnimator;
    [SerializeField] private TMP_Text levelValueText;
    [SerializeField] private TMP_Text xpValueText;
    [SerializeField] private TMP_Text pointsValueText;
    [SerializeField] private TMP_Text attackValueText;
    [SerializeField] private TMP_Text defenseValueText;
    [SerializeField] private TMP_Text vitalityValueText;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueBodyText;
    [SerializeField] private TMP_Text questTitleText;
    [SerializeField] private TMP_Text questProgressText;
    [SerializeField] private TMP_Text victoryMessageText;
    [SerializeField] private TMP_Text defeatMessageText;
    [SerializeField] private TMP_Text nextLevelMessageText;

    [SerializeField] private Button attackUpgradeButton;
    [SerializeField] private Button defenseUpgradeButton;
    [SerializeField] private Button vitalityUpgradeButton;
    [SerializeField] private Button acceptQuestButton;
    [SerializeField] private Button continueToNextLevelButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button loadNextLevelButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadSaveButton;
    [SerializeField] private Button deleteSaveButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetAudioButton;

    [SerializeField] private bool showStatsOnStart;
    [SerializeField] private string[] defaultDialogueLines =
    {
        "Eroe, hanno invaso il villaggio!",
        "I nemici sono numerosi. Proteggi i monaci.",
        "Conta su di me.",
        "Eliminali tutti. Difendi il villaggio."
    };

    private QuestManager questManager;
    private PlayerStats playerStats;
    private PlayerMovement playerMovement;
    private NPC_AI currentNpc;
    private GameInput input;
    private int dialogueIndex;
    private bool dialogueOpen;
    private bool eventsBound;

    public bool IsDialogueOpen => dialogueOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResolveReferences();
        BindButtons();
    }


    private void OnEnable()
    {
        input = GameInput.GetOrCreate();
        input.OnPause += TogglePause;
        input.OnInteract += HandleInteract;
        input.OnToggleStats += ToggleStatsPanel;
        SubscribeQuestManager();
        SubscribePlayerStats();
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.OnPause -= TogglePause;
            input.OnInteract -= HandleInteract;
            input.OnToggleStats -= ToggleStatsPanel;
        }

        UnsubscribeQuestManager();
        UnsubscribePlayerStats();
    }

    private void Start()
    {
        ResolveReferences();
        BindButtons();
        SetInitialPanels();
        RefreshPlayerStats();
        RefreshQuestProgress();
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        if (uiRoot == null) Debug.LogError("UIManager: uiRoot mancante.");
        if (gameplayHUD == null) Debug.LogError("UIManager: gameplayHUD mancante.");
        if (healthPanel == null) Debug.LogError("UIManager: healthPanel mancante.");
        if (statsPanel == null) Debug.LogError("UIManager: statsPanel mancante.");
        if (minimapPanel == null) Debug.LogError("UIManager: minimapPanel mancante.");
        if (dialoguePanel == null) Debug.LogError("UIManager: dialoguePanel mancante.");
        if (questProgressPanel == null) Debug.LogError("UIManager: questProgressPanel mancante.");
        if (victoryPanel == null) Debug.LogError("UIManager: victoryPanel mancante.");
        if (defeatPanel == null) Debug.LogError("UIManager: defeatPanel mancante.");
        if (nextLevelPanel == null) Debug.LogError("UIManager: nextLevelPanel mancante.");
        if (pauseMenuPanel == null) Debug.LogError("UIManager: pauseMenuPanel mancante.");
        if (settingsPanel == null) Debug.LogError("UIManager: settingsPanel mancante.");
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void ResolveReferences()
    {
        uiRoot = uiRoot != null ? uiRoot : SceneReferenceFinder.ResolveSceneTransform(null, "UIRoot_Gameplay");
        Transform root = uiRoot != null ? uiRoot : transform;

        gameplayHUD = ResolveObject(gameplayHUD, root, "GameplayHUD");
        healthPanel = ResolveObject(healthPanel, root, "HealthPanel");
        statsPanel = ResolveObject(statsPanel, root, "StatsPanel");
        minimapPanel = ResolveObject(minimapPanel, root, "MinimapPanel");
        dialoguePanel = ResolveObject(dialoguePanel, root, "DialoguePanel");
        questProgressPanel = ResolveObject(questProgressPanel, root, "QuestProgressPanel");
        victoryPanel = ResolveObject(victoryPanel, root, "VictoryPanel");
        defeatPanel = ResolveObject(defeatPanel, root, "DefeatPanel");
        nextLevelPanel = ResolveObject(nextLevelPanel, root, "NextLevelPanel");
        pauseMenuPanel = ResolveObject(pauseMenuPanel, root, "PauseMenuPanel");
        settingsPanel = ResolveObject(settingsPanel, root, "SettingsPanel");

        healthValueText = ResolveText(healthValueText, root, "HealthValueText");
        if (healthValueText == null) healthValueText = ResolveText(healthValueText, root, "HealthText");
        if (healthValueText != null && healthValueAnimator == null) healthValueAnimator = healthValueText.GetComponent<Animator>();

        levelValueText = ResolveText(levelValueText, root, "LevelValueText");
        xpValueText = ResolveText(xpValueText, root, "XPValueText");
        pointsValueText = ResolveText(pointsValueText, root, "PointsValueText");
        attackValueText = ResolveText(attackValueText, root, "AttackValueText");
        defenseValueText = ResolveText(defenseValueText, root, "DefenseValueText");
        vitalityValueText = ResolveText(vitalityValueText, root, "VitalityValueText");
        speakerNameText = ResolveText(speakerNameText, root, "SpeakerNameText");
        dialogueBodyText = ResolveText(dialogueBodyText, root, "DialogueBodyText");
        questTitleText = ResolveText(questTitleText, root, "QuestTitleText");
        questProgressText = ResolveText(questProgressText, root, "QuestProgressText");
        victoryMessageText = ResolveText(victoryMessageText, root, "VictoryMessageText");
        defeatMessageText = ResolveText(defeatMessageText, root, "DefeatMessageText");
        nextLevelMessageText = ResolveText(nextLevelMessageText, root, "NextLevelMessageText");

        attackUpgradeButton = ResolveButton(attackUpgradeButton, root, "AttackUpgradeButton");
        defenseUpgradeButton = ResolveButton(defenseUpgradeButton, root, "DefenseUpgradeButton");
        vitalityUpgradeButton = ResolveButton(vitalityUpgradeButton, root, "VitalityUpgradeButton");
        acceptQuestButton = ResolveButton(acceptQuestButton, root, "AcceptQuestButton");
        continueToNextLevelButton = ResolveButton(continueToNextLevelButton, root, "ContinueToNextLevelButton");
        retryButton = ResolveButton(retryButton, root, "RetryButton");
        loadNextLevelButton = ResolveButton(loadNextLevelButton, root, "LoadNextLevelButton");
        continueButton = ResolveButton(continueButton, root, "ContinueButton");
        settingsButton = ResolveButton(settingsButton, root, "SettingsButton");
        restartButton = ResolveButton(restartButton, root, "RestartButton");
        mainMenuButton = ResolveButton(mainMenuButton, root, "MainMenuButton");
        saveButton = ResolveButton(saveButton, root, "SaveButton");
        loadSaveButton = ResolveButton(loadSaveButton, root, "LoadSaveButton");
        deleteSaveButton = ResolveButton(deleteSaveButton, root, "DeleteSaveButton");
        exitButton = ResolveButton(exitButton, root, "ExitButton");
        backButton = ResolveButton(backButton, root, "BackButton");
        resetAudioButton = ResolveButton(resetAudioButton, root, "ResetAudioButton");

        if (questManager == null || !SceneReferenceFinder.IsSceneInstance(questManager)) questManager = FindFirstObjectByType<QuestManager>();
        if (playerStats == null || !SceneReferenceFinder.IsSceneInstance(playerStats)) playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerMovement == null || !SceneReferenceFinder.IsSceneInstance(playerMovement)) playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    private GameObject ResolveObject(GameObject current, Transform root, string name)
    {
        return SceneReferenceFinder.ResolveSceneObject(current, root, name);
    }

    private TMP_Text ResolveText(TMP_Text current, Transform root, string name)
    {
        return SceneReferenceFinder.ResolveComponentInChildren(current, root, name);
    }

    private Button ResolveButton(Button current, Transform root, string name)
    {
        return SceneReferenceFinder.ResolveComponentInChildren(current, root, name);
    }

    private void BindButtons()
    {
        if (eventsBound) return;

        BindButton(attackUpgradeButton, () => SpendStat(StatType.Attacco));
        BindButton(defenseUpgradeButton, () => SpendStat(StatType.Difesa));
        BindButton(vitalityUpgradeButton, () => SpendStat(StatType.Vita));
        BindButton(acceptQuestButton, AcceptQuest);
        BindButton(continueToNextLevelButton, ShowNextLevelPanel);
        BindButton(retryButton, () => SceneLoader.Instance?.RestartScene());
        BindButton(loadNextLevelButton, () => SceneLoader.Instance?.LoadNextScene());
        BindButton(continueButton, ResumeGame);
        BindButton(saveButton, SaveGame);
        BindButton(loadSaveButton, LoadSavedGame);
        BindButton(deleteSaveButton, DeleteSavedGame);
        BindButton(settingsButton, OpenSettings);
        BindButton(restartButton, () => SceneLoader.Instance?.RestartScene());
        BindButton(mainMenuButton, () => SceneLoader.Instance?.LoadMainMenu());
        BindButton(exitButton, () => SceneLoader.Instance?.QuitGame());
        BindButton(backButton, BackFromSettings);
        BindButton(resetAudioButton, () => AudioManager.Instance?.ResetAudioSettings());

        eventsBound = true;
    }

    private void BindButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null || action == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    private void SetInitialPanels()
    {
        SetActive(gameplayHUD, true);
        SetActive(healthPanel, true);
        SetActive(statsPanel, showStatsOnStart);
        SetActive(minimapPanel, true);
        SetActive(dialoguePanel, false);
        SetActive(questProgressPanel, false);
        SetActive(victoryPanel, false);
        SetActive(defeatPanel, false);
        SetActive(nextLevelPanel, false);
        SetActive(pauseMenuPanel, false);
        SetActive(settingsPanel, false);
        SetActive(acceptQuestButton != null ? acceptQuestButton.gameObject : null, false);
    }

    private void SubscribeQuestManager()
    {
        ResolveReferences();
        if (questManager == null) return;

        questManager.OnQuestAggiornata -= RefreshQuestProgress;
        questManager.OnQuestCompletata -= ShowVictory;
        questManager.OnQuestFallita -= ShowDefeat;

        questManager.OnQuestAggiornata += RefreshQuestProgress;
        questManager.OnQuestCompletata += ShowVictory;
        questManager.OnQuestFallita += ShowDefeat;
    }

    private void UnsubscribeQuestManager()
    {
        if (questManager == null) return;

        questManager.OnQuestAggiornata -= RefreshQuestProgress;
        questManager.OnQuestCompletata -= ShowVictory;
        questManager.OnQuestFallita -= ShowDefeat;
    }

    private void SubscribePlayerStats()
    {
        ResolveReferences();
        if (playerStats == null) return;
        playerStats.OnStatsChanged -= RefreshPlayerStats;
        playerStats.OnStatsChanged += RefreshPlayerStats;
    }

    private void UnsubscribePlayerStats()
    {
        if (playerStats == null) return;
        playerStats.OnStatsChanged -= RefreshPlayerStats;
    }

    public void UpdatePlayerHealth(int current, int max)
    {
        ResolveReferences();

        if (healthValueText != null) healthValueText.text = "HP: " + current + " / " + max;
        if (healthValueAnimator != null) healthValueAnimator.Play("TextAnimation");
    }

    public void RefreshPlayerStats()
    {
        ResolveReferences();
        if (playerStats == null) return;

        if (levelValueText != null) levelValueText.text = "Livello: " + playerStats.Level;
        if (xpValueText != null) xpValueText.text = "XP: " + playerStats.CurrentXp + " / " + playerStats.XpNecessari;
        if (pointsValueText != null) pointsValueText.text = "Punti disp.: " + playerStats.PuntiDisponibili;
        if (attackValueText != null) attackValueText.text = "Attacco: " + playerStats.AttaccoEffettivo;
        if (defenseValueText != null) defenseValueText.text = "Difesa: " + playerStats.DifesaEffettivo;
        if (vitalityValueText != null) vitalityValueText.text = "Vita max: " + playerStats.vitaMassimaEffettiva;

        bool canSpend = playerStats.PuntiDisponibili > 0;
        SetInteractable(attackUpgradeButton, canSpend);
        SetInteractable(defenseUpgradeButton, canSpend);
        SetInteractable(vitalityUpgradeButton, canSpend);
    }

    public void ToggleStatsPanel()
    {
        ResolveReferences();
        if (statsPanel == null) return;
        statsPanel.SetActive(!statsPanel.activeSelf);
        if (statsPanel.activeSelf) RefreshPlayerStats();
    }

    private void SpendStat(StatType statType)
    {
        ResolveReferences();
        if (playerStats == null) return;
        playerStats.SpendiPunto(statType);
        RefreshPlayerStats();
    }

    public void OpenDialogue(NPC_AI npc)
    {
        if (dialogueOpen) return;

        ResolveReferences();

        currentNpc = npc;
        dialogueOpen = true;
        dialogueIndex = 0;

        playerMovement?.BloccaController();
        GameManager.Instance?.SetDialogue();

        SetActive(dialoguePanel, true);
        SetActive(acceptQuestButton != null ? acceptQuestButton.gameObject : null, false);
        ShowDialogueLine();
    }

    private void HandleInteract()
    {
        if (!dialogueOpen) return;
        NextDialogueLine();
    }

    private void NextDialogueLine()
    {
        if (defaultDialogueLines == null || defaultDialogueLines.Length == 0) return;
        if (dialogueIndex >= defaultDialogueLines.Length - 1) return;

        dialogueIndex++;
        ShowDialogueLine();
    }

    private void ShowDialogueLine()
    {
        if (defaultDialogueLines == null || defaultDialogueLines.Length == 0) return;

        if (speakerNameText != null) speakerNameText.text = "Monaco";
        if (dialogueBodyText != null) dialogueBodyText.text = defaultDialogueLines[dialogueIndex];

        bool lastLine = dialogueIndex >= defaultDialogueLines.Length - 1;
        SetActive(acceptQuestButton != null ? acceptQuestButton.gameObject : null, lastLine);
    }

    private void AcceptQuest()
    {
        QuestManager.Instance?.IniziaQuest();
        CloseDialogue();
    }

    private void CloseDialogue()
    {
        dialogueOpen = false;
        SetActive(dialoguePanel, false);
        SetActive(acceptQuestButton != null ? acceptQuestButton.gameObject : null, false);
        playerMovement?.SbloccaController();
        currentNpc?.FineDialogo();
        currentNpc = null;
        GameManager.Instance?.SetGameplay();
    }

    public void RefreshQuestProgress()
    {
        ResolveReferences();
        if (questManager == null) return;

        bool questRunning = questManager.Stato == QuestState.InCorso;
        SetActive(questProgressPanel, questRunning);

        if (!questRunning) return;

        if (questTitleText != null && questManager.questAttiva != null)
        {
            questTitleText.text = questManager.questAttiva.titolo;
        }

        if (questProgressText != null)
        {
            questProgressText.text = "Nemici: " + questManager.NemiciUccisi + " / " + questManager.NemiciTotali + "\n" +
                                     "Monaci salvi: " + questManager.MonaciSalvi + " / " + questManager.MonaciTotali;
        }
    }

    private void ShowVictory()
    {
        ResolveReferences();
        GameManager.Instance?.SetResult();

        SetActive(questProgressPanel, false);
        SetActive(victoryPanel, true);
        SetActive(defeatPanel, false);
        SetActive(nextLevelPanel, false);
        SetActive(pauseMenuPanel, false);
        SetActive(settingsPanel, false);

        if (victoryMessageText != null)
        {
            int xp = questManager != null && questManager.questAttiva != null ? questManager.questAttiva.xpRicompensa : 0;
            victoryMessageText.text = "Quest completata!\nHai protetto tutti i monaci.\n+" + xp + " XP";
        }
    }

    private void ShowDefeat()
    {
        ResolveReferences();
        GameManager.Instance?.SetResult();

        SetActive(questProgressPanel, false);
        SetActive(victoryPanel, false);
        SetActive(defeatPanel, true);
        SetActive(nextLevelPanel, false);
        SetActive(pauseMenuPanel, false);
        SetActive(settingsPanel, false);

        if (defeatMessageText != null)
        {
            defeatMessageText.text = "Quest fallita!\nUn monaco è stato ucciso.\nRiprova e difendi meglio il villaggio.";
        }
    }

    public void ShowNextLevelPanel()
    {
        ResolveReferences();
        SetActive(victoryPanel, false);
        SetActive(nextLevelPanel, true);

        if (nextLevelMessageText != null)
        {
            nextLevelMessageText.text = "Preparati per il prossimo livello.";
        }
    }

    public void HideResultPanels()
    {
        SetActive(victoryPanel, false);
        SetActive(defeatPanel, false);
        SetActive(nextLevelPanel, false);
        GameManager.Instance?.SetGameplay();
    }

    public void TogglePause()
    {
        if (dialogueOpen) return;
        if (victoryPanel != null && victoryPanel.activeSelf) return;
        if (defeatPanel != null && defeatPanel.activeSelf) return;
        if (nextLevelPanel != null && nextLevelPanel.activeSelf) return;

        if (pauseMenuPanel != null && pauseMenuPanel.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        ResolveReferences();
        GameManager.Instance?.Pause();
        SetActive(pauseMenuPanel, true);
        SetActive(settingsPanel, false);
    }

    public void ResumeGame()
    {
        ResolveReferences();
        GameManager.Instance?.Resume();
        SetActive(pauseMenuPanel, false);
        SetActive(settingsPanel, false);
    }


    public void SaveGame()
    {
        SaveSystem.SaveCurrentGame();
    }


    public void LoadSavedGame()
    {
        Time.timeScale = 1f;
        SaveSystem.LoadSaveGame();
    }

    public void DeleteSavedGame()
    {
        SaveSystem.DeleteSave();
    }

    public void OpenSettings()
    {
        ResolveReferences();
        SetActive(pauseMenuPanel, false);
        SetActive(settingsPanel, true);
    }

    public void BackFromSettings()
    {
        ResolveReferences();
        SetActive(settingsPanel, false);
        SetActive(pauseMenuPanel, true);
    }

    private void SetActive(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }

    private void SetInteractable(Button button, bool interactable)
    {
        if (button != null) button.interactable = interactable;
    }
}
