using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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

    private bool isInvulnerable = false;
    private void Start()
    {
        if(stats != null)
        {
            currentHealth = stats.vitaMassimaEffettiva;
        }
        UpdateHealthText();

    }

    public void ChangeHealth(int amount)
    {
        if(isInvulnerable && amount < 0)
        {
            return;
        }

        if(amount < 0 && stats != null)
        {
            int dannoSubito = stats.ApplicaDifesa(-amount);
            amount = -dannoSubito;
        }
        currentHealth += amount;
        int max = stats.vitaMassimaEffettiva;
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
        if (healthText != null)
        {
            int max = stats.vitaMassimaEffettiva;
            healthText.text = "HP: " + currentHealth + " / " + max;
        }
    }
}
