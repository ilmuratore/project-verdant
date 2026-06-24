using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Volume")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float ambientVolume = 0.45f;
    [Range(0f, 1f)] public float sfxVolume = 0.60f;
    [Range(0f, 1f)] public float uiVolume = 0.6f;

    [Header("Avvio")]
    public AudioClip defaultMusic;
    public AudioClip defaultAmbient;

    public bool playDefaultMusicOnStart = true;
    public bool playDefaultAmbientOnStart = true;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureSources();
        ApplyVolumes();

    }


    private void Start()
    {
       if(playDefaultMusicOnStart && defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }
       if(playDefaultAmbientOnStart && defaultAmbient != null)
        {
            PlayAmbient(defaultAmbient);
        }
    }


    public static AudioManager GetOrCreate()
    {
        if (Instance != null) return Instance;
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
        if(musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null) return;

        EnsureSources();

        if (musicSource.clip == clip && ambientSource.isPlaying) return;
        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }
    public void StopAmbient()
    {
        if (ambientSource != null)
        {
            ambientSource.Stop();
        }
    }

    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        EnsureSources();

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));

    }


    public void PlaySfxAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, masterVolume * sfxVolume * Mathf.Clamp01(volumeScale));
    }

    public void PlayUi(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        EnsureSources();
        uiSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolumes();

    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolumes();

    }

    public void SetAmbientVolume(float value)
    {
        ambientVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolumes(); 
    }

    public void SetUiVolume(float value)
    {
        uiVolume = Mathf.Clamp01(value);
        ApplyVolumes();
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
