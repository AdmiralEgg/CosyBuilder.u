using Sirenix.OdinInspector;
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

    [SerializeReference, ReadOnly]
    private bool _detatchOnPointerUp = false;

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
            _stateMachine.GetActiveSnapPoint().InUse = true;
        }

        // TODO: How do we determine where this has been placed?
        //_placedPosition = PlacedPosition.Floor;

        _stateMachine.OnPointerUpEvent += OnPointerUp;
        _stateMachine.OnDetatch += OnDetatch;
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
            _stateMachine.GetActiveSnapPoint().InUse = false;
        }

        _detatchOnPointerUp = false;

        _stateMachine.OnDetatch -= OnDetatch;
        _stateMachine.OnPointerUpEvent -= OnPointerUp;
        _stateMachine.OnScrollEvent -= OnScroll;
    }

    public override void UpdateState()
    {
    }

    private void OnDetatch()
    {
        // how do we play an animation while detatching? 
        _stateMachine.PlayOneShotAudio("Ready to detatch");
        _detatchOnPointerUp = true;
    }

    private void OnPointerUp(PointerEventData data)
    {
        Debug.Log("If we're about to detatch, play a sound and queue selected state");
        
        if (_detatchOnPointerUp)
        {
            _stateMachine.PlayOneShotAudio("Detatching on next frame");
            _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Selected);
        }
    }

    private void OnScroll(PointerEventData data)
    {
        Debug.Log("Scrolling while in placed state - check Focusable");

        // check object is focusable

        // start zooming the camera, if a zoom threshold hits then switch to the VCam
    }
}