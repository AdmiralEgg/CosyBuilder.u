using UnityEngine;
using Sirenix.OdinInspector;

public class CbObjectSelectedState : BaseState<CbObjectStateMachine.CbObjectState>
{
    [SerializeField, ReadOnly, Tooltip("Rigidbody to update")]
    private Rigidbody _cbObjectRb;

    [SerializeField, ReadOnly, Tooltip("Reference to the layer controller to allow us to update CbObject layers")]
    private CbObjectLayerController _cbObjectLayerController;

    [SerializeField, ReadOnly, Tooltip("Reference to the audio controller to allow us to trigger audio")]
    private CbObjectAudioController _cbObjectAudioController;

    public CbObjectSelectedState(CbObjectStateMachine.CbObjectState key, Rigidbody cbObjectRb, CbObjectLayerController layerController) : base(key)
    {
        _cbObjectRb = cbObjectRb;
        _cbObjectLayerController = layerController;
    }

    public override void EnterState(CbObjectStateMachine.CbObjectState lastState)
    {
        // TODO: Set the bounds we're currently in
        //CurrentBounds = GameManager.GetObjectMovementBounds();

        _cbObjectRb.useGravity = false;
        _cbObjectRb.isKinematic = true;
        _cbObjectRb.constraints = RigidbodyConstraints.None;

        // Set layer to Object for raycasting
        _cbObjectLayerController.SetLayers(CbObjectLayerController.LayerState.CbObject);

        // If previous state was hanging, this has been detatched. Play an audio cue.
        if (lastState == CbObjectStateMachine.CbObjectState.Placed)
        {
            _cbObjectAudioController.PlayOneShot("Detatch Audio (Placed -> Selected)");
        }

        if (lastState == CbObjectStateMachine.CbObjectState.Free)
        {
            _cbObjectAudioController.PlayOneShot("Select Audio (Free -> Selected)");
        }
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }
}