using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "RPG/Player Stats")]
public class PlayerStatsData : ScriptableObject
{
    public int attaccoBase = 1;
    public int difesaBase = 1;
    public int vitaBase = 10;
    public int incrementoAttaco = 1;
    public int incrementoDifesa = 1;
    public int incrementoVita = 2;
    public int xpBaseLevelUp = 5;
    public float xpCrescita = 1.5f;
    public int puntiPerLivello = 1;
    public float velocita = 3.2f;
    public float dodgeSpeed = 13f;
    public float dodgeDuration = 0.3f;
    public float dodgeCooldown = 0.8f;
    public int damage = 1;
    public float attackRange = 1f;
}
