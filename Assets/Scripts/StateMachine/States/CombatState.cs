using UnityEngine;

public class CombatState : State
{
    bool sheatheWeapon;
    bool attack;

    private Vector3 cVelocity;

    public CombatState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        sheatheWeapon = false;
        player.stateData.moveInput = Vector2.zero;
        player.stateData.moveDirection = Vector3.zero;
        player.stateData.velocity = Vector3.zero;
        player.stateData.gravityVelocity.y = 0;
        player.animator.SetBool("combat", true);
        attack = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (primaryAction.triggered)
        {
            sheatheWeapon = true;
        }
        if (secondaryAction.triggered)
        {
            attack = true;
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

        if (sheatheWeapon)
        { 
            player.animator.SetTrigger("sheatheWeapon");
            stateMachine.ChangeState(player.groundState);
        }

        if (attack)
        {
            player.animator.SetTrigger("attack");
            stateMachine.ChangeState(player.attackState);
        }
        
        player.stateData.gravityVelocity.y += player.gravityValue * Time.deltaTime;

        if (player.controller.isGrounded && player.stateData.gravityVelocity.y < 0)
        {
            player.stateData.gravityVelocity.y = 0f;
        }

        player.stateData.velocity = Vector3.SmoothDamp(player.stateData.velocity, player.stateData.moveDirection, ref cVelocity, player.velocityDampTime);
        player.controller.Move(player.moveSpeed * Time.deltaTime * player.stateData.velocity + player.stateData.gravityVelocity * Time.deltaTime);

        if (player.stateData.moveDirection.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(player.stateData.moveDirection), player.rotationDampTime);
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
