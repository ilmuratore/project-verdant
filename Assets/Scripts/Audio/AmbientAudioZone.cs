using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AmbientAudioZone : MonoBehaviour
{
    public AudioClip ambientClip;
    [Range(0f, 1f)] public float volumeScale = 0.5f;
    public string playerTag = "Player";

    private AudioSource source;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();

        source.clip = ambientClip;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = volumeScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag) || ambientClip == null) return;
        source.clip = ambientClip;
        source.volume = volumeScale;
        source.Play();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        source.Stop();
    }
}
