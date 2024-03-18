using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectFreeState : BaseState<CbObjectStateMachine.CbObjectState>
{
    [Tooltip("Access to the state machine")]
    private CbObjectStateMachine _stateMachine;

    public CbObjectFreeState(CbObjectStateMachine.CbObjectState key, CbObjectStateMachine stateMachine) : base(key)
    {
        _stateMachine = stateMachine;
    }

    public override void EnterState(CbObjectStateMachine.CbObjectState lastState)
    {
        //CurrentBounds = GameManager.GetObjectMovementBounds();
        _stateMachine.OnPointerUpEvent += OnPointerUp;

        Rigidbody rb = _stateMachine.CbObjectRigidBody;

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;

        _stateMachine.SetLayers(CbObjectLayerController.LayerState.CbObject);

        _stateMachine.UpdateRotationComponent(isActive: false);
        _stateMachine.UpdateMovementComponent(isActive: false);
    }

    private void OnPointerUp(PointerEventData data)
    {
        _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Selected);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }
}