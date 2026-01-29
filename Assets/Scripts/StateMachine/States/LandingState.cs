using UnityEngine;

public class LandingState : State
{
    private float timePassed;
    private float landingTime;
    public LandingState(PlayerController _player, StateMachine _stateMachine) : base(_player, _stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        timePassed = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timePassed > player.landingTime)
        {
            player.animator.SetTrigger("grounded");
            stateMachine.ChangeState(player.groundState);   
        }
        timePassed += Time.deltaTime;
    }
}
