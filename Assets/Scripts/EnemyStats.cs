using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Combat")]
    public int damage = 1;

    [Header("Reward")]
    public int xpReward = 2;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;

    private void Start()
    {
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

        PlayerStats ps = Object.FindFirstObjectByType<PlayerStats>();
        if (ps != null)
        {
            ps.AddXp(xpReward);
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