using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Accessible by anyone, allowing outsiders
/// to know what's happening with the player.
/// 
/// Also spans all states allowing for one data store
/// 
/// Things like if they're in a cutscene,
/// if they're attacking, if they're invulnerable, etc.
/// </summary>
public struct PlayerStateData
{
    public Vector3 gravityVelocity; // Active "fake" gravity value
    public Vector3 velocity; // Simulated velocity correlating with CharacterController
    public Vector2 moveInput; // Raw movement input
    public Vector3 moveDirection; // Movement input relative to camera forward
    public Transform lockOnTarget; // The target we're locked on to, if any
    public bool isStrafing;
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
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
    [Tooltip("Rotation speed during lockon")]
    public float lockOnTurnSpeed = 540f;

    // Static Values
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public Transform focus;
    [HideInInspector] public Animator animator;
    [HideInInspector] public LockOnController lockOnController;
    [HideInInspector] public float gravityValue = -9.81f;
    [HideInInspector] public PlayerStateData stateData;
    
    // State Machine
    private StateMachine playerSM;
    public BaseGroundState groundState;
    public JumpState jumpState;
    public LandingState landingState;
    public CombatGroundState combatState;
    public AttackState attackState;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        lockOnController = GetComponent<LockOnController>();
        
        playerSM = new StateMachine();
        groundState = new BaseGroundState(this, playerSM);
        jumpState = new JumpState(this, playerSM);
        landingState = new LandingState(this, playerSM);
        combatState = new CombatGroundState(this, playerSM);
        attackState = new AttackState(this, playerSM);
    }

    void Start()
    {
        if (focus == null)
        {
            focus = Camera.main.transform;
        }

        playerSM.Initialize(groundState);
    }

    void Update()
    {
        playerSM.HandleInput();
        playerSM.LogicUpdate();
    }

    void LateUpdate()
    {
        playerSM.LateUpdate();
    }

    void FixedUpdate()
    {
        playerSM.PhysicsUpdate();
    }

    public void LockOn()
    {
        if(!lockOnController) return;
        Transform lockedOnTransform = lockOnController.AttemptLockOn();
        if (lockedOnTransform)
        {
            stateData.lockOnTarget = lockedOnTransform;
            CameraController.Instance.SetState(CameraState.LockOn);
        }
    }

    public void LockOff()
    {
        stateData.lockOnTarget = null;
    }

    public void LockOnValidityCheck()
    {
        if(!lockOnController) return;
        bool isTargetValid = lockOnController.CheckForTargetValidity(stateData.lockOnTarget);
        if (!isTargetValid)
        {
            stateData.lockOnTarget = null;
        }
    }

    public void RotateTowardTarget(Transform target)
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            lockOnTurnSpeed * Time.deltaTime
        );
    }

    void OnGUI()
    {
        GUI.Label(new Rect(15, 15, 300, 100), playerSM.GetCurrentState().ToString());
    }
}
