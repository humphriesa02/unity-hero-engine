using UnityEngine;

/// <summary>
/// Jumping/Inair state
/// </summary>
public class JumpState : State
{
    bool grounded;
 
    float gravityValue;
    float jumpHeight;
    float playerSpeed;
 
    Vector3 airVelocity;
    public JumpState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        grounded = false;
        gravityValue = player.gravityValue;
        jumpHeight = player.jumpHeight;
        playerSpeed = player.moveSpeed;
        gravityVelocity.y = 0;

        player.animator.SetFloat("speed", 0);
        player.animator.SetTrigger("jump");
        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        moveInput = moveAction.ReadValue<Vector2>();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (grounded)
        {
            stateMachine.ChangeState(player.landingState);
            
        }
        else // In air
        {
            airVelocity = new Vector3(moveInput.x, 0, moveInput.y);

            velocity = velocity.x * player.focus.right.normalized + velocity.z * player.focus.forward.normalized;
            velocity.y = 0f;
            airVelocity = airVelocity.x * player.focus.right.normalized + airVelocity.z * player.focus.forward.normalized;
            airVelocity.y = 0f;
            player.controller.Move(gravityVelocity * Time.deltaTime + (airVelocity*player.airControl+velocity * (1- player.airControl)) * player.moveSpeed * Time.deltaTime);
        }
        
        gravityVelocity.y += player.gravityValue * Time.deltaTime;
        grounded = player.controller.isGrounded;
    }

    public override void Exit()
    {
        base.Exit();

    }

    private void Jump()
    {
        // TODO - dynamic jump amount based on velocity
        gravityVelocity.y += Mathf.Sqrt(player.jumpHeight * -3.0f * player.gravityValue);
    }
}
