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
    [Header("Scappa")]
    public float raggioPericolo = 1.5f;
    public float velocitaFuga = 2.5f;
    public LayerMask enemyLayer;

    [Header("Ritorno")]
    public float velocitaRitorno = 2f;
    public float distanzaArrivo = 0.08f;

    [Header("Ostacoli")]
    [Tooltip("Layer della Tilemap, case, muri, ostacoli.")]
    public LayerMask obstacleLayer;

    [Tooltip("Raggio usato per controllare se davanti al monaco c'è un ostacolo.")]
    public float obstacleCheckRadius = 0.18f;

    [Tooltip("Distanza del controllo ostacolo davanti al monaco.")]
    public float obstacleCheckDistance = 0.25f;

    [Header("Animazione")]
    public string walkingParameter = "IsWalking";

    [SerializeField] private MonkSurvivorState currentState = MonkSurvivorState.Safe;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private MonkHealth health;

    private Vector3 puntoDiPartenza;

    public bool IsControllingMovement
    {
        get
        {
            return currentState == MonkSurvivorState.Fleeing ||
                   currentState == MonkSurvivorState.ReturningHome;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = GetComponent<MonkHealth>();

        puntoDiPartenza = transform.position;
        currentState = MonkSurvivorState.Safe;
    }

    private void FixedUpdate()
    {
        if (MonkMorto())
        {
            currentState = MonkSurvivorState.Dead;
            StopMovement();
            return;
        }

        Transform enemy = TrovaNemicoPiuVicino();

        if (enemy != null)
        {
            currentState = MonkSurvivorState.Fleeing;
            ScappaDa(enemy);
            return;
        }

        if (currentState == MonkSurvivorState.Fleeing ||
            currentState == MonkSurvivorState.ReturningHome)
        {
            currentState = MonkSurvivorState.ReturningHome;
            RitornaAlPuntoDiPartenza();
            return;
        }

        currentState = MonkSurvivorState.Safe;
        StopMovement();
    }

    private bool MonkMorto()
    {
        return health != null && health.isDead;
    }

    private Transform TrovaNemicoPiuVicino()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            raggioPericolo,
            enemyLayer
        );

        Transform migliore = null;
        float distanzaMigliore = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            if (hit.isTrigger) continue;
            if (!hit.gameObject.activeInHierarchy) continue;

            EnemyStats enemyStats = hit.GetComponentInParent<EnemyStats>();
            if (enemyStats == null) continue;

            float distanza = Vector2.Distance(transform.position, enemyStats.transform.position);

            if (distanza > raggioPericolo) continue;

            if (distanza < distanzaMigliore)
            {
                distanzaMigliore = distanza;
                migliore = enemyStats.transform;
            }
        }

        return migliore;
    }

    private void ScappaDa(Transform enemy)
    {
        Vector2 direzione = ((Vector2)transform.position - (Vector2)enemy.position).normalized;
        MuoviInDirezione(direzione, velocitaFuga);
    }

    private void RitornaAlPuntoDiPartenza()
    {
        Vector2 posizione = rb.position;
        Vector2 casa = puntoDiPartenza;
        Vector2 versoCasa = casa - posizione;

        if (versoCasa.magnitude <= distanzaArrivo)
        {
            rb.position = casa;
            currentState = MonkSurvivorState.Safe;
            StopMovement();
            return;
        }

        Vector2 direzione = versoCasa.normalized;
        MuoviInDirezione(direzione, velocitaRitorno);
    }

    private void MuoviInDirezione(Vector2 direzione, float velocita)
    {
        if (direzione == Vector2.zero)
        {
            StopMovement();
            return;
        }

        if (DirezioneLibera(direzione))
        {
            rb.linearVelocity = direzione * velocita;
            SetWalking(true);
            Flip(direzione);
            return;
        }

        Vector2 direzioneLateraleA = Vector2.Perpendicular(direzione).normalized;
        Vector2 direzioneLateraleB = -direzioneLateraleA;

        if (DirezioneLibera(direzioneLateraleA))
        {
            rb.linearVelocity = direzioneLateraleA * velocita;
            SetWalking(true);
            Flip(direzioneLateraleA);
            return;
        }

        if (DirezioneLibera(direzioneLateraleB))
        {
            rb.linearVelocity = direzioneLateraleB * velocita;
            SetWalking(true);
            Flip(direzioneLateraleB);
            return;
        }

        StopMovement();
    }

    private bool DirezioneLibera(Vector2 direzione)
    {
        if (obstacleLayer.value == 0) return true;

        RaycastHit2D hit = Physics2D.CircleCast(
            rb.position,
            obstacleCheckRadius,
            direzione,
            obstacleCheckDistance,
            obstacleLayer
        );

        return hit.collider == null;
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        SetWalking(false);
    }

    private void SetWalking(bool value)
    {
        if (anim != null)
        {
            anim.SetBool(walkingParameter, value);
        }
    }

    private void Flip(Vector2 direzione)
    {
        if (sr == null) return;

        if (direzione.x > 0.1f)
        {
            sr.flipX = false;
        }
        else if (direzione.x < -0.1f)
        {
            sr.flipX = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, raggioPericolo);

        Gizmos.color = Color.cyan;
        Vector3 centro = puntoDiPartenza == Vector3.zero ? transform.position : puntoDiPartenza;
        Gizmos.DrawWireSphere(centro, distanzaArrivo);
    }
}
