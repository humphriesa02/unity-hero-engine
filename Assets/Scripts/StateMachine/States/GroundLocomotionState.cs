using UnityEngine;

/// <summary>
/// Base state for all states that contain "grounded" movement.
/// 
/// This should feature "core" functionality of the player,
/// things like jumping, rolling, or z targetting.
/// 
/// Also moves to other locomotion states, like climbing ladders
/// or swimming.
/// </summary>
public class GroundLocomotionState : State
{
    protected bool jump;
    protected bool roll;
    private bool prevStrafe;
    private Vector3 strafeForward;
    protected bool lockOn;

    private Vector3 cVelocity;

    public GroundLocomotionState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        jump = false;
        roll = false;
        player.stateData.moveInput = Vector2.zero;
        player.stateData.moveDirection = Vector3.zero;
        player.stateData.velocity = Vector3.zero;
        player.stateData.gravityVelocity.y = 0;

        strafeForward = player.transform.forward;
        strafeForward.y = 0f;
        if (strafeForward.sqrMagnitude > 0.001f)
            strafeForward.Normalize();

        prevStrafe = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (primaryAction.triggered)
        {
            roll = true;
        }
        player.stateData.isStrafing = lockOnAction.IsPressed();

        if (lockOnAction.WasPressedThisFrame())
        {
            lockOn = true;
        }

        player.stateData.moveInput = moveAction.ReadValue<Vector2>();
        player.stateData.moveDirection = new Vector3(player.stateData.moveInput.x, 0.0f, player.stateData.moveInput.y);

        player.stateData.moveDirection = player.stateData.moveDirection.x * player.focus.right.normalized + player.stateData.moveDirection.z * player.focus.forward.normalized;
        player.stateData.moveDirection.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.animator.SetFloat("speed", player.stateData.moveInput.magnitude, player.speedDampTime, Time.deltaTime);
        player.animator.SetFloat("inputX", player.stateData.moveInput.x);
        player.animator.SetBool("isStrafing", player.stateData.isStrafing || player.stateData.lockOnTarget != null);

        if (jump) stateMachine.ChangeState(player.jumpState);

        // Roll if moving, otherwise contextual
        if (roll && player.stateData.moveInput.magnitude > 0.05f)
        {
            // roll state
        }

        if (lockOn)
        {
            if (player.stateData.lockOnTarget == null)
                player.LockOn();
            else
                player.LockOff();
        }
        lockOn = false;

        if (player.stateData.isStrafing && !prevStrafe)
        {
            if (player.stateData.lockOnTarget == null)
            {
                strafeForward = player.transform.forward;
                strafeForward.y = 0f;
                if (strafeForward.sqrMagnitude > 0.001f)
                    strafeForward.Normalize();
            }
        }
        prevStrafe = player.stateData.isStrafing;

        // Apply gravity
        player.stateData.gravityVelocity.y += player.gravityValue * Time.deltaTime;
        if (player.controller.isGrounded && player.stateData.gravityVelocity.y < 0)
        {
            player.stateData.gravityVelocity.y = 0f;
        }

        // Move the player
        player.stateData.velocity = Vector3.SmoothDamp(player.stateData.velocity, player.stateData.moveDirection, ref cVelocity, player.velocityDampTime);
        player.controller.Move(player.moveSpeed * Time.deltaTime * player.stateData.velocity + player.stateData.gravityVelocity * Time.deltaTime);

        // Handle rotation of the player
        if (player.stateData.lockOnTarget)
        {
            player.RotateTowardTarget(player.stateData.lockOnTarget);
            player.LockOnValidityCheck();
        }
        else if (player.stateData.isStrafing)
        {
            if (strafeForward.sqrMagnitude > 0.001f)
            {
                player.transform.rotation = Quaternion.Slerp(
                    player.transform.rotation,
                    Quaternion.LookRotation(strafeForward),
                    player.rotationDampTime
                );
            }
        }
        else if (player.stateData.moveDirection.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                Quaternion.LookRotation(player.stateData.moveDirection),
                player.rotationDampTime
            );
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.stateData.gravityVelocity.y = 0f;
        if (player.stateData.moveDirection.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(player.stateData.moveDirection);
        }
    }
}
