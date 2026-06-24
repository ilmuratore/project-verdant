using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attack
}

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_AI : MonoBehaviour
{
    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    [SerializeField] private float targetSearchInterval = 0.25f;
    [SerializeField] private float aggroRadius = 30f;

    private IDamageable currentTarget;
    private float targetSearchTimer;
    private float attackRange = 0.6f;
    private float speed = 2f;
    private int damage = 1;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private SpriteRenderer sr;
    private CircleCollider2D detectionCollider;
    private Collider2D ownCollider;
    private EnemyStats stats;
    private Vector3 startPosition;
    private bool attackCycleRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        detectionCollider = GetComponent<CircleCollider2D>();
        ownCollider = GetComponent<Collider2D>();
        stats = GetComponent<EnemyStats>();
        startPosition = transform.position;
    }

    private void Start()
    {
        if (stats != null && stats.data != null)
        {
            attackRange = stats.data.attackRange;
            speed = stats.data.speed;
            damage = stats.data.damage;
        }

        currentTarget = FindNearestTarget();
        SetState(EnemyState.Idle);
    }

    private void FixedUpdate()
    {
        UpdateTargetIfNeeded();

        if (!TargetValid())
        {
            SetState(EnemyState.Idle);
            ReturnBase();
            return;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Chasing:
                UpdateChasing();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
        }
    }

    private void UpdateIdle()
    {
        SetRunning(false);

        if (TargetValid())
        {
            SetState(EnemyState.Chasing);
            return;
        }

        ReturnBase();
    }

    private void UpdateChasing()
    {
        if (!TargetValid())
        {
            SetState(EnemyState.Idle);
            return;
        }

        if (TargetInAttackRange())
        {
            StartAttackCycle();
            return;
        }

        Vector2 direction = ((Vector2)currentTarget.TargetTransform.position - rb.position).normalized;
        rb.linearVelocity = direction * speed;
        SetRunning(true);
        FlipAndCollider(direction);
    }

    private void UpdateAttack()
    {
        rb.linearVelocity = Vector2.zero;
        SetRunning(false);

        if (!TargetValid())
        {
            attackCycleRunning = false;
            SetState(EnemyState.Idle);
            return;
        }

        if (!TargetInAttackRange())
        {
            attackCycleRunning = false;
            SetState(EnemyState.Chasing);
            return;
        }

        Vector2 direction = ((Vector2)currentTarget.TargetTransform.position - rb.position).normalized;
        FlipAndCollider(direction);
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == newState && newState != EnemyState.Attack) return;

        currentState = newState;

        if (newState == EnemyState.Idle)
        {
            attackCycleRunning = false;
            rb.linearVelocity = Vector2.zero;
            SetRunning(false);
        }

        if (newState == EnemyState.Chasing)
        {
            attackCycleRunning = false;
        }
    }

    private void StartAttackCycle()
    {
        if (attackCycleRunning) return;

        currentState = EnemyState.Attack;
        attackCycleRunning = true;
        rb.linearVelocity = Vector2.zero;
        SetRunning(false);
        enemyAnim?.SetTrigger("Attack");
    }

    private void UpdateTargetIfNeeded()
    {
        targetSearchTimer += Time.fixedDeltaTime;

        if (targetSearchTimer < targetSearchInterval && TargetValid()) return;

        targetSearchTimer = 0f;
        currentTarget = FindNearestTarget();
    }

    private IDamageable FindNearestTarget()
    {
        DamageableRegistry.Cleanup();

        IDamageable best = null;
        float bestDistance = Mathf.Infinity;

        foreach (IDamageable target in DamageableRegistry.All)
        {
            if (target == null || target.IsDead) continue;
            if (target.Team != DamageableTeam.Player && target.Team != DamageableTeam.Ally) continue;
            if (target.TargetTransform == null) continue;

            float distance = Vector2.Distance(transform.position, target.TargetTransform.position);
            if (distance > aggroRadius || distance >= bestDistance) continue;

            bestDistance = distance;
            best = target;
        }

        return best;
    }

    private bool TargetValid()
    {
        if (currentTarget == null || currentTarget.IsDead || currentTarget.TargetTransform == null) return false;
        if (!currentTarget.TargetTransform.gameObject.activeInHierarchy) return false;
        return Vector2.Distance(transform.position, currentTarget.TargetTransform.position) <= aggroRadius;
    }

    private bool TargetInAttackRange()
    {
        return GetDistanceToTarget() <= attackRange;
    }

    private float GetDistanceToTarget()
    {
        if (!TargetValid()) return Mathf.Infinity;

        Collider2D targetCollider = currentTarget.HitCollider;

        if (ownCollider != null && targetCollider != null && ownCollider.enabled && targetCollider.enabled)
        {
            ColliderDistance2D distance = ownCollider.Distance(targetCollider);
            return Mathf.Max(0f, distance.distance);
        }

        if (targetCollider != null && targetCollider.enabled)
        {
            Vector2 closestPoint = targetCollider.ClosestPoint(transform.position);
            return Vector2.Distance(transform.position, closestPoint);
        }

        return Vector2.Distance(transform.position, currentTarget.TargetTransform.position);
    }

    private void SetRunning(bool running)
    {
        if (enemyAnim != null) enemyAnim.SetBool("IsRunning", running);
    }

    private void FlipAndCollider(Vector2 direction)
    {
        if (sr == null) return;

        if (direction.x > 0.1f)
        {
            sr.flipX = false;
            FlipDetectionCollider(false);
        }
        else if (direction.x < -0.1f)
        {
            sr.flipX = true;
            FlipDetectionCollider(true);
        }
    }

    private void FlipDetectionCollider(bool left)
    {
        if (detectionCollider == null) return;
        float x = Mathf.Abs(detectionCollider.offset.x);
        detectionCollider.offset = new Vector2(left ? -x : x, detectionCollider.offset.y);
    }

    private void ReturnBase()
    {
        float distance = Vector2.Distance(transform.position, startPosition);

        if (distance > 0.1f)
        {
            Vector2 direction = ((Vector2)startPosition - rb.position).normalized;
            rb.linearVelocity = direction * speed;
            SetRunning(true);
            FlipAndCollider(direction);
            return;
        }

        rb.linearVelocity = Vector2.zero;
        SetRunning(false);

        if (detectionCollider != null)
        {
            detectionCollider.offset = new Vector2(Mathf.Abs(detectionCollider.offset.x), detectionCollider.offset.y);
        }

        if (sr != null) sr.flipX = false;
    }

    public void InflictDamage()
    {
        if (!CanInflictDamage()) return;
        currentTarget.TakeDamage(damage);
    }

    public void EndAttack()
    {
        attackCycleRunning = false;
        UpdateTargetIfNeeded();

        if (!TargetValid())
        {
            SetState(EnemyState.Idle);
            return;
        }

        if (TargetInAttackRange())
        {
            StartAttackCycle();
        }
        else
        {
            SetState(EnemyState.Chasing);
        }
    }

    private bool CanInflictDamage()
    {
        return currentState == EnemyState.Attack && TargetValid() && TargetInAttackRange();
    }
}
