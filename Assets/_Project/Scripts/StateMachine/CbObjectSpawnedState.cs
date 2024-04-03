
using UnityEngine;

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
        // get cursor and move to it.
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);
        _movementController.MoveSpawnedObject();
    }

    public override void ExitState() { }

    public override void UpdateState()
    {
        _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Selected);
    }
}