using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CbObjectPlacedDefaultSubState : BaseState<CbObjectPlacedSubStateMachine.CbObjectPlacedSubState>
{
    private CbObjectPlacedSubStateMachine _subStateMachine;
    private Rigidbody _cbObjectRb;

    private CancellationTokenSource _ctSource;

    private bool _isPointerHovering;

    public CbObjectPlacedDefaultSubState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState key, CbObjectPlacedSubStateMachine stateMachine, Rigidbody cbObjectRb) : base(key)
    {
        _subStateMachine = stateMachine;
        _cbObjectRb = cbObjectRb;
    }

    public override void EnterState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState lastState)
    {
        _isPointerHovering = false;
        
        PlayerInput.GetPlayerByIndex(0).actions["SwitchView"].performed += SwitchToFocusView;

        _subStateMachine.OnScrollEvent += OnScroll;
        _subStateMachine.OnPointerDownEvent += OnPointerDown;
        _subStateMachine.OnPointerUpEvent += OnPointerUp;
        _subStateMachine.OnPointerEnterEvent += OnPointerEnter;
        _subStateMachine.OnPointerExitEvent += OnPointerExit;

        SetPlacedPosition();
        ConfigureRigidbody();
    }

    private void SetPlacedPosition()
    {
        // before fixing, move to the cursor marker
        switch (_subStateMachine.GetObjectData().PlacedPosition)
        {
            case CbObjectScriptableData.PlacedPosition.SnapPoint:
                RaycastHit snapPointhit = CursorData.GetRaycastHit(CursorData.LayerMaskType.WithinSnapPoint);
                _subStateMachine.transform.position = snapPointhit.collider.GetComponent<SnapPoint>().transform.position;
                break;
            case CbObjectScriptableData.PlacedPosition.Floor:
                RaycastHit movementHit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);
                _subStateMachine.transform.position = movementHit.point;
                break;
        }
    }

    private void ConfigureRigidbody()
    {
        _cbObjectRb.useGravity = false;
        _cbObjectRb.isKinematic = true;

        // TODO: Why are we not setting FreezeRotation?
        _cbObjectRb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void SwitchToFocusView(InputAction.CallbackContext context)
    {
        if (_isPointerHovering)
        {
            UnityEngine.Debug.Log("Immediate switch to Focus view");
            _subStateMachine.QueueNextState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState.Focused);
        }
    }

    private void OnPointerEnter()
    {
        _isPointerHovering = true;
    }

    private void OnPointerExit()
    {
        _isPointerHovering = false;
    }

    private void OnPointerUp(PointerEventData data)
    {
        _ctSource.Cancel();
    }

    private async void OnPointerDown(PointerEventData data)
    {
        // start a 2 second wait, cancel if a pointer up is heard
        _ctSource = new CancellationTokenSource();
        CancellationToken ct = _ctSource.Token;
        
        await WaitForDetatchStart(ct, _subStateMachine.DetatchHoldStartTime);
    }

    private async Task WaitForDetatchStart(CancellationToken token, float waitTime)
    {
        float time = 0;

        // TODO: Focus camera on object, and slow zoom
        while (time < waitTime)
        {
            if (token.IsCancellationRequested == true) return;

            await Task.Yield();
            time += Time.deltaTime;
        }

        _subStateMachine.SetDetatchStartedOutline();
        _subStateMachine.QueueNextState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState.Detatching);
    }

    public override void ExitState() 
    {
        PlayerInput.GetPlayerByIndex(0).actions["SwitchView"].performed -= SwitchToFocusView;
        _subStateMachine.OnScrollEvent -= OnScroll;
        _subStateMachine.OnPointerDownEvent -= OnPointerDown;
        _subStateMachine.OnPointerUpEvent -= OnPointerUp;
        _subStateMachine.OnPointerEnterEvent -= OnPointerEnter;
        _subStateMachine.OnPointerExitEvent -= OnPointerExit;
    }

    public override void UpdateState() { }

    private void OnScroll(PointerEventData data)
    {
        // Check scroll up
        if (data.scrollDelta.y <= 0) return;

        // check object is focusable
        if (_subStateMachine.GetObjectData().IsFocusable == false) return;

        // start zooming the camera, if a zoom threshold hits then switch to the VCam
        _subStateMachine.QueueNextState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState.Focused);
    }
}