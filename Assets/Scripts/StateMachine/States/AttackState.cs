using UnityEngine;

public class AttackState : State
{
    float timePassed;
    float clipLength;
    float clipSpeed;
    bool attack;

    public AttackState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        attack = false;
        player.animator.applyRootMotion = true;
        timePassed = 0f;
        player.animator.SetTrigger("attack");
        player.animator.SetFloat("speed", 0f);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (secondaryAction.triggered)
        {
            attack = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timePassed += Time.deltaTime;
        clipLength = player.animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
        clipSpeed = player.animator.GetCurrentAnimatorStateInfo(1).speed;

        // Combo
        if (timePassed >= clipLength / clipSpeed && attack)
        {
            stateMachine.ChangeState(player.attackState);
        }
        if (timePassed >= clipLength / clipSpeed)
        {
            stateMachine.ChangeState(player.combatState);
            player.animator.SetTrigger("grounded");
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.applyRootMotion = false;
    }
}
