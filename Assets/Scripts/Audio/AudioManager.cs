using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string MasterVolumeKey = "Audio_MasterVolume";
    private const string MusicVolumeKey = "Audio_MusicVolume";
    private const string AmbientVolumeKey = "Audio_AmbientVolume";
    private const string SfxVolumeKey = "Audio_SfxVolume";
    private const string UiVolumeKey = "Audio_UiVolume";
    private const float DefaultMasterVolume = 1f;
    private const float DefaultMusicVolume = 0.6f;
    private const float DefaultAmbientVolume = 0.45f;
    private const float DefaultSfxVolume = 0.85f;
    private const float DefaultUiVolume = 0.8f;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [Range(0f, 1f)] public float masterVolume = DefaultMasterVolume;
    [Range(0f, 1f)] public float musicVolume = DefaultMusicVolume;
    [Range(0f, 1f)] public float ambientVolume = DefaultAmbientVolume;
    [Range(0f, 1f)] public float sfxVolume = DefaultSfxVolume;
    [Range(0f, 1f)] public float uiVolume = DefaultUiVolume;

    public AudioClip defaultMusic;
    public AudioClip defaultAmbient;
    public bool playDefaultMusicOnStart = true;
    public bool playDefaultAmbientOnStart = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureSources();
        LoadAudioSettings();
        ApplyVolumes();
    }

    private void Start()
    {
        if (playDefaultMusicOnStart && defaultMusic != null) PlayMusic(defaultMusic);
        if (playDefaultAmbientOnStart && defaultAmbient != null) PlayAmbient(defaultAmbient);
    }

    public static AudioManager GetOrCreate()
    {
        if (Instance != null) return Instance;

        AudioManager existing = FindFirstObjectByType<AudioManager>();
        if (existing != null) return existing;

        GameObject go = new GameObject("AudioManager");
        return go.AddComponent<AudioManager>();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        EnsureSources();

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null) return;
        EnsureSources();

        if (ambientSource.clip == clip && ambientSource.isPlaying) return;

        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        if (ambientSource != null) ambientSource.Stop();
    }

    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        EnsureSources();
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    public void PlayUi(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        EnsureSources();
        uiSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    public void PlaySfxAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip == null) return;
        float finalVolume = masterVolume * sfxVolume * Mathf.Clamp01(volumeScale);
        AudioSource.PlayClipAtPoint(clip, position, finalVolume);
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        SaveAudioSettings();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        SaveAudioSettings();
    }

    public void SetAmbientVolume(float value)
    {
        ambientVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        SaveAudioSettings();
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        SaveAudioSettings();
    }

    public void SetUiVolume(float value)
    {
        uiVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        SaveAudioSettings();
    }

    public void ResetAudioSettings()
    {
        masterVolume = DefaultMasterVolume;
        musicVolume = DefaultMusicVolume;
        ambientVolume = DefaultAmbientVolume;
        sfxVolume = DefaultSfxVolume;
        uiVolume = DefaultUiVolume;
        ApplyVolumes();
        SaveAudioSettings();
    }

    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume);
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
        ambientVolume = PlayerPrefs.GetFloat(AmbientVolumeKey, DefaultAmbientVolume);
        sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, DefaultSfxVolume);
        uiVolume = PlayerPrefs.GetFloat(UiVolumeKey, DefaultUiVolume);
    }

    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(AmbientVolumeKey, ambientVolume);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.SetFloat(UiVolumeKey, uiVolume);
        PlayerPrefs.Save();
    }

    private void EnsureSources()
    {
        if (musicSource == null) musicSource = CreateSource("MusicSource", true);
        if (ambientSource == null) ambientSource = CreateSource("AmbientSource", true);
        if (sfxSource == null) sfxSource = CreateSource("SfxSource", false);
        if (uiSource == null) uiSource = CreateSource("UiSource", false);
    }

    private AudioSource CreateSource(string sourceName, bool loop)
    {
        GameObject sourceObject = new GameObject(sourceName);
        sourceObject.transform.SetParent(transform);

        AudioSource source = sourceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        source.spatialBlend = 0f;
        return source;
    }

    private void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = masterVolume * musicVolume;
        if (ambientSource != null) ambientSource.volume = masterVolume * ambientVolume;
        if (sfxSource != null) sfxSource.volume = masterVolume * sfxVolume;
        if (uiSource != null) uiSource.volume = masterVolume * uiVolume;
    }
}
