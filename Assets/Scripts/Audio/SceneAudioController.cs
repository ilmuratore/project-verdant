using UnityEngine;

public class SceneAudioController : MonoBehaviour
{
    public AudioClip musicClip;
    public AudioClip ambientClip;
    public bool playMusicOnStart = true;
    public bool playAmbientOnStart = true;

    private void Start()
    {
        AudioManager audioManager = AudioManager.GetOrCreate();
        if (playMusicOnStart && musicClip != null) audioManager.PlayMusic(musicClip);
        if (playAmbientOnStart && ambientClip != null) audioManager.PlayAmbient(ambientClip);
    }
}
