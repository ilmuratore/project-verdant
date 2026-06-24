using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction dodgeAction;
    private InputAction interactAction;
    private InputAction pauseAction;
    private InputAction toggleStatsAction;

    public event Action OnAttack;
    public event Action OnDodge;
    public event Action OnInteract;
    public event Action OnPause;
    public event Action OnToggleStats;

    public Vector2 MovementValue => moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CreateActions();
    }

    private void OnEnable()
    {
        EnableActions();
    }

    private void OnDisable()
    {
        DisableActions();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        DisposeActions();
    }

    public static GameInput GetOrCreate()
    {
        if (Instance != null) return Instance;

        GameInput existing = FindFirstObjectByType<GameInput>();
        if (existing != null) return existing;

        GameObject inputObject = new GameObject("InputManager");
        return inputObject.AddComponent<GameInput>();
    }

    private void CreateActions()
    {
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick");

        attackAction = new InputAction("Attack", InputActionType.Button);
        attackAction.AddBinding("<Keyboard>/space");
        attackAction.AddBinding("<Mouse>/leftButton");
        attackAction.AddBinding("<Gamepad>/buttonSouth");

        dodgeAction = new InputAction("Dodge", InputActionType.Button);
        dodgeAction.AddBinding("<Keyboard>/v");
        dodgeAction.AddBinding("<Keyboard>/leftShift");
        dodgeAction.AddBinding("<Gamepad>/buttonEast");

        interactAction = new InputAction("Interact", InputActionType.Button);
        interactAction.AddBinding("<Keyboard>/e");
        interactAction.AddBinding("<Keyboard>/enter");
        interactAction.AddBinding("<Gamepad>/buttonWest");

        pauseAction = new InputAction("Pause", InputActionType.Button);
        pauseAction.AddBinding("<Keyboard>/escape");
        pauseAction.AddBinding("<Gamepad>/start");

        toggleStatsAction = new InputAction("ToggleStats", InputActionType.Button);
        toggleStatsAction.AddBinding("<Keyboard>/p");
        toggleStatsAction.AddBinding("<Gamepad>/select");

        attackAction.performed += _ => OnAttack?.Invoke();
        dodgeAction.performed += _ => OnDodge?.Invoke();
        interactAction.performed += _ => OnInteract?.Invoke();
        pauseAction.performed += _ => OnPause?.Invoke();
        toggleStatsAction.performed += _ => OnToggleStats?.Invoke();
    }

    private void EnableActions()
    {
        moveAction?.Enable();
        attackAction?.Enable();
        dodgeAction?.Enable();
        interactAction?.Enable();
        pauseAction?.Enable();
        toggleStatsAction?.Enable();
    }

    private void DisableActions()
    {
        moveAction?.Disable();
        attackAction?.Disable();
        dodgeAction?.Disable();
        interactAction?.Disable();
        pauseAction?.Disable();
        toggleStatsAction?.Disable();
    }

    private void DisposeActions()
    {
        moveAction?.Dispose();
        attackAction?.Dispose();
        dodgeAction?.Dispose();
        interactAction?.Dispose();
        pauseAction?.Dispose();
        toggleStatsAction?.Dispose();
    }
}
