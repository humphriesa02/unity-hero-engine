using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Base Player State
/// 
/// Used for all Player actions/movement
/// </summary>
public class State
{
    // States store ref to player and their owning machine.
    // They handle swapping themselves to other states
    protected PlayerController player;
    protected StateMachine stateMachine;

    // Some good 
    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 moveInput;

    protected InputAction moveAction; // Movement
    protected InputAction primaryAction; // "A" button press
    protected InputAction secondaryAction; // "B" button press

    public State(PlayerController _player, StateMachine _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;

        moveAction = player.playerInput.actions["Move"];
        primaryAction = player.playerInput.actions["Primary"];
        secondaryAction = player.playerInput.actions["Secondary"];
    }

    /// <summary>
    /// Called upon entering a state, once.
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// Called every frame. Equivalent technically
    /// to a logic update, but dedicated to polling input
    /// from the input actions
    /// </summary>
    public virtual void HandleInput() { }

    /// <summary>
    /// Equivalent to a single Update() call.
    /// Called once every frame. Should not handle physics logic
    /// </summary>
    public virtual void LogicUpdate() { }

    /// <summary>
    /// Equivalent to a single LateUpdate() call.
    /// Called once every frame, at the end.
    /// </summary>
    public virtual void LateUpdate() { }

    /// <summary>
    /// Equivalent to a FixedUpdate.
    /// Called once every frame, should handle physics related things.
    /// </summary>
    public virtual void PhysicsUpdate() { }

    /// <summary>
    /// Called upon leaving a state, once.
    /// </summary>
    public virtual void Exit() { }
}
