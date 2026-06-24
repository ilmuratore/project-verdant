using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Video UI")]
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;

    [Header("Audio Value Texts")]
    [SerializeField] private TMP_Text masterVolumeValueText;
    [SerializeField] private TMP_Text musicVolumeValueText;
    [SerializeField] private TMP_Text ambientVolumeValueText;
    [SerializeField] private TMP_Text sfxVolumeValueText;
    [SerializeField] private TMP_Text uiVolumeValueText;

    [Header("Nomi oggetti audio nel prefab")]
    [SerializeField] private string masterVolumeSliderName = "Slider_MasterVolume";
    [SerializeField] private string musicVolumeSliderName = "Slider_MusicVolume";
    [SerializeField] private string ambientVolumeSliderName = "Slider_AmbientVolume";
    [SerializeField] private string sfxVolumeSliderName = "Slider_SfxVolume";
    [SerializeField] private string uiVolumeSliderName = "Slider_UiVolume";

    [SerializeField] private string masterVolumeTextName = "Text_MasterVolume";
    [SerializeField] private string musicVolumeTextName = "Text_MusicVolume";
    [SerializeField] private string ambientVolumeTextName = "Text_AmbientVolume";
    [SerializeField] private string sfxVolumeTextName = "Text_SfxVolume";
    [SerializeField] private string uiVolumeTextName = "Text_UiVolume";

    private Resolution[] resolutions;
    private bool isInitializingAudioUI;
    private bool videoSetupDone;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        SetupVideoIfNeeded();
        SetupAudioSliders();
    }

    private void OnEnable()
    {
        ResolveReferences();
        SetupAudioSliders();
        RefreshAudioSlidersFromManager();
    }

    private void ResolveReferences()
    {
        Transform root = transform;

        masterVolumeSlider = SceneReferenceFinder.ResolveComponentInChildren(masterVolumeSlider, root, masterVolumeSliderName);
        musicVolumeSlider = SceneReferenceFinder.ResolveComponentInChildren(musicVolumeSlider, root, musicVolumeSliderName);
        ambientVolumeSlider = SceneReferenceFinder.ResolveComponentInChildren(ambientVolumeSlider, root, ambientVolumeSliderName);
        sfxVolumeSlider = SceneReferenceFinder.ResolveComponentInChildren(sfxVolumeSlider, root, sfxVolumeSliderName);
        uiVolumeSlider = SceneReferenceFinder.ResolveComponentInChildren(uiVolumeSlider, root, uiVolumeSliderName);

        masterVolumeValueText = SceneReferenceFinder.ResolveComponentInChildren(masterVolumeValueText, root, masterVolumeTextName);
        musicVolumeValueText = SceneReferenceFinder.ResolveComponentInChildren(musicVolumeValueText, root, musicVolumeTextName);
        ambientVolumeValueText = SceneReferenceFinder.ResolveComponentInChildren(ambientVolumeValueText, root, ambientVolumeTextName);
        sfxVolumeValueText = SceneReferenceFinder.ResolveComponentInChildren(sfxVolumeValueText, root, sfxVolumeTextName);
        uiVolumeValueText = SceneReferenceFinder.ResolveComponentInChildren(uiVolumeValueText, root, uiVolumeTextName);
    }

    private void SetupVideoIfNeeded()
    {
        if (videoSetupDone) return;

        SetupFullScreen();
        SetupQualityDropdown();
        SetupResolutionDropdown();

        videoSetupDone = true;
    }

    private void SetupFullScreen()
    {
        if (fullScreenToggle == null) return;

        fullScreenToggle.isOn = Screen.fullScreen;
        fullScreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        fullScreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void SetupQualityDropdown()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (string qualityName in QualitySettings.names)
        {
            options.Add(qualityName);
        }

        qualityDropdown.AddOptions(options);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        qualityDropdown.onValueChanged.RemoveListener(SetQuality);
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            int refreshRate = Mathf.RoundToInt((float)resolutions[i].refreshRateRatio.value);
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + refreshRate + " Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetupAudioSliders()
    {
        ConfigureSlider(masterVolumeSlider);
        ConfigureSlider(musicVolumeSlider);
        ConfigureSlider(ambientVolumeSlider);
        ConfigureSlider(sfxVolumeSlider);
        ConfigureSlider(uiVolumeSlider);

        AddSliderListener(masterVolumeSlider, SetMasterVolume);
        AddSliderListener(musicVolumeSlider, SetMusicVolume);
        AddSliderListener(ambientVolumeSlider, SetAmbientVolume);
        AddSliderListener(sfxVolumeSlider, SetSfxVolume);
        AddSliderListener(uiVolumeSlider, SetUiVolume);
    }

    private void AddSliderListener(Slider slider, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null) return;

        slider.onValueChanged.RemoveListener(callback);
        slider.onValueChanged.AddListener(callback);
    }

    private void ConfigureSlider(Slider slider)
    {
        if (slider == null) return;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }

    private void RefreshAudioSlidersFromManager()
    {
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager == null) return;

        isInitializingAudioUI = true;

        SetSliderValueWithoutNotify(masterVolumeSlider, audioManager.masterVolume);
        SetSliderValueWithoutNotify(musicVolumeSlider, audioManager.musicVolume);
        SetSliderValueWithoutNotify(ambientVolumeSlider, audioManager.ambientVolume);
        SetSliderValueWithoutNotify(sfxVolumeSlider, audioManager.sfxVolume);
        SetSliderValueWithoutNotify(uiVolumeSlider, audioManager.uiVolume);

        RefreshAllVolumeTexts();

        isInitializingAudioUI = false;
    }

    private void SetSliderValueWithoutNotify(Slider slider, float value)
    {
        if (slider == null) return;
        slider.SetValueWithoutNotify(Mathf.Clamp01(value));
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    public void SetQuality(int index)
    {
        if (index < 0 || index >= QualitySettings.names.Length) return;
        QualitySettings.SetQualityLevel(index);
    }

    public void SetResolution(int index)
    {
        if (resolutions == null || resolutions.Length == 0) return;
        if (index < 0 || index >= resolutions.Length) return;

        Resolution selected = resolutions[index];
        Screen.SetResolution(selected.width, selected.height, Screen.fullScreen);
    }

    public void SetMasterVolume(float value)
    {
        if (isInitializingAudioUI) return;

        AudioManager.Instance?.SetMasterVolume(value);
        UpdateVolumeText(masterVolumeValueText, value);
    }

    public void SetMusicVolume(float value)
    {
        if (isInitializingAudioUI) return;

        AudioManager.Instance?.SetMusicVolume(value);
        UpdateVolumeText(musicVolumeValueText, value);
    }

    public void SetAmbientVolume(float value)
    {
        if (isInitializingAudioUI) return;

        AudioManager.Instance?.SetAmbientVolume(value);
        UpdateVolumeText(ambientVolumeValueText, value);
    }

    public void SetSfxVolume(float value)
    {
        if (isInitializingAudioUI) return;

        AudioManager.Instance?.SetSfxVolume(value);
        UpdateVolumeText(sfxVolumeValueText, value);
    }

    public void SetUiVolume(float value)
    {
        if (isInitializingAudioUI) return;

        AudioManager.Instance?.SetUiVolume(value);
        UpdateVolumeText(uiVolumeValueText, value);
    }

    public void ResetAudioSettings()
    {
        AudioManager.Instance?.ResetAudioSettings();
        RefreshAudioSlidersFromManager();
    }

    private void RefreshAllVolumeTexts()
    {
        UpdateVolumeText(masterVolumeValueText, masterVolumeSlider != null ? masterVolumeSlider.value : 0f);
        UpdateVolumeText(musicVolumeValueText, musicVolumeSlider != null ? musicVolumeSlider.value : 0f);
        UpdateVolumeText(ambientVolumeValueText, ambientVolumeSlider != null ? ambientVolumeSlider.value : 0f);
        UpdateVolumeText(sfxVolumeValueText, sfxVolumeSlider != null ? sfxVolumeSlider.value : 0f);
        UpdateVolumeText(uiVolumeValueText, uiVolumeSlider != null ? uiVolumeSlider.value : 0f);
    }

    private void UpdateVolumeText(TMP_Text text, float value)
    {
        if (text == null) return;

        int percentage = Mathf.RoundToInt(Mathf.Clamp01(value) * 100f);
        text.text = percentage + "%";
    }
}
