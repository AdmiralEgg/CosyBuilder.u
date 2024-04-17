
using UnityEngine;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;

public class CbObjectSpawnedState : BaseState<CbObjectStateMachine.CbObjectState>
{
    private CbObjectStateMachine _stateMachine;
    private CbObjectMovementController _movementController;

    public CbObjectSpawnedState(CbObjectStateMachine.CbObjectState key, CbObjectStateMachine stateMachine, CbObjectMovementController movementController) : base(key)
    {
        _movementController = movementController;
        _stateMachine = stateMachine;
    }

    public override void EnterState(CbObjectStateMachine.CbObjectState lastState)
    {
        // Move to the cursor
        _movementController.MoveSpawnedObject();

        // Play spawned sound
        _stateMachine.PlayOneShotAudio(CbObjectAudioController.ObjectAudio.Spawn);
    }

    public override void ExitState() { }

    public override void UpdateState()
    {
        _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Selected);
    }
}