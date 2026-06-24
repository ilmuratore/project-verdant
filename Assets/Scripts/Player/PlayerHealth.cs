using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public PlayerStats stats;

    [Header("Runtime")]
    public int currentHealth;

    [Header("UI")]
    public TMP_Text healthText;
    public Animator healthTextAnim;

    [Header("Nomi UI nella scena")]
    [SerializeField] private string healthTextObjectName = "HealthText";
    [SerializeField] private string fallbackHealthTextObjectName = "Player Health";
    [SerializeField] private string healthTextAnimatorObjectName = "HealthText";

    private bool isInvulnerable = false;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        ResolveReferences();

        if (stats != null && currentHealth <= 0)
        {
            currentHealth = stats.vitaMassimaEffettiva;
        }

        UpdateHealthText();
    }

    private void ResolveReferences()
    {
        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
        }

        healthText = SceneReferenceFinder.ResolveComponentInChildren(healthText, null, healthTextObjectName);

        if (healthText == null && !string.IsNullOrWhiteSpace(fallbackHealthTextObjectName))
        {
            healthText = SceneReferenceFinder.ResolveComponentInChildren(healthText, null, fallbackHealthTextObjectName);
        }

        healthTextAnim = SceneReferenceFinder.ResolveComponentInChildren(healthTextAnim, null, healthTextAnimatorObjectName);

        if (healthTextAnim == null && healthText != null)
        {
            healthTextAnim = healthText.GetComponent<Animator>();
        }
    }

    public void ChangeHealth(int amount)
    {
        ResolveReferences();

        if (isInvulnerable && amount < 0)
        {
            return;
        }

        if (amount < 0 && stats != null)
        {
            int dannoSubito = stats.ApplicaDifesa(-amount);
            amount = -dannoSubito;
        }

        currentHealth += amount;

        int max = stats != null ? stats.vitaMassimaEffettiva : Mathf.Max(1, currentHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, max);

        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHealth(currentHealth, max);
        }

        if (healthTextAnim != null)
        {
            healthTextAnim.Play("TextAnimation");
        }

        UpdateHealthText();

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void AumentaVitaMassima(int quantita)
    {
        currentHealth += quantita;
        UpdateHealthText();
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    public void UpdateHealthText()
    {
        ResolveReferences();

        int max = stats != null ? stats.vitaMassimaEffettiva : Mathf.Max(1, currentHealth);

        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHealth(currentHealth, max);
        }

        if (healthText == null) return;

        healthText.text = "HP: " + currentHealth + " / " + max;
    }
}
