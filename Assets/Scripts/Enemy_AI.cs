using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attack
}

public class Enemy_AI : MonoBehaviour
{
    [SerializeField] protected EnemyState currentState = EnemyState.Idle;

    public float attackRange = 0.6f;
    public float speed;
    public Transform player;
    public int damage = 1;

    private Rigidbody2D rb;
    private Animator enemyAnim;
    private SpriteRenderer sr;
    private CircleCollider2D detectionCollider;

    private Vector3 posizioneIniziale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        detectionCollider = GetComponent<CircleCollider2D>();

        posizioneIniziale = transform.position;

        TransizioneA(EnemyState.Idle);
    }

    void FixedUpdate()
    {
        if (!PlayerValido())
        {
            VaiInIdle();
            return;
        }

        switch (currentState)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Chasing: UpdateChasing(); break;
            case EnemyState.Attack: UpdateAttack(); break;
        }
    }


    private void UpdateIdle()
    {
        SetRunning(false);

        if (GetDistanceToDetection() <= GetDetectionRadius())
        {
            TransizioneA(EnemyState.Chasing);
            return;
        }

        ReturnBase();
    }

    private void UpdateChasing()
    {
        if (GetDistanceToDetection() > GetDetectionRadius())
        {
            TransizioneA(EnemyState.Idle);
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = direction * speed;
        SetRunning(true);
        FlipECollider(direction);

        if (GetDistanceToPlayer() <= attackRange)
        {
            TransizioneA(EnemyState.Attack);
        }
    }

    private void UpdateAttack()
    {
        rb.linearVelocity = Vector2.zero;
        SetRunning(false);

        Vector2 direction = (player.position - transform.position).normalized;
        FlipECollider(direction);


    }


    private void TransizioneA(EnemyState nuovo)
    {
        currentState = nuovo;

        if (nuovo == EnemyState.Idle || nuovo == EnemyState.Attack)
            rb.linearVelocity = Vector2.zero;

        if (nuovo == EnemyState.Attack)
        {
            SetRunning(false);
            enemyAnim.SetTrigger("Attack");
        }
        else if (nuovo == EnemyState.Idle)
        {
            SetRunning(false);
        }
    }

    private void VaiInIdle()
    {
        rb.linearVelocity = Vector2.zero;
        SetRunning(false);
        currentState = EnemyState.Idle;
    }

    private void SetRunning(bool running)
    {
        enemyAnim.SetBool("IsRunning", running);
    }

    private bool PlayerValido()
    {
        return player != null && player.gameObject.activeInHierarchy;
    }


    private float GetDistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    private float GetDistanceToDetection()
    {
        Vector2 colliderCenter = (Vector2)transform.position + detectionCollider.offset;
        return Vector2.Distance(colliderCenter, player.position);
    }

    private float GetDetectionRadius()
    {
        return detectionCollider.radius * Mathf.Max(
            Mathf.Abs(transform.lossyScale.x),
            Mathf.Abs(transform.lossyScale.y)
        );
    }


    private void FlipECollider(Vector2 direction)
    {
        if (direction.x > 0.1f)
        {
            sr.flipX = false;
            detectionCollider.offset = new Vector2(
                Mathf.Abs(detectionCollider.offset.x),
                detectionCollider.offset.y
            );
        }
        else if (direction.x < -0.1f)
        {
            sr.flipX = true;
            detectionCollider.offset = new Vector2(
                -Mathf.Abs(detectionCollider.offset.x),
                detectionCollider.offset.y
            );
        }
    }


    private void ReturnBase()
    {
        float distanza = Vector2.Distance(transform.position, posizioneIniziale);

        if (distanza > 0.1f)
        {
            Vector2 direction = (posizioneIniziale - transform.position).normalized;

            rb.linearVelocity = direction * speed;
            SetRunning(true);
            FlipECollider(direction);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            SetRunning(false);

            detectionCollider.offset = new Vector2(
                Mathf.Abs(detectionCollider.offset.x),
                detectionCollider.offset.y
            );
            sr.flipX = false;
        }
    }


    public void InflictDamage()
    {
        if (!CanInflictDamage())
            return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        if (playerHealth.currentHealth <= 0)
            return;

        playerHealth.ChangeHealth(-damage);
    }
    public void EndAttack()
    {
        if (!PlayerValido())
        {
            currentState = EnemyState.Idle;
            return;
        }

        if (GetDistanceToPlayer() <= attackRange)
        {
            TransizioneA(EnemyState.Attack);
        }
        else if (GetDistanceToDetection() <= GetDetectionRadius())
        {
            TransizioneA(EnemyState.Chasing);
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    private bool CanInflictDamage()
    {
        if (!PlayerValido())
            return false;

        if (currentState != EnemyState.Attack)
            return false;

        if (GetDistanceToPlayer() > attackRange)
            return false;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return false;

        if (playerHealth.currentHealth <= 0)
            return false;

        return true;
    }
}