public class GameModePhotoState : BaseState<GameModeStateMachine.GameModeState>
{
    private GameModeStateMachine _stateMachine;
    
    public GameModePhotoState(GameModeStateMachine.GameModeState key, GameModeStateMachine stateMachine) : base(key)
    {
        _stateMachine = stateMachine;
    }

    public override void EnterState(GameModeStateMachine.GameModeState lastState)
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState() { }
}