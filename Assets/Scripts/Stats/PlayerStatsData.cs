using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "RPG/Player Stats")]
public class PlayerStatsData : ScriptableObject
{

    [Header("Health")]
    public int maxHealth = 5;

    [Header("Movement")]
    public float velocita = 5f;

    [Header("Dodge")]
    public float dodgeSpeed = 12f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 0.7f;


    [Header("Combat")]
    public int damage = 1;
    public float attackRange = 1f;

}
