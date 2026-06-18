using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Configurazione (ScriptableObject)")]
    public EnemyStatsData data;

    [Header("Runtime")]
    public int currentHealth;

    public static event Action<EnemyStats> OnAnyEnemyDied;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;

    public int Damage => data != null ? data.damage : 1;
    public int XpReward => data != null ? data.xpReward : 2;

    private void Start()
    {
        int maxHealth = (data != null) ? data.maxHealth : 3;
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        PlayerStats ps = UnityEngine.Object.FindFirstObjectByType<PlayerStats>();
        if (ps != null)
        {
            ps.AddXp(XpReward);
        }

        if (OnAnyEnemyDied != null)
        {
            OnAnyEnemyDied.Invoke(this);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (col != null)
        {
            col.enabled = false;
        }

        Enemy_AI ai = GetComponent<Enemy_AI>();
        if (ai != null)
        {
            ai.EndAttack();
            ai.enabled = false;
        }

        if (anim != null)
        {
            anim.SetTrigger("Destroy");
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