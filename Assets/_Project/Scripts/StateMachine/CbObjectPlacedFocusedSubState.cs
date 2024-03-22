using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectPlacedFocusedSubState : BaseState<CbObjectPlacedSubStateMachine.CbObjectPlacedSubState>
{
    private CbObjectPlacedSubStateMachine _subStateMachine;

    public static Action<CbObjectPlacedSubStateMachine> CbObjectFocused;
    public static Action<CbObjectScriptableData> cbObjectFocusedScriptableData;

    public CbObjectPlacedFocusedSubState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState key, CbObjectPlacedSubStateMachine stateMachine, Rigidbody cbObjectRb) : base(key)
    {
        _subStateMachine = stateMachine;
    }

    public override void EnterState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState lastState)
    {
        _subStateMachine.GetComponent<CbObjectFocusCameraController>().EnableFocusCamera();
        cbObjectFocusedScriptableData?.Invoke(_subStateMachine.GetComponent<CbObjectParameters>().CbObjectData);
        CbObjectFocused?.Invoke(_subStateMachine);
    }

    public override void ExitState()
    {
        // turn off cameras
        _subStateMachine.GetComponent<CbObjectFocusCameraController>().DisableFocusCamera();
    }

    public override void UpdateState() { }
}