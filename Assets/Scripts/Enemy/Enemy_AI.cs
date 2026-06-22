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

    [Header("Tag dei target")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string npcTag = "NPC";

    [Header("Target dinamici")]
    [Tooltip("Ogni quanto il nemico ricalcola il bersaglio più vicino tra Player e NPC.")]
    public float intervalloRicercaTarget = 0.25f;

    [Tooltip("Distanza massima entro cui il nemico cerca un bersaglio. Per questa quest conviene 30 o 40.")]
    public float raggioAggro = 30f;

    private Transform currentTarget;
    private float timerRicercaTarget;

    private float attackRange = 0.6f;
    private float speed = 2f;
    private int damage = 1;

    private Rigidbody2D rb;
    private Animator enemyAnim;
    private SpriteRenderer sr;
    private CircleCollider2D detectionCollider;
    private EnemyStats stats;

    private Vector3 posizioneIniziale;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        detectionCollider = GetComponent<CircleCollider2D>();
        stats = GetComponent<EnemyStats>();

        if (stats != null && stats.data != null)
        {
            attackRange = stats.data.attackRange;
            speed = stats.data.speed;
            damage = stats.data.damage;
        }

        posizioneIniziale = transform.position;
        currentTarget = TrovaTargetPiuVicino();

        TransizioneA(EnemyState.Idle);
    }

    private void FixedUpdate()
    {
        AggiornaTargetSeNecessario();

        if (!TargetValido())
        {
            VaiInIdle();
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

        if (TargetValido())
        {
            TransizioneA(EnemyState.Chasing);
            return;
        }

        ReturnBase();
    }

    private void UpdateChasing()
    {
        if (!TargetValido())
        {
            TransizioneA(EnemyState.Idle);
            return;
        }

        if (TargetInAttackRange())
        {
            TransizioneA(EnemyState.Attack);
            return;
        }

        Vector2 direction = (currentTarget.position - transform.position).normalized;

        rb.linearVelocity = direction * speed;
        SetRunning(true);
        FlipECollider(direction);
    }

    private void UpdateAttack()
    {
        rb.linearVelocity = Vector2.zero;
        SetRunning(false);

        if (!TargetValido())
        {
            TransizioneA(EnemyState.Idle);
            return;
        }

        if (!TargetInAttackRange())
        {
            TransizioneA(EnemyState.Chasing);
            return;
        }

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        FlipECollider(direction);
    }

    private void TransizioneA(EnemyState nuovo)
    {
        currentState = nuovo;

        if (nuovo == EnemyState.Idle || nuovo == EnemyState.Attack)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (nuovo == EnemyState.Attack)
        {
            SetRunning(false);

            if (enemyAnim != null)
            {
                enemyAnim.SetTrigger("Attack");
            }
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
        if (enemyAnim != null)
        {
            enemyAnim.SetBool("IsRunning", running);
        }
    }

    private void AggiornaTargetSeNecessario()
    {
        timerRicercaTarget += Time.fixedDeltaTime;

        if (timerRicercaTarget < intervalloRicercaTarget && TargetValido())
        {
            return;
        }

        timerRicercaTarget = 0f;
        currentTarget = TrovaTargetPiuVicino();
    }

    private Transform TrovaTargetPiuVicino()
    {
        Transform migliore = null;
        float distanzaMigliore = Mathf.Infinity;

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        ValutaTarget(playerObj, ref migliore, ref distanzaMigliore);

        GameObject[] npcObjects = GameObject.FindGameObjectsWithTag(npcTag);

        foreach (GameObject npc in npcObjects)
        {
            ValutaTarget(npc, ref migliore, ref distanzaMigliore);
        }

        return migliore;
    }

    private void ValutaTarget(GameObject targetObj, ref Transform migliore, ref float distanzaMigliore)
    {
        if (targetObj == null) return;
        if (!targetObj.activeInHierarchy) return;

        Transform targetTransform = targetObj.transform;

        if (!TargetVivo(targetTransform)) return;

        float distanza = Vector2.Distance(transform.position, targetTransform.position);

        if (distanza > raggioAggro) return;

        if (distanza < distanzaMigliore)
        {
            distanzaMigliore = distanza;
            migliore = targetTransform;
        }
    }

    private bool TargetValido()
    {
        if (currentTarget == null) return false;
        if (!currentTarget.gameObject.activeInHierarchy) return false;
        if (!TargetVivo(currentTarget)) return false;

        float distanza = Vector2.Distance(transform.position, currentTarget.position);
        return distanza <= raggioAggro;
    }

    private bool TargetVivo(Transform target)
    {
        if (target == null) return false;

        if (target.CompareTag(playerTag))
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            return playerHealth != null && playerHealth.currentHealth > 0;
        }

        if (target.CompareTag(npcTag))
        {
            MonkHealth monkHealth = target.GetComponent<MonkHealth>();
            return monkHealth != null && !monkHealth.isDead && monkHealth.currentHealth > 0;
        }

        return false;
    }

    private float GetDistanceToTarget()
    {
        if (!TargetValido()) return Mathf.Infinity;

        Collider2D targetCollider = GetBestTargetCollider();

        if (targetCollider != null)
        {
            Vector2 closestPoint = targetCollider.ClosestPoint(transform.position);
            return Vector2.Distance(transform.position, closestPoint);
        }

        return Vector2.Distance(transform.position, currentTarget.position);
    }

    private Collider2D GetBestTargetCollider()
    {
        Collider2D[] colliders = currentTarget.GetComponents<Collider2D>();

        foreach (Collider2D col in colliders)
        {
            if (col != null && col.enabled && !col.isTrigger)
            {
                return col;
            }
        }

        foreach (Collider2D col in colliders)
        {
            if (col != null && col.enabled)
            {
                return col;
            }
        }

        return null;
    }

    private bool TargetInAttackRange()
    {
        return GetDistanceToTarget() <= attackRange;
    }

    private void FlipECollider(Vector2 direction)
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

    private void FlipDetectionCollider(bool versoSinistra)
    {
        if (detectionCollider == null) return;

        float x = Mathf.Abs(detectionCollider.offset.x);

        detectionCollider.offset = new Vector2(
            versoSinistra ? -x : x,
            detectionCollider.offset.y
        );
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

            if (detectionCollider != null)
            {
                detectionCollider.offset = new Vector2(
                    Mathf.Abs(detectionCollider.offset.x),
                    detectionCollider.offset.y
                );
            }

            if (sr != null)
            {
                sr.flipX = false;
            }
        }
    }

    public void InflictDamage()
    {
        if (!CanInflictDamage()) return;

        if (currentTarget.CompareTag(playerTag))
        {
            PlayerHealth playerHealth = currentTarget.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-damage);
            }

            return;
        }

        if (currentTarget.CompareTag(npcTag))
        {
            MonkHealth monkHealth = currentTarget.GetComponent<MonkHealth>();

            if (monkHealth != null)
            {
                monkHealth.TakeDamage(damage);
            }
        }
    }

    public void EndAttack()
    {
        AggiornaTargetSeNecessario();

        if (!TargetValido())
        {
            TransizioneA(EnemyState.Idle);
            return;
        }

        if (TargetInAttackRange())
        {
            TransizioneA(EnemyState.Attack);
        }
        else
        {
            TransizioneA(EnemyState.Chasing);
        }
    }

    private bool CanInflictDamage()
    {
        if (!TargetValido()) return false;
        if (currentState != EnemyState.Attack) return false;
        if (!TargetInAttackRange()) return false;

        return true;
    }
}
