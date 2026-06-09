using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attack
}

public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] protected EnemyState currentState = EnemyState.Idle;

    public float attackRange = 0.6f;
    public float speed;
    public Transform player;
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
    }

    void FixedUpdate()
    {
        if (currentState == EnemyState.Chasing)
        {
            Vector2 colliderCenter = (Vector2)transform.position + detectionCollider.offset;
            float distanceToPlayer = Vector2.Distance(colliderCenter, player.position);

            if (distanceToPlayer > detectionCollider.radius)
            {
                currentState = EnemyState.Idle;
                return;
            }

            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
            enemyAnim.SetBool("IsRunning", true);
            enemyAnim.SetBool("Attack", false);
            FlipECollider(direction);

            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attack;
            }
        }
        else if (currentState == EnemyState.Attack)
        {
            rb.linearVelocity = Vector2.zero;
            enemyAnim.SetBool("IsRunning", false);
            enemyAnim.SetBool("Attack", true);

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                enemyAnim.SetBool("Attack", false);
                currentState = EnemyState.Chasing;
            }
        }
        else // Idle
        {
            enemyAnim.SetBool("Attack", false);

            if (player != null)
            {
                Vector2 colliderCenter = (Vector2)transform.position + detectionCollider.offset;
                float distanceToPlayer = Vector2.Distance(colliderCenter, player.position);
                if (distanceToPlayer <= detectionCollider.radius)
                {
                    currentState = EnemyState.Chasing;
                    return;
                }
            }

            ReturnBase();
        }
    }

    private void FlipECollider(Vector2 direction)
    {
        if (direction.x > 0.1f)
        {
            sr.flipX = false;
            detectionCollider.offset = new Vector2(Mathf.Abs(detectionCollider.offset.x), detectionCollider.offset.y);
        }
        else if (direction.x < -0.1f)
        {
            sr.flipX = true;
            detectionCollider.offset = new Vector2(-Mathf.Abs(detectionCollider.offset.x), detectionCollider.offset.y);
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
            FlipECollider(direction);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            detectionCollider.offset = new Vector2(Mathf.Abs(detectionCollider.offset.x), detectionCollider.offset.y);
            enemyAnim.SetBool("IsRunning", false);
            sr.flipX = false;
        }
    }
}