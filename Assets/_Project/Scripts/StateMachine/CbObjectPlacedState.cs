using Sirenix.OdinInspector;
using UnityEngine;

public class CbObjectPlacedState : BaseState<CbObjectStateMachine.CbObjectState>
{
    public enum PlacedPosition { Wall, Floor, Object }

    [Header("Placement Data")]
    [SerializeField, ReadOnly]
    private PlacedPosition _placedPosition = PlacedPosition.Floor;

    [SerializeField, ReadOnly, Tooltip("If placed on an object, a ref to the parent object.")]
    private CbObjectData _parentObject;

    [SerializeField, ReadOnly, Tooltip("If placed on a snappoint, a ref to the snappoint.")]
    private GameObject _parentSnapPoint;

    [Header("Object Configuration References")]
    [SerializeField, ReadOnly, Tooltip("Rigidbody to update")]
    private Rigidbody _cbObjectRb;

    [SerializeField, ReadOnly, Tooltip("Reference to the layer controller to allow us to update CbObject layers")]
    private CbObjectLayerController _cbObjectLayerController;

    public CbObjectPlacedState(CbObjectStateMachine.CbObjectState key, Rigidbody cbObjectRb, CbObjectLayerController layerController) : base(key)
    {
        _cbObjectRb = cbObjectRb;
        _cbObjectLayerController = layerController;
    }

    public override void EnterState(CbObjectStateMachine.CbObjectState lastState)
    {
        // TODO: How do we determine where this has been placed?
        //_placedPosition = PlacedPosition.Floor;
        
        _cbObjectRb.useGravity = false;
        _cbObjectRb.isKinematic = true;
        _cbObjectRb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        _cbObjectLayerController.SetLayers(CbObjectLayerController.LayerState.CbObjectStatic);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }
}