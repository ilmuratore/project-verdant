using System.Collections;
using System.Security.Cryptography;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Moving,
    Dodging,
    Attacking,
    Dead
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float velocita = 2.5f;


    [Header("Dodge")]
    public float dodgeSpeed = 5f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 3f;

    [Header("State")]
    [SerializeField] private PlayerState currentState = PlayerState.Idle;

    private Rigidbody2D player;
    private Animator anim;
    private PlayerHealth playerHealth;

    private Vector2 inputDirection;
    private Vector2 lastMoveDirection = Vector2.right;

    private int facingDirection = 1;
    private bool canDodge = true;


    void Start()
    {
        
        player = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

    }

    void Update()
    {
        if(currentState == PlayerState.Dead)
        {
            return;
        }
        ReadMovementInput();
        HandleDodgeInput();


    }

    void FixedUpdate() 
    {
       switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Moving:
                MovePlayer(); 
                break;

            case PlayerState.Dodging:
            case PlayerState.Attacking:
            case PlayerState.Dead:
                break;
        }
    }


    private void ReadMovementInput()
    {
        float x = 0f;
        float y = 0f;

        if (Keyboard.current.dKey.isPressed) x = 1f;
        if (Keyboard.current.aKey.isPressed) x = -1f;
        if (Keyboard.current.wKey.isPressed) y = 1f;
        if (Keyboard.current.sKey.isPressed) y = -1f;


        inputDirection = new Vector2(x, y).normalized;

        if (inputDirection != Vector2.zero)
        {
            lastMoveDirection = inputDirection;
        }
    }

    private void MovePlayer()
    {
        player.linearVelocity = inputDirection * velocita;

        TransizioneA(inputDirection != Vector2.zero ? PlayerState.Moving : PlayerState.Idle);
        if((inputDirection.x > 0 && transform.localScale.x <0) || (inputDirection.x < 0 && transform.localScale.x > 0)) { Flip(); }
        UpdateMovementAnimation();

    }

    private void HandleDodgeInput()
    {
        if(Keyboard.current.vKey.wasPressedThisFrame && canDodge){
            StartCoroutine(Dodge());
        }
    }

    private IEnumerator Dodge()
    {
        canDodge = false;
        TransizioneA(PlayerState.Dodging);
        if(playerHealth != null)
        {
            playerHealth.SetInvulnerable(true);
        }
        if( anim != null)
        {
            anim.SetTrigger("Dodge");
        }
        player.linearVelocity = lastMoveDirection * dodgeSpeed;
        yield return new WaitForSeconds(dodgeDuration);
        player.linearVelocity = Vector2.zero;
        if(playerHealth != null)
        {
            playerHealth.SetInvulnerable(false);
        }
        TransizioneA(PlayerState.Idle);
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    private void TransizioneA(PlayerState nuovo)
    {
        currentState = nuovo;
    }

    private void UpdateMovementAnimation()
    {
        if (anim == null) return;
        anim.SetFloat("horizontal", Mathf.Abs(inputDirection.x));
        anim.SetFloat("vertical", Mathf.Abs(inputDirection.y));
        anim.SetBool("IsMoving", inputDirection != Vector2.zero);
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    

  
}
