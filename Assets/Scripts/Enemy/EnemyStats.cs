using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    public EnemyStatsData data;
    public int currentHealth;

    public static event Action<EnemyStats> OnAnyEnemyDied;

    [SerializeField] private float destroyDelay = 0.8f;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    private Enemy_AI ai;
    private bool isDead;

    public int Damage => data != null ? data.damage : 1;
    public int XpReward => data != null ? data.xpReward : 2;
    public bool IsDead => isDead;
    public DamageableTeam Team => DamageableTeam.Enemy;
    public Transform TargetTransform => transform;
    public Collider2D HitCollider => col;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ai = GetComponent<Enemy_AI>();
    }

    private void OnEnable()
    {
        int maxHealth = data != null ? data.maxHealth : 3;
        currentHealth = maxHealth;
        isDead = false;
        DamageableRegistry.Register(this);
    }

    private void OnDisable()
    {
        DamageableRegistry.Unregister(this);
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, data != null ? data.maxHealth : currentHealth);

        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Hit");
        }

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null) playerStats.AddXp(XpReward);

        OnAnyEnemyDied?.Invoke(this);

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;
        if (ai != null) ai.enabled = false;

        if (anim != null)
        {
            anim.SetTrigger("Destroy");
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
