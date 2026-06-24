using UnityEngine;

public class AnimationSoundEvents : MonoBehaviour
{
    public AudioClip playerAttackClip;
    public AudioClip playerDodgeClip;
    public AudioClip playerFootstepClip;
    public AudioClip playerHitClip;
    public AudioClip enemyAttackClip;
    public AudioClip enemyHitClip;
    public AudioClip enemyDeathClip;
    public AudioClip enemyFootstepClip;
    public AudioClip specialClip;
    [Range(0f, 1f)] public float volumeScale = 1f;
    public bool use2DSound = true;

    public void PlayPlayerAttack() => Play(playerAttackClip);
    public void PlayPlayerDodge() => Play(playerDodgeClip);
    public void PlayPlayerFootstep() => Play(playerFootstepClip);
    public void PlayPlayerHit() => Play(playerHitClip);
    public void PlayEnemyAttack() => Play(enemyAttackClip);
    public void PlayEnemyHit() => Play(enemyHitClip);
    public void PlayEnemyDeath() => Play(enemyDeathClip);
    public void PlayEnemyFootstep() => Play(enemyFootstepClip);
    public void PlaySpecial() => Play(specialClip);

    private void Play(AudioClip clip)
    {
        if (clip == null) return;

        AudioManager audioManager = AudioManager.GetOrCreate();

        if (use2DSound)
        {
            audioManager.PlaySfx(clip, volumeScale);
        }
        else
        {
            audioManager.PlaySfxAtPosition(clip, transform.position, volumeScale);
        }
    }
}
