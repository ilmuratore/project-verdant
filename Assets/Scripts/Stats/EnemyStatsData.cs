using UnityEngine;


[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "RPG/Enemy Stats")]
public class EnemyStatsData : ScriptableObject
{
    [Header("Identità")]
    public string nomeNemico = "Nemico";

    [Header("Vita")]
    public int maxHealth = 3;

    [Header("Combattimento")]
    public int damage = 1;
    public float attackRange = 0.6f;

    [Header("Movimento")]
    public float speed = 2f;

    [Header("Ricompensa")]
    public int xpReward = 2;
}

