using System.Collections;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Dodging,
    Attacking,
    Dead
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerStatsData stats;
    public bool controlliBloccato = false;

    [SerializeField] private PlayerState currentState = PlayerState.Idle;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerHealth playerHealth;
    private Vector2 inputDirection;
    private Vector2 lastMoveDirection = Vector2.right;
    private int facingDirection = 1;
    private bool canDodge = true;
    private GameInput input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        PlayerStats playerStats = GetComponent<PlayerStats>();
        if (stats == null && playerStats != null) stats = playerStats.data;
    }

    private void OnEnable()
    {
        input = GameInput.GetOrCreate();
        input.OnDodge += HandleDodgePressed;
    }

    private void OnDisable()
    {
        if (input != null) input.OnDodge -= HandleDodgePressed;
    }

    private void Update()
    {
        if (IsControlBlocked())
        {
            StopPlayer();
            return;
        }

        if (currentState == PlayerState.Dead) return;

        ReadMovementInput();
    }

    private void FixedUpdate()
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

    private bool IsControlBlocked()
    {
        return controlliBloccato || GameManager.Instance != null && GameManager.Instance.ControlsBlocked;
    }

    private void ReadMovementInput()
    {
        inputDirection = input != null ? input.MovementValue.normalized : Vector2.zero;

        if (inputDirection != Vector2.zero)
        {
            lastMoveDirection = inputDirection;
        }
    }

    private void MovePlayer()
    {
        float speed = stats != null ? stats.velocita : 5f;
        rb.linearVelocity = inputDirection * speed;

        SetState(inputDirection != Vector2.zero ? PlayerState.Moving : PlayerState.Idle);

        if ((inputDirection.x > 0 && transform.localScale.x < 0) ||
            (inputDirection.x < 0 && transform.localScale.x > 0))
        {
            Flip();
        }

        UpdateMovementAnimation();
    }

    private void HandleDodgePressed()
    {
        if (IsControlBlocked()) return;
        if (!canDodge || !CanDodge()) return;
        StartCoroutine(Dodge());
    }

    private IEnumerator Dodge()
    {
        float dodgeSpeed = stats != null ? stats.dodgeSpeed : 12f;
        float dodgeDuration = stats != null ? stats.dodgeDuration : 0.2f;
        float dodgeCooldown = stats != null ? stats.dodgeCooldown : 0.7f;

        canDodge = false;
        SetState(PlayerState.Dodging);
        playerHealth?.SetInvulnerable(true);
        anim?.SetTrigger("Dodge");
        rb.linearVelocity = lastMoveDirection * dodgeSpeed;

        yield return new WaitForSeconds(dodgeDuration);

        rb.linearVelocity = Vector2.zero;
        playerHealth?.SetInvulnerable(false);
        SetState(PlayerState.Idle);

        yield return new WaitForSeconds(dodgeCooldown);

        canDodge = true;
    }

    public bool CanAttack()
    {
        return currentState != PlayerState.Attacking && currentState != PlayerState.Dodging && currentState != PlayerState.Dead;
    }

    public bool CanDodge()
    {
        return currentState != PlayerState.Attacking && currentState != PlayerState.Dodging && currentState != PlayerState.Dead;
    }

    public void StartAttack()
    {
        SetState(PlayerState.Attacking);
        rb.linearVelocity = Vector2.zero;
        UpdateMovementAnimation();
    }

    public void EndAttack()
    {
        if (currentState == PlayerState.Attacking) SetState(PlayerState.Idle);
    }

    public void SetDead()
    {
        SetState(PlayerState.Dead);
        StopPlayer();
    }

    public void BloccaController()
    {
        controlliBloccato = true;
        StopPlayer();
        SetState(PlayerState.Idle);
        if (anim != null) anim.ResetTrigger("Attack");
    }

    public void SbloccaController()
    {
        controlliBloccato = false;
    }

    private void StopPlayer()
    {
        inputDirection = Vector2.zero;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        UpdateMovementAnimation();
    }

    private void SetState(PlayerState newState)
    {
        currentState = newState;
    }

    private void UpdateMovementAnimation()
    {
        if (anim == null) return;
        anim.SetFloat("horizontal", Mathf.Abs(inputDirection.x));
        anim.SetFloat("vertical", Mathf.Abs(inputDirection.y));
        anim.SetBool("IsMoving", inputDirection != Vector2.zero && currentState == PlayerState.Moving);
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
    }
}
