using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public TMP_Text healthText;
    public Animator healthTextAnim;


    private bool isInvulnerable = false;

    private void Start()
    {
        UpdateHealthText();

    }

    public void ChangeHealth(int amount )
    {
        if(isInvulnerable && amount < 0)
        {
            return;
        }
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
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
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
        }
    }
}
