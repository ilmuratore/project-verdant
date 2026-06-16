using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [Header("Stats")]
    public PlayerStatsData stats;

    [Header("Runtime")]
    public int currentHealth;

    [Header("UI")]
    public TMP_Text healthText;
    public Animator healthTextAnim;

    private bool isInvulnerable = false;
    private void Start()
    {
        if(stats != null)
        {
            currentHealth = stats.maxHealth;
        }
        UpdateHealthText();

    }

    public void ChangeHealth(int amount )
    {
        if(isInvulnerable && amount < 0)
        {
            return;
        }
        currentHealth += amount;
        int max = stats.maxHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, max);
        if(healthTextAnim != null)
        {
            healthTextAnim.Play("TextAnimation");
        }

        UpdateHealthText();

        if ( currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    
    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    public void UpdateHealthText()
    {
        if (healthText != null)
        {
            int max = stats.maxHealth;
            healthText.text = "HP: " + currentHealth + " / " + max;
        }
    }
}
