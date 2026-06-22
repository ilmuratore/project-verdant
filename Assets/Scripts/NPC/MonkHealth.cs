using System;
using UnityEngine;

public class MonkHealth : MonoBehaviour
{
    [Header("Vita Monaco")]
    public int maxHealth = 20;

    [Header("Runtime")]
    public int currentHealth;

    public static event Action<MonkHealth> OnAnyMonkDied;

    public bool isDead { get; private set; }

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (amount <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        isDead = true;
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if(col != null)
        {
            col.enabled = false;
        }
        if(anim != null)
        {
            anim.SetBool("IsWalking", false);
        }
        OnAnyMonkDied?.Invoke(this);
        gameObject.SetActive(false);
    }
}
