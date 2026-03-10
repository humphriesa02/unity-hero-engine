using UnityEngine;

/// <summary>
/// Basic grounded movement
/// </summary>
public class BaseGroundState : GroundLocomotionState
{
    bool drawWeapon;

    public BaseGroundState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        drawWeapon = false;
        player.animator.SetBool("combat", false);
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if (secondaryAction.triggered)
        {
            drawWeapon = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (drawWeapon)
        { 
            stateMachine.ChangeState(player.combatState);
            player.animator.SetTrigger("drawWeapon");
        }
        
    }

    public override void Exit()
    {
        base.Exit();
    }
}
