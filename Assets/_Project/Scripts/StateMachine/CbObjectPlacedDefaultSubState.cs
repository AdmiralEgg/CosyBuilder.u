using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectPlacedDefaultSubState : BaseState<CbObjectPlacedSubStateMachine.CbObjectPlacedSubState>
{
    private CbObjectPlacedSubStateMachine _subStateMachine;
    private Rigidbody _cbObjectRb;

    private CancellationTokenSource _ctSource;

    public CbObjectPlacedDefaultSubState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState key, CbObjectPlacedSubStateMachine stateMachine, Rigidbody cbObjectRb) : base(key)
    {
        _subStateMachine = stateMachine;
        _cbObjectRb = cbObjectRb;
    }

    public override void EnterState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState lastState)
    {
        _cbObjectRb.useGravity = false;
        _cbObjectRb.isKinematic = true;

        // TODO: Why are we not setting FreezeRotation?
        _cbObjectRb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        _subStateMachine.OnScrollEvent += OnScroll;
        _subStateMachine.OnPointerDownEvent += OnPointerDown;
        _subStateMachine.OnPointerUpEvent += OnPointerUp;
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
        _ctSource.Dispose();
        _subStateMachine.OnScrollEvent -= OnScroll;
        _subStateMachine.OnPointerDownEvent -= OnPointerDown;
        _subStateMachine.OnPointerUpEvent -= OnPointerUp;
    }

    public override void UpdateState() { }

    private void OnScroll(PointerEventData data)
    {
        UnityEngine.Debug.Log("Scrolling while in placed state - check Focusable");

        // check object is focusable
        // start zooming the camera, if a zoom threshold hits then switch to the VCam
    }
}