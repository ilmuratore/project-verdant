using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "RPG/Player Stats")]
public class PlayerStatsData : ScriptableObject
{
    [Header("Valori di base (Livello 1")]
    public int attaccoBase = 1;
    public int difesaBase = 1;
    public int vitaBase = 10;

    [Header("Incrementali per Lv.")]
    public int incrementoAttaco = 1;
    public int incrementoDifesa = 1;
    public int incrementoVita = 2;

    [Header("Progressione")]
    public int xpBaseLevelUp = 5;
    public float xpCrescita = 1.5f;
    public int puntiPerLivello = 1;

    [Header("Movimento")]
    public float velocita = 3.2f;
    public float dodgeSpeed = 13f;
    public float dodgeDuration = 0.3f;
    public float dodgeCooldown = 0.8f;

    [Header("Combattimento")]
    public int damage = 1;
    public float attackRange = 1f;


}
