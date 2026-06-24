using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    private Animator anim;
    private PlayerMovement playerMovement;
    private GameInput input;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        stats = stats != null ? stats : GetComponent<PlayerStats>();

        if (attackPoint == null)
        {
            Transform foundAttackPoint = transform.Find("AttackPoint");
            if (foundAttackPoint != null) attackPoint = foundAttackPoint;
        }
    }

    private void OnEnable()
    {
        input = GameInput.GetOrCreate();
        input.OnAttack += TryAttack;
    }

    private void OnDisable()
    {
        if (input != null) input.OnAttack -= TryAttack;
    }

    private void TryAttack()
    {
        if (playerMovement == null) return;
        if (playerMovement.controlliBloccato) return;
        if (GameManager.Instance != null && GameManager.Instance.ControlsBlocked) return;
        if (!playerMovement.CanAttack()) return;

        playerMovement.StartAttack();
        anim?.SetTrigger("Attack");
    }

    public void InflictDamage()
    {
        if (attackPoint == null || stats == null || stats.data == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, stats.data.attackRange, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead || damageable.Team != DamageableTeam.Enemy) continue;

            damageable.TakeDamage(stats.AttaccoEffettivo);
        }
    }

    public void EndAttack()
    {
        playerMovement?.EndAttack();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || stats == null || stats.data == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, stats.data.attackRange);
    }
}
