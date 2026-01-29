
using UnityEngine;

public class StateMachine
{
    private State currentState;

    public State GetCurrentState() {return currentState;}
    
    public void Initialize(State startingState)
    {
        currentState = startingState;
        startingState.Enter();
    }

    public void HandleInput()
    {
        currentState.HandleInput();
    }

    public void LogicUpdate()
    {
        currentState.LogicUpdate();
    }

    public void LateUpdate()
    {
        currentState.LateUpdate();
    }

    public void PhysicsUpdate()
    {
        currentState.PhysicsUpdate();
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();

        currentState = newState;

        newState.Enter();
    }
}
