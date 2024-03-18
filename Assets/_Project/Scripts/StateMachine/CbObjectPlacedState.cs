using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectPlacedState : BaseState<CbObjectStateMachine.CbObjectState>
{
    public enum PlacedPosition { Wall, Floor, Object }

    [Header("Placement Data")]
    [SerializeField, ReadOnly]
    private PlacedPosition _placedPosition = PlacedPosition.Floor;

    [SerializeField, ReadOnly, Tooltip("If placed on an object, a ref to the parent object.")]
    private CbObjectData _parentObject;

    [SerializeField, ReadOnly, Tooltip("If placed on a snappoint, a ref to the snappoint.")]
    private SnapPoint _parentSnapPoint;

    private CbObjectStateMachine _stateMachine;

    public CbObjectPlacedState(CbObjectStateMachine.CbObjectState key, CbObjectStateMachine stateMachine) : base(key)
    {
        _stateMachine = stateMachine;
    }

    public override void EnterState(CbObjectStateMachine.CbObjectState lastState)
    {
        // Check if we're inside a snappoint, and link the snappoint and set the state
        if (_placedPosition == PlacedPosition.Wall) 
        {
            _parentSnapPoint = _stateMachine.GetActiveSnapPoint();
            _parentSnapPoint.InUse = true;
        }

        // TODO: How do we determine where this has been placed?
        //_placedPosition = PlacedPosition.Floor;

        _stateMachine.OnPointerDownEvent += OnPointerDown;
        _stateMachine.OnScrollEvent += OnScroll;

        Rigidbody rb = _stateMachine.CbObjectRigidBody;

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        _stateMachine.SetLayers(CbObjectLayerController.LayerState.CbObjectStatic);

        // Switch on required components
        _stateMachine.UpdateRotationComponent(isActive: false);
        _stateMachine.UpdateMovementComponent(isActive: false);
    }

    public override void ExitState()
    {
        if (_placedPosition == PlacedPosition.Wall)
        {
            _parentSnapPoint.InUse = false;
            _parentSnapPoint = null;
        }
        
        _stateMachine.OnPointerDownEvent -= OnPointerDown;
        _stateMachine.OnScrollEvent -= OnScroll;
    }

    public override void UpdateState()
    {
    }

    private void OnPointerDown(PointerEventData data)
    {
        Debug.Log("Clicked while in placed state. If held for long enough, detatch.");
    }

    private void OnScroll(PointerEventData data)
    {
        Debug.Log("Scrolling while in placed state - check Focusable");
    }
}