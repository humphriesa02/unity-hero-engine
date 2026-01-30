using UnityEngine;

/// <summary>
/// Basic grounded movement
/// </summary>
public class GroundState : State
{
    float gravityValue;
    bool jump;
    bool roll;
    Vector3 currentVelocity;
    bool grounded;
    float playerSpeed;
    bool drawWeapon;

    private Vector3 cVelocity;

    public GroundState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        jump = false;
        roll = false;
        moveInput = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        drawWeapon = false;
        player.animator.SetBool("combat", false);
 
        playerSpeed = player.moveSpeed;
        grounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (primaryAction.triggered)
        {
            roll = true;
        }

        if (secondaryAction.triggered)
        {
            drawWeapon = true;
        }

        moveInput = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(moveInput.x, 0.0f, moveInput.y);

        velocity = velocity.x * player.focus.right.normalized + velocity.z * player.focus.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.animator.SetFloat("speed", moveInput.magnitude, player.speedDampTime, Time.deltaTime);

        if (jump) stateMachine.ChangeState(player.jumpState);
        if (drawWeapon)
        { 
            stateMachine.ChangeState(player.combatState);
            player.animator.SetTrigger("drawWeapon");
        }
        if (roll)
        {
            // roll state
        }
        
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = player.controller.isGrounded;

        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
        player.controller.Move(playerSpeed * Time.deltaTime * currentVelocity + gravityVelocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity), player.rotationDampTime);
        }
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(moveInput.x, 0, moveInput.y);
        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
