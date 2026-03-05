using UnityEngine;

/// <summary>
/// Jumping/Inair state
/// </summary>
public class JumpState : State
{
    bool grounded;
    Vector3 airVelocity;
    public JumpState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        grounded = false;
        player.stateData.gravityVelocity.y = 0;

        player.animator.SetFloat("speed", 0);
        player.animator.SetTrigger("jump");
        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        player.stateData.moveInput = moveAction.ReadValue<Vector2>();
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
            airVelocity = new Vector3(player.stateData.moveInput.x, 0, player.stateData.moveInput.y);

            player.stateData.moveDirection = player.stateData.moveDirection.x * player.focus.right.normalized + player.stateData.moveDirection.z * player.focus.forward.normalized;
            player.stateData.velocity.y = 0f;
            airVelocity = airVelocity.x * player.focus.right.normalized + airVelocity.z * player.focus.forward.normalized;
            airVelocity.y = 0f;
            player.controller.Move(player.stateData.gravityVelocity * Time.deltaTime + (airVelocity*player.airControl+player.stateData.velocity * (1- player.airControl)) * player.moveSpeed * Time.deltaTime);
        }
        
        player.stateData.gravityVelocity.y += player.gravityValue * Time.deltaTime;
        grounded = player.controller.isGrounded;
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void Jump()
    {
        // TODO - dynamic jump amount based on velocity
        player.stateData.gravityVelocity.y += Mathf.Sqrt(player.jumpHeight * -3.0f * player.gravityValue);
    }
}
