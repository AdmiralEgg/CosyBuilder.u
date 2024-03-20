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
    private CbObjectPlacedSubStateMachine _subStateMachine;

    public CbObjectPlacedState(CbObjectStateMachine.CbObjectState key, CbObjectStateMachine stateMachine, CbObjectPlacedSubStateMachine subStateMachine) : base(key)
    {
        _stateMachine = stateMachine;
        _subStateMachine = subStateMachine;
    }

    public override void EnterState(CbObjectStateMachine.CbObjectState lastState)
    {
        // Check if we're inside a snappoint, and link the snappoint and set the state
        if (_placedPosition == PlacedPosition.Wall) 
        {
            _stateMachine.GetActiveSnapPoint().InUse = true;
        }

        _subStateMachine.enabled = true;

        _stateMachine.SetLayers(CbObjectLayerController.LayerState.CbObjectStatic);

        // Switch on required components
        _stateMachine.UpdateRotationComponent(isActive: false);
        _stateMachine.UpdateMovementComponent(isActive: false);
    }

    public override void ExitState()
    {
        _subStateMachine.enabled = false;

        if (_placedPosition == PlacedPosition.Wall)
        {
            _stateMachine.GetActiveSnapPoint().InUse = false;
        }
    }

    public override void UpdateState() { }
}