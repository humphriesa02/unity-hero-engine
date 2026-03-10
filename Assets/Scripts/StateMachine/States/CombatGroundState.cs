using UnityEngine;

public class CombatGroundState : GroundLocomotionState
{
    bool sheatheWeapon;
    bool attack;

    public CombatGroundState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        sheatheWeapon = false;
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
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (sheatheWeapon && player.stateData.moveInput.magnitude <= 0.05)
        { 
            player.animator.SetTrigger("sheatheWeapon");
            stateMachine.ChangeState(player.groundState);
        }

        if (attack)
        {
            player.animator.SetTrigger("attack");
            stateMachine.ChangeState(player.attackState);
        }
        sheatheWeapon = false;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
