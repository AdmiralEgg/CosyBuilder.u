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
        
        TempSelectedStateManager.SetSelectedObject(_stateMachine.CbObjectData);

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

        Debug.Log("Into Selected State. Last state was: " + lastState);

        if (lastState == CbObjectStateMachine.CbObjectState.Placed)
        {
            // Detatch Audio (Placed -> Selected)
            _stateMachine.PlayOneShotAudio(CbObjectAudioController.ObjectAudio.Detatched);
        }

        if (lastState == CbObjectStateMachine.CbObjectState.Free)
        {
            // Select Audio (Free -> Selected)
            _stateMachine.PlayOneShotAudio(CbObjectAudioController.ObjectAudio.NotImplemented);
        }
    }

    private void OnDropOrPlace(InputAction.CallbackContext data)
    {
        Debug.Log("Drop or place...");
        
        switch (_stateMachine.CbObjectData.PlacedPosition)
        {
            case CbObjectScriptableData.PlacedPosition.None:
                _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Free);
                break;
            
            case CbObjectScriptableData.PlacedPosition.SnapPoint:
                if (_stateMachine.GetActiveSnapPoint() == null)
                {
                    _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Free);
                    break;
                }

                Debug.Log("Switch to Placed state: SnapPoint");
                _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Placed);
                break;
            case CbObjectScriptableData.PlacedPosition.Floor:

                if (CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask).collider.tag != LayerAndTagValidator.ValidatedTagDataLookup[LayerAndTagValidator.CbTag.Floor])
                {
                    _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Free);
                    break;
                }

                Debug.Log("Switch to Placed state: Floor");
                _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Placed);
                break;
            default:
                _stateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Free);
                break;
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
        TempSelectedStateManager.SetSelectedObject(null);
        PlayerInput.GetPlayerByIndex(0).actions["DropOrPlace"].performed -= OnDropOrPlace;
        Cursor.visible = true;
    }

    public override void UpdateState()
    {
    }
}