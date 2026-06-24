using UnityEngine;

public enum MonkSurvivorState
{
    Safe,
    Fleeing,
    ReturningHome,
    Dead
}

[RequireComponent(typeof(Rigidbody2D))]
public class MonkSurvivorAI : MonoBehaviour
{
    public float raggioPericolo = 1.5f;
    public float velocitaFuga = 2.5f;
    public LayerMask enemyLayer;
    public float velocitaRitorno = 2f;
    public float distanzaArrivo = 0.08f;
    public LayerMask obstacleLayer;
    public float obstacleCheckRadius = 0.18f;
    public float obstacleCheckDistance = 0.25f;
    public string walkingParameter = "IsWalking";

    [SerializeField] private MonkSurvivorState currentState = MonkSurvivorState.Safe;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private MonkHealth health;
    private Vector3 puntoDiPartenza;

    public bool IsControllingMovement => currentState == MonkSurvivorState.Fleeing || currentState == MonkSurvivorState.ReturningHome;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = GetComponent<MonkHealth>();
        puntoDiPartenza = transform.position;
    }

    private void FixedUpdate()
    {
        if (MonkMorto())
        {
            currentState = MonkSurvivorState.Dead;
            StopMovement();
            return;
        }

        Transform enemy = FindNearestEnemy();

        if (enemy != null)
        {
            currentState = MonkSurvivorState.Fleeing;
            FleeFrom(enemy);
            return;
        }

        if (currentState == MonkSurvivorState.Fleeing || currentState == MonkSurvivorState.ReturningHome)
        {
            currentState = MonkSurvivorState.ReturningHome;
            ReturnHome();
            return;
        }

        currentState = MonkSurvivorState.Safe;
        StopMovement();
    }

    private bool MonkMorto()
    {
        return health != null && health.isDead;
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, raggioPericolo, enemyLayer);
        Transform best = null;
        float bestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit.isTrigger || !hit.gameObject.activeInHierarchy) continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead || damageable.Team != DamageableTeam.Enemy) continue;

            float distance = Vector2.Distance(transform.position, damageable.TargetTransform.position);
            if (distance >= bestDistance) continue;

            bestDistance = distance;
            best = damageable.TargetTransform;
        }

        return best;
    }

    private void FleeFrom(Transform enemy)
    {
        Vector2 direction = ((Vector2)transform.position - (Vector2)enemy.position).normalized;
        MoveInDirection(direction, velocitaFuga);
    }

    private void ReturnHome()
    {
        Vector2 position = rb.position;
        Vector2 home = puntoDiPartenza;
        Vector2 toHome = home - position;

        if (toHome.magnitude <= distanzaArrivo)
        {
            rb.position = home;
            currentState = MonkSurvivorState.Safe;
            StopMovement();
            return;
        }

        MoveInDirection(toHome.normalized, velocitaRitorno);
    }

    private void MoveInDirection(Vector2 direction, float speed)
    {
        if (direction == Vector2.zero)
        {
            StopMovement();
            return;
        }

        if (DirectionFree(direction))
        {
            ApplyMovement(direction, speed);
            return;
        }

        Vector2 sideA = Vector2.Perpendicular(direction).normalized;
        Vector2 sideB = -sideA;

        if (DirectionFree(sideA))
        {
            ApplyMovement(sideA, speed);
            return;
        }

        if (DirectionFree(sideB))
        {
            ApplyMovement(sideB, speed);
            return;
        }

        StopMovement();
    }

    private void ApplyMovement(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction * speed;
        SetWalking(true);
        Flip(direction);
    }

    private bool DirectionFree(Vector2 direction)
    {
        if (obstacleLayer.value == 0) return true;
        RaycastHit2D hit = Physics2D.CircleCast(rb.position, obstacleCheckRadius, direction, obstacleCheckDistance, obstacleLayer);
        return hit.collider == null;
    }

    private void StopMovement()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
        SetWalking(false);
    }

    private void SetWalking(bool value)
    {
        if (anim != null) anim.SetBool(walkingParameter, value);
    }

    private void Flip(Vector2 direction)
    {
        if (sr == null) return;
        if (direction.x > 0.1f) sr.flipX = false;
        if (direction.x < -0.1f) sr.flipX = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, raggioPericolo);
    }
}
