using UnityEngine;

public enum NPCstate
{
    Idle,
    Wandering,
    Talking
}

[RequireComponent(typeof(Rigidbody2D))]
public class NPC_AI : MonoBehaviour
{
    public bool puoPasseggiare = true;
    public bool daQuest = false;
    public float velocita = 2f;
    public float raggioMovimento = 2f;
    public float attesaMin = 1.5f;
    public float attesaMax = 3f;
    public float distanzaArrivo = 0.08f;
    public LayerMask obstacleLayer;
    public float obstacleCheckRadius = 0.18f;
    public float obstacleCheckDistance = 0.25f;
    public float raggioInterazione = 1.5f;
    public Transform player;

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
    private GameInput input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        survivorAI = GetComponent<MonkSurvivorAI>();
        health = GetComponent<MonkHealth>();
        puntoDiPartenza = transform.position;
    }

    private void OnEnable()
    {
        input = GameInput.GetOrCreate();
        input.OnInteract += HandleInteract;
    }

    private void OnDisable()
    {
        if (input != null) input.OnInteract -= HandleInteract;
    }

    private void Start()
    {
        ResolveSceneReferences();
        SetState(NPCstate.Idle);
    }

    private void Update()
    {
        if (MonkMorto())
        {
            SetState(NPCstate.Idle);
            return;
        }

        if (MovimentoGestitoDaSurvivor())
        {
            currentState = NPCstate.Idle;
            timer = 0f;
            return;
        }

        if (currentState == NPCstate.Idle)
        {
            UpdateIdle();
        }
    }

    private void FixedUpdate()
    {
        if (MonkMorto())
        {
            StopMovement();
            return;
        }

        if (MovimentoGestitoDaSurvivor()) return;

        if (currentState == NPCstate.Wandering)
        {
            UpdateWandering();
        }
    }

    private void ResolveSceneReferences()
    {
        if (player != null && player.gameObject.scene.IsValid()) return;

        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null) player = playerMovement.transform;
    }

    private void HandleInteract()
    {
        if (!daQuest) return;
        if (currentState == NPCstate.Talking) return;
        if (GameManager.Instance != null && GameManager.Instance.ControlsBlocked) return;
        if (!PlayerVicino()) return;

        StartDialogue();
    }

    private void UpdateIdle()
    {
        StopMovement();

        if (!puoPasseggiare) return;

        timer += Time.deltaTime;

        if (timer >= attesaCorrente)
        {
            ChooseNewDestination();
            SetState(NPCstate.Wandering);
        }
    }

    private void UpdateWandering()
    {
        Vector2 posizione = rb.position;
        Vector2 verso = destinazione - posizione;

        if (verso.magnitude <= distanzaArrivo)
        {
            SetState(NPCstate.Idle);
            return;
        }

        Vector2 direzione = verso.normalized;

        if (!DirectionFree(direzione))
        {
            SetState(NPCstate.Idle);
            return;
        }

        rb.linearVelocity = direzione * velocita;
        SetWalking(true);
        Flip(direzione);
    }

    private void SetState(NPCstate newState)
    {
        currentState = newState;

        if (newState == NPCstate.Idle)
        {
            StopMovement();
            timer = 0f;
            attesaCorrente = Random.Range(attesaMin, attesaMax);
        }

        if (newState == NPCstate.Talking)
        {
            StopMovement();
        }
    }

    private void ChooseNewDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * raggioMovimento;
            Vector2 nuovaDestinazione = (Vector2)puntoDiPartenza + offset;

            if (PointFree(nuovaDestinazione))
            {
                destinazione = nuovaDestinazione;
                return;
            }
        }

        destinazione = transform.position;
    }

    private bool PointFree(Vector2 point)
    {
        if (obstacleLayer.value == 0) return true;
        return Physics2D.OverlapCircle(point, obstacleCheckRadius, obstacleLayer) == null;
    }

    private bool DirectionFree(Vector2 direction)
    {
        if (obstacleLayer.value == 0) return true;
        RaycastHit2D hit = Physics2D.CircleCast(rb.position, obstacleCheckRadius, direction, obstacleCheckDistance, obstacleLayer);
        return hit.collider == null;
    }

    private void StartDialogue()
    {
        SetState(NPCstate.Talking);
        UIManager.Instance?.OpenDialogue(this);
    }

    public void FineDialogo()
    {
        SetState(NPCstate.Idle);
    }

    private bool PlayerVicino()
    {
        ResolveSceneReferences();
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= raggioInterazione;
    }

    private bool MonkMorto()
    {
        return health != null && health.isDead;
    }

    private bool MovimentoGestitoDaSurvivor()
    {
        return survivorAI != null && survivorAI.IsControllingMovement;
    }

    private void StopMovement()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
        SetWalking(false);
    }

    private void SetWalking(bool value)
    {
        if (anim != null) anim.SetBool("IsWalking", value);
    }

    private void Flip(Vector2 direction)
    {
        if (sr == null) return;
        if (direction.x > 0.01f) sr.flipX = false;
        if (direction.x < -0.01f) sr.flipX = true;
    }
}
