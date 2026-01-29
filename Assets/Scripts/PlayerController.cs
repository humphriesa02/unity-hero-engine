using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Control")]
    [Tooltip("Base max move speed of the player")]
    public float moveSpeed = 6.0f;
    [Tooltip("Base max jump height of the player")]
    public float jumpHeight = 0.8f;
    [Tooltip("Base max roll speed of the player")]
    public float rollSpeed = 10.0f;
    [Tooltip("Gravity multiplier, increase or decrease to affect gravity")]
    public float gravityMultiplier = 1f;
    [Range(0, 1), Tooltip("Animation speed damp time.")]
    public float speedDampTime = 0.1f;
    [Range(0, 1), Tooltip("The rate at which our velocity falls off. Increase for slidey movement.")]
    public float velocityDampTime = 0.9f;
    [Range(0, 1), Tooltip("The rate at which we fully rotate the player.")]
    public float rotationDampTime = 0.2f;
    [Range(0, 1), Tooltip("The amount of control we have over the player in the air.")]
    public float airControl = 0.5f;
    [Tooltip("The amount of time we stay 'landed'. After this time ends we can move again")]
    public float landingTime = 0.5f;

    // Static Values
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public Transform focus;
    [HideInInspector] public Animator animator;
    [HideInInspector] public float gravityValue = -9.81f;
    [HideInInspector] public Vector3 playerVelocity;
    
    // State Machine
    private StateMachine movementSM;
    public GroundState groundState;
    public JumpState jumpState;
    public LandingState landingState;
    public CombatState combatState;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        
        movementSM = new StateMachine();
        groundState = new GroundState(this, movementSM);
        jumpState = new JumpState(this, movementSM);
        landingState = new LandingState(this, movementSM);
        combatState = new CombatState(this, movementSM);
    }

    void Start()
    {
        if (focus == null)
        {
            focus = Camera.main.transform;
        }

        movementSM.Initialize(groundState);
    }

    void Update()
    {
        movementSM.HandleInput();
        movementSM.LogicUpdate();
    }

    void LateUpdate()
    {
        movementSM.LateUpdate();
    }

    void FixedUpdate()
    {
        movementSM.PhysicsUpdate();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(15, 15, 300, 100), movementSM.GetCurrentState().ToString());
    }
}
