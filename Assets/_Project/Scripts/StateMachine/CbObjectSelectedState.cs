using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CbObjectSelectedState : BaseState<CbObjectStateMachine.CbObjectState>
{
    private CbObjectStateMachine _stateMachine;

    public CbObjectSelectedState(CbObjectStateMachine.CbObjectState key, CbObjectStateMachine stateMachine) : base(key)
    {
        _stateMachine = stateMachine;
    }

    public override void EnterState(CbObjectStateMachine.CbObjectState lastState)
    {
        PlayerInput.GetPlayerByIndex(0).actions["DropOrPlace"].performed += OnDropOrPlace;

        Cursor.visible = false;

        // TODO: Set the bounds we're currently in
        //CurrentBounds = GameManager.GetObjectMovementBounds();

        ConfigureRigidbody();

        // Set layer to Object for raycasting
        _stateMachine.SetLayers(CbObjectLayerController.LayerState.CbObject);

        // Switch on required components
        _stateMachine.UpdateRotationComponent(isActive: true);
        _stateMachine.UpdateMovementComponent(isActive: true);

        _stateMachine.ResetRotation();

        PlayStateChangeAudio(lastState);
    }

    private void OnDropOrPlace(InputAction.CallbackContext data)
    {
        Debug.Log("Drop or place...");
        
        // if over a snappoint, then attach
        if (_stateMachine.GetActiveSnapPoint() != null)
        {
            Debug.Log("Switch to Placed state");
            _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Placed);
            return;
        }
        
        _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Free);
    }

    private void PlayStateChangeAudio(CbObjectStateMachine.CbObjectState lastState)
    {
        // If previous state was hanging, this has been detatched. Play an audio cue.
        if (lastState == CbObjectStateMachine.CbObjectState.Placed)
        {
            _stateMachine.PlayOneShotAudio("Detatch Audio (Placed -> Selected)");
        }

        if (lastState == CbObjectStateMachine.CbObjectState.Free)
        {
            _stateMachine.PlayOneShotAudio("Select Audio (Free -> Selected)");
        }
    }

    private void ConfigureRigidbody()
    {
        Rigidbody rb = _stateMachine.CbObjectRigidBody;

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.None;
    }

    public override void ExitState()
    {
        PlayerInput.GetPlayerByIndex(0).actions["DropOrPlace"].performed -= OnDropOrPlace;
        Cursor.visible = true;
    }

    public override void UpdateState()
    {
    }
}