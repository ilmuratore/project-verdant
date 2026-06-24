using UnityEngine;

public class SceneAudioController : MonoBehaviour
{
    [Header("Audio scena")]
    public AudioClip musicClip;
    public AudioClip ambientClip;

    [Header("Options")]
    public bool playMusicOnStart = true;
    public bool playAmbientOnStart = true;


    private void Start() {
        AudioManager audioManager = AudioManager.GetOrCreate();

        if(playMusicOnStart && musicClip != null)
        {
            audioManager.PlayMusic(musicClip);
        }
        if(playAmbientOnStart && ambientClip != null)
        {
            audioManager.PlayAmbient(ambientClip);
        }
    }
}
