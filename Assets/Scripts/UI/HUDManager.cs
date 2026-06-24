using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
   public static HUDManager Instance { get; private set; }

    [Header("Player UI")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text xpText;

    [Header("Quest UI")]
    [SerializeField] private TMP_Text questTitleText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private TMP_Text questCounterText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if(healthText != null)
        {
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
        }
    }


}
