using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
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
    [Tooltip("Se true questo NPC passeggia;")]
    public bool puoPasseggiare = true;

    [Tooltip("Se true questo NPC puo dare quest")]
    public bool daQuest = false;

    [Header("Passeggiata")]
    public float velocita = 2f;
    public float raggioMovimento = 2f;
    public float attesaMin = 1.5f;
    public float attesaMax = 3f;

    [Header("Interazione")]
    public float raggioInterazione = 1.5f;
    public Transform player;


    [Header("Dialogo")]
    public DialogueController dialogo;

    [SerializeField] private NPCstate currentState = NPCstate.Idle;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private Vector3 puntoDiPartenza;
    private Vector2 destinazione;
    private float attesaCorrente;
    private float timer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        puntoDiPartenza = transform.position;
        TransizioneA(NPCstate.Idle);

    }

    void Update()
    {
        if (currentState != NPCstate.Talking && daQuest && PlayerVicino())
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                IniziaDialogo();
            }
        }

        switch (currentState)
        {
            case NPCstate.Idle: UpdateIdle(); break;
            case NPCstate.Wandering: UpdateWandering(); break;
            case NPCstate.Talking: break;
        }
    }


    private void UpdateIdle()
    {
        SetWalking(false);

        if (!puoPasseggiare) return;

        timer += Time.deltaTime;
        if(timer >= attesaCorrente)
        {
            ScegliNuovaDestinazione();
            TransizioneA(NPCstate.Wandering);
        }
    }

    private void UpdateWandering()
    {
        Vector2 posizione = transform.position;
        Vector2 verso = (destinazione - posizione);

        if(verso.magnitude <= 0.05f)
        {
            rb.linearVelocity = Vector2.zero;
            TransizioneA(NPCstate.Idle);
            return;
        }

        Vector2 direzione = verso.normalized;
        rb.linearVelocity = direzione * velocita;
        SetWalking(true);
        Flip(direzione);
    }

    private void TransizioneA(NPCstate nuovo)
    {
        currentState = nuovo;
        if(nuovo == NPCstate.Idle)
        {
            rb.linearVelocity = Vector2.zero;
            timer = 0f;
            attesaCorrente = Random.Range(attesaMin, attesaMax);
            SetWalking(false);
        } else if( nuovo == NPCstate.Talking)
        {
            rb.linearVelocity = Vector2.zero;
            SetWalking(false);
        }
    }

    private void ScegliNuovaDestinazione()
    {
        Vector2 offset = Random.insideUnitCircle * raggioMovimento;
        destinazione = (Vector2)puntoDiPartenza + offset;
    }

    private void IniziaDialogo()
    {
        if(dialogo == null)
        {
            Debug.LogWarning("DialogueController non assegnato a NPC");
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

    private void SetWalking(bool walking)
    {
        if (anim != null) anim.SetBool("IsWalking", walking);
    }

    private void Flip(Vector2 direzione)
    {
        if (sr == null) return;
        if (direzione.x > 0.1f) sr.flipX = false;
        else if (direzione.x < 0.1f) sr.flipX = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raggioInterazione);
    }
}
