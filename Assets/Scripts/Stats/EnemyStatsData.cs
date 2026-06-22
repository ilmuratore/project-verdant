using UnityEngine;


[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "RPG/Enemy Stats")]
public class EnemyStatsData : ScriptableObject
{
    [Header("Identità")]
    public string nomeNemico = "Nemico";

    [Header("Vita")]
    public int maxHealth = 5;

    [Header("Combattimento")]
    public int damage = 1;
    public float attackRange = 1f;

    [Header("Movimento")]
    public float speed = 2.8f;

    [Header("Ricompensa")]
    public int xpReward = 3;
}

