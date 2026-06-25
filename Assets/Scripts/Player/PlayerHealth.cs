using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public PlayerStats stats;

    [SerializeField] private int currentHealth;
    [SerializeField] private bool isInvulnerable;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => stats != null ? stats.vitaMassimaEffettiva : Mathf.Max(1, currentHealth);
    public bool IsDead => currentHealth <= 0;
    public DamageableTeam Team => DamageableTeam.Player;
    public Transform TargetTransform => transform;
    public Collider2D HitCollider { get; private set; }

    private PlayerMovement movement;
    private Animator animator;

    private void Awake()
    {
        stats = stats != null ? stats : GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        HitCollider = GetComponent<Collider2D>();

        if (currentHealth <= 0) currentHealth = MaxHealth;
    }

    private void OnEnable()
    {
        DamageableRegistry.Register(this);
    }

    private void OnDisable()
    {
        DamageableRegistry.Unregister(this);
    }

    private void Start()
    {
        bool applied = PlayerProgressMemory.TryApplyToPlayer(stats, this, transform);

        if (!applied)
        {
            currentHealth = Mathf.Clamp(currentHealth, 1, MaxHealth);
            UIManager.Instance?.UpdatePlayerHealth(currentHealth, MaxHealth);
        }
       
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        ChangeHealth(-amount);
    }

    public void ChangeHealth(int amount)
    {
        if (IsDead) return;
        if (isInvulnerable && amount < 0) return;

        if (amount < 0 && stats != null)
        {
            amount = -stats.ApplicaDifesa(-amount);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
        UIManager.Instance?.UpdatePlayerHealth(currentHealth, MaxHealth);

        if (currentHealth <= 0) Die();
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
        UIManager.Instance?.UpdatePlayerHealth(currentHealth, MaxHealth);
    }

    public void AumentaVitaMassima(int quantita)
    {
        IncreaseMaxHealth(quantita);
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    public void UpdateHealthText()
    {
        UIManager.Instance?.UpdatePlayerHealth(currentHealth, MaxHealth);
    }

    public void ApplySavedHealth(int savedCurrentHealth)
    {
        currentHealth = Mathf.Clamp(savedCurrentHealth, 1, MaxHealth);
        UIManager.Instance?.UpdatePlayerHealth(currentHealth, MaxHealth);
    }

    private void Die()
    {
        currentHealth = 0;
        movement?.SetDead();

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        gameObject.SetActive(false);
    }
}
