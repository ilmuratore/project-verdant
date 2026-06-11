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

        currentState = EnemyState.Idle;
        rb.linearVelocity = Vector2.zero;
        enemyAnim.SetBool("IsRunning", false);
        enemyAnim.SetBool("Attack", false);
    }

    void FixedUpdate()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            rb.linearVelocity = Vector2.zero;
            enemyAnim.SetBool("IsRunning", false);
            enemyAnim.SetBool("Attack", false);
            currentState = EnemyState.Idle;
            return;
        }

        if (currentState == EnemyState.Chasing)
        {
            float distanceToDetection = GetDistanceToDetection();

            if (distanceToDetection > GetDetectionRadius())
            {
                currentState = EnemyState.Idle;
                return;
            }

            Vector2 direction = (player.position - transform.position).normalized;

            rb.linearVelocity = direction * speed;

            enemyAnim.SetBool("IsRunning", true);
            enemyAnim.SetBool("Attack", false);

            FlipECollider(direction);

            float distanceToPlayer = GetDistanceToPlayer();

            if (distanceToPlayer <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;

                enemyAnim.SetBool("IsRunning", false);
                enemyAnim.SetBool("Attack", true);

                currentState = EnemyState.Attack;
            }
        }
        else if (currentState == EnemyState.Attack)
        {
            rb.linearVelocity = Vector2.zero;

            enemyAnim.SetBool("IsRunning", false);

            Vector2 direction = (player.position - transform.position).normalized;
            FlipECollider(direction);

            float distanceToPlayer = GetDistanceToPlayer();

            if (distanceToPlayer > attackRange)
            {
                enemyAnim.SetBool("Attack", false);
                currentState = EnemyState.Chasing;
            }
        }
        else // Idle
        {
            enemyAnim.SetBool("Attack", false);

            float distanceToDetection = GetDistanceToDetection();

            if (distanceToDetection <= GetDetectionRadius())
            {
                currentState = EnemyState.Chasing;
                return;
            }

            ReturnBase();
        }
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

            enemyAnim.SetBool("IsRunning", true);
            enemyAnim.SetBool("Attack", false);

            FlipECollider(direction);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            enemyAnim.SetBool("IsRunning", false);
            enemyAnim.SetBool("Attack", false);

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
        enemyAnim.SetBool("Attack", false);

        if (player == null || !player.gameObject.activeInHierarchy)
        {
            currentState = EnemyState.Idle;
            return;
        }

        float distanceToDetection = GetDistanceToDetection();

        if (distanceToDetection <= GetDetectionRadius())
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    private bool CanInflictDamage()
    {
        if (player == null)
            return false;

        if (!player.gameObject.activeInHierarchy)
            return false;

        if (currentState != EnemyState.Attack)
            return false;

        float distanceToPlayer = GetDistanceToPlayer();

        if (distanceToPlayer > attackRange)
            return false;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return false;

        if (playerHealth.currentHealth <= 0)
            return false;

        return true;
    }
}