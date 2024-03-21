using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectPlacedFocusedSubState : BaseState<CbObjectPlacedSubStateMachine.CbObjectPlacedSubState>
{
    private CbObjectPlacedSubStateMachine _subStateMachine;

    public static Action<CbObjectData> CbObjectFocused;

    public CbObjectPlacedFocusedSubState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState key, CbObjectPlacedSubStateMachine stateMachine, Rigidbody cbObjectRb) : base(key)
    {
        _subStateMachine = stateMachine;
    }

    public override void EnterState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState lastState)
    {
        CbObjectFocused?.Invoke(_subStateMachine.GetObjectData());
    }

    public override void ExitState()
    {
    }

    public override void UpdateState() { }
}