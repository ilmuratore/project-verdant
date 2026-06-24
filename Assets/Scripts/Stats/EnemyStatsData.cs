using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "RPG/Enemy Stats")]
public class EnemyStatsData : ScriptableObject
{
    public string nomeNemico = "Nemico";
    public int maxHealth = 5;
    public int damage = 1;
    public float attackRange = 1f;
    public float speed = 2.8f;
    public int xpReward = 3;
}
