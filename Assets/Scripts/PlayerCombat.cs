using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    public PlayerStatsData stats;

    [Header("Attack")]
    public Transform attackPoint;
    public LayerMask enemyLayer;


    private Animator anim;
    private PlayerMovement playerMovement;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (playerMovement == null) return;

        if (!playerMovement.CanAttack())
        {
            return;
        }

        playerMovement.StartAttack();

        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
    }

    public void InflictDamage()
    {
        
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint non assegnato nel PlayerCombat");
            return;
        }
        float range = stats.attackRange;
        int dmg = stats.damage;

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
            attackPoint.position,
            range,
            enemyLayer
        );

        foreach (Collider2D enemy in enemiesHit)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

            if (enemyStats != null)
            {
                enemyStats.TakeDamage(dmg);
            }
        }
    }

    public void EndAttack()
    {
        if (playerMovement != null)
        {
            playerMovement.EndAttack();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        float range = stats.attackRange;
        Gizmos.DrawWireSphere(attackPoint.position, range);
    }
}