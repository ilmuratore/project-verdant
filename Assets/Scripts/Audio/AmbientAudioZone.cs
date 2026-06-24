using UnityEngine;

public class AmbientAudioZone : MonoBehaviour
{
    [Header("Audio ambientale zona")]
    public AudioClip ambientClip;

    [Range(0f, 1f)] public float volumeScale = 0.5f;

    [Header("Target")]
    public string playerTag = "Player";

    private AudioSource source;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        source = gameObject.AddComponent<AudioSource>();
        source.clip = ambientClip;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = volumeScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (ambientClip == null) return;
        if(source.clip != ambientClip)
        {
            source.clip = ambientClip;
        }
        source.volume = volumeScale;
        source.Play();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        source.Stop();
    }
}
