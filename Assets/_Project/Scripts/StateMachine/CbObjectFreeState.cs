using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public class CbObjectFreeState : BaseState<CbObjectStateMachine.CbObjectState>
{
    [SerializeField, ReadOnly, Tooltip("Rigidbody to update")]
    private Rigidbody _cbObjectRb;

    [SerializeField, ReadOnly, Tooltip("Reference to the layer controller to allow us to update CbObject layers")]
    private CbObjectLayerController _cbObjectLayerController;

    public CbObjectFreeState(CbObjectStateMachine.CbObjectState key, Rigidbody rb, CbObjectLayerController layerController) : base(key)
    {
        _cbObjectRb = rb;
        _cbObjectLayerController = layerController;
    }

    public override void EnterState(CbObjectStateMachine.CbObjectState lastState)
    {
        //CurrentBounds = GameManager.GetObjectMovementBounds();

        _cbObjectRb.useGravity = true;
        _cbObjectRb.isKinematic = false;
        _cbObjectRb.constraints = RigidbodyConstraints.None;

        _cbObjectLayerController.SetLayers(CbObjectLayerController.LayerState.CbObject);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }
}