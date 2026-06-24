using System;
using UnityEngine;

public class MonkHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 20;
    public int currentHealth;

    public static event Action<MonkHealth> OnAnyMonkDied;

    public bool isDead { get; private set; }
    public bool IsDead => isDead;
    public DamageableTeam Team => DamageableTeam.Ally;
    public Transform TargetTransform => transform;
    public Collider2D HitCollider { get; private set; }

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        HitCollider = col;
    }

    private void OnEnable()
    {
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

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        isDead = true;

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;
        if (anim != null) anim.SetBool("IsWalking", false);

        OnAnyMonkDied?.Invoke(this);
        gameObject.SetActive(false);
    }
}
