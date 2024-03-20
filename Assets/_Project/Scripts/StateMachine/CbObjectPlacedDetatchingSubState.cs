using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectPlacedDetatchingSubState : BaseState<CbObjectPlacedSubStateMachine.CbObjectPlacedSubState>
{
    private CbObjectPlacedSubStateMachine _subStateMachine;
    private Rigidbody _cbObjectRb;
    private CancellationTokenSource _ctSource;
    private bool _readyToDetatch;

    public CbObjectPlacedDetatchingSubState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState key, CbObjectPlacedSubStateMachine stateMachine, Rigidbody cbObjectRb) : base(key)
    {
        _subStateMachine = stateMachine;
        _cbObjectRb = cbObjectRb;
    }

    public override void EnterState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState lastState)
    {
        _readyToDetatch = false;
        
        // Don't free rotation Z
        _cbObjectRb.useGravity = false;
        _cbObjectRb.isKinematic = true;
        _cbObjectRb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY;

        _subStateMachine.OnPointerUpEvent += StopDetaching;
        
        StartDetatching();
    }

    private async void StartDetatching()
    {
        Debug.Log("Detatch starting!");
        // wait for a few more seconds, perform animation and audio

        _ctSource = new CancellationTokenSource();
        CancellationToken ct = _ctSource.Token;

        await WaitForHold(ct, _subStateMachine.DetatchHoldAnimationTime);
    }

    private void StopDetaching(PointerEventData data)
    {
        // if hold has completed, we're ready to detatch.
        if (_readyToDetatch)
        {
            _subStateMachine.RevertToPreviousState();
            _subStateMachine.DetatchComplete();
            return;
        }

        _ctSource.Cancel();
        _subStateMachine.RevertToPreviousState();
    }

    private async Task WaitForHold(CancellationToken token, float waitTime)
    {
        float time = 0;

        // zoom in camera?

        while (time < waitTime)
        {
            if (token.IsCancellationRequested == true)
            {
                _ctSource.Dispose();
                return;
            }

            await Task.Yield();
            time += Time.deltaTime;
        }

        _readyToDetatch = true;
    }

    public override void ExitState()
    {
        _ctSource.Dispose();
        _subStateMachine.OnPointerUpEvent -= StopDetaching;
    }

    public override void UpdateState() { }
}