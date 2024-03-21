using System;

public class GameModeBuildState : BaseState<GameModeStateMachine.GameModeState>
{
    GameModeStateMachine _stateMachine;
    
    public GameModeBuildState(GameModeStateMachine.GameModeState key, GameModeStateMachine stateMachine) : base(key)
    {
        _stateMachine = stateMachine;
    }

    public override void EnterState(GameModeStateMachine.GameModeState lastState)
    {
        // List for events from the PlacedFocusedSubState
        CbObjectPlacedFocusedSubState.CbObjectFocused += OnObjectFocusEvent;

        // Subscribe to the photo mode switch button
    }

    private void OnObjectFocusEvent(CbObjectData data)
    {
        _stateMachine.QueueNextState(GameModeStateMachine.GameModeState.Focus);
    }

    public override void ExitState() { }

    public override void UpdateState() { }
}