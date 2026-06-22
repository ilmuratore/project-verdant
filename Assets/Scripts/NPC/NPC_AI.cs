using UnityEngine;
using UnityEngine.InputSystem;

public enum NPCstate
{
    Idle,
    Wandering,
    Talking
}

public class NPC_AI : MonoBehaviour
{
    [Header("Comportamento")]
    [Tooltip("Se true questo NPC passeggia.")]
    public bool puoPasseggiare = true;

    [Tooltip("Se true questo NPC può dare quest.")]
    public bool daQuest = false;

    [Header("Passeggiata")]
    public float velocita = 2f;
    public float raggioMovimento = 2f;
    public float attesaMin = 1.5f;
    public float attesaMax = 3f;
    public float distanzaArrivo = 0.08f;

    [Header("Ostacoli")]
    [Tooltip("Layer della Tilemap, case, muri, ostacoli.")]
    public LayerMask obstacleLayer;
    public float obstacleCheckRadius = 0.18f;
    public float obstacleCheckDistance = 0.25f;

    [Header("Interazione")]
    public float raggioInterazione = 1.5f;
    public Transform player;

    [Header("Dialogo")]
    public DialogueController dialogo;

    [SerializeField] private NPCstate currentState = NPCstate.Idle;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private MonkSurvivorAI survivorAI;
    private MonkHealth health;

    private Vector3 puntoDiPartenza;
    private Vector2 destinazione;
    private float attesaCorrente;
    private float timer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        survivorAI = GetComponent<MonkSurvivorAI>();
        health = GetComponent<MonkHealth>();

        puntoDiPartenza = transform.position;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        TransizioneA(NPCstate.Idle);
    }

    private void Update()
    {
        if (MonkMorto())
        {
            TransizioneA(NPCstate.Idle);
            return;
        }

        if (MovimentoGestitoDaSurvivor())
        {
            currentState = NPCstate.Idle;
            timer = 0f;
            return;
        }

        if (currentState != NPCstate.Talking && daQuest && PlayerVicino())
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                IniziaDialogo();
            }
        }

        switch (currentState)
        {
            case NPCstate.Idle:
                UpdateIdle();
                break;

            case NPCstate.Wandering:
                break;

            case NPCstate.Talking:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (MonkMorto())
        {
            StopMovement();
            return;
        }

        if (MovimentoGestitoDaSurvivor())
        {
            return;
        }

        if (currentState == NPCstate.Wandering)
        {
            UpdateWanderingFisico();
        }
    }

    private bool MonkMorto()
    {
        return health != null && health.isDead;
    }

    private bool MovimentoGestitoDaSurvivor()
    {
        return survivorAI != null && survivorAI.IsControllingMovement;
    }

    private void UpdateIdle()
    {
        StopMovement();

        if (!puoPasseggiare) return;

        timer += Time.deltaTime;

        if (timer >= attesaCorrente)
        {
            ScegliNuovaDestinazione();
            TransizioneA(NPCstate.Wandering);
        }
    }

    private void UpdateWanderingFisico()
    {
        Vector2 posizione = rb.position;
        Vector2 verso = destinazione - posizione;

        if (verso.magnitude <= distanzaArrivo)
        {
            TransizioneA(NPCstate.Idle);
            return;
        }

        Vector2 direzione = verso.normalized;

        if (!DirezioneLibera(direzione))
        {
            TransizioneA(NPCstate.Idle);
            return;
        }

        rb.linearVelocity = direzione * velocita;
        SetWalking(true);
        Flip(direzione);
    }

    private void TransizioneA(NPCstate nuovo)
    {
        currentState = nuovo;

        if (nuovo == NPCstate.Idle)
        {
            StopMovement();
            timer = 0f;
            attesaCorrente = Random.Range(attesaMin, attesaMax);
        }
        else if (nuovo == NPCstate.Talking)
        {
            StopMovement();
        }
    }

    private void ScegliNuovaDestinazione()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * raggioMovimento;
            Vector2 nuovaDestinazione = (Vector2)puntoDiPartenza + offset;

            if (PuntoLibero(nuovaDestinazione))
            {
                destinazione = nuovaDestinazione;
                return;
            }
        }

        destinazione = transform.position;
    }

    private bool PuntoLibero(Vector2 punto)
    {
        if (obstacleLayer.value == 0) return true;

        Collider2D hit = Physics2D.OverlapCircle(
            punto,
            obstacleCheckRadius,
            obstacleLayer
        );

        return hit == null;
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

    private void IniziaDialogo()
    {
        if (dialogo == null)
        {
            Debug.LogWarning("DialogueController non assegnato a NPC.");
            return;
        }

        TransizioneA(NPCstate.Talking);
        dialogo.ApriDialogo(this);
    }

    public void FineDialogo()
    {
        TransizioneA(NPCstate.Idle);
    }

    private bool PlayerVicino()
    {
        if (player == null) return false;

        return Vector2.Distance(transform.position, player.position) <= raggioInterazione;
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        SetWalking(false);
    }

    private void SetWalking(bool walking)
    {
        if (anim != null)
        {
            anim.SetBool("IsWalking", walking);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raggioInterazione);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raggioMovimento);
    }
}
