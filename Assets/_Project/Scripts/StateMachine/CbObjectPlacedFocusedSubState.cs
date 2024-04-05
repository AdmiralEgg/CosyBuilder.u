using System;
using UnityEngine;

public class CbObjectPlacedFocusedSubState : BaseState<CbObjectPlacedSubStateMachine.CbObjectPlacedSubState>
{
    private CbObjectPlacedSubStateMachine _subStateMachine;
    private CbObjectFocusCameraController _focusController;

    public static Action<CbObjectPlacedSubStateMachine> CbObjectFocused;
    public static Action<CbObjectScriptableData> CbObjectFocusedScriptableData;

    public CbObjectPlacedFocusedSubState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState key, CbObjectPlacedSubStateMachine stateMachine, CbObjectFocusCameraController focusController) : base(key)
    {
        _subStateMachine = stateMachine;
        _focusController = focusController;
    }

    public override void EnterState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState lastState)
    {
        if (_subStateMachine.GetObjectData().FocusCameraType == CbObjectScriptableData.FocusCameraType.Set)
        {
            GameObject focusedObject = _subStateMachine.GetFocusedGameObject();
            _focusController.EnableFocusCamera(focusedObject);
        }
        else
        {
            _focusController.EnableFocusCamera();
        }
        
        CbObjectFocusedScriptableData?.Invoke(_subStateMachine.GetComponent<CbObjectParameters>().CbObjectData);
        CbObjectFocused?.Invoke(_subStateMachine);
    }

    public override void ExitState()
    {
        // turn off cameras
        _subStateMachine.GetComponent<CbObjectFocusCameraController>().DisableFocusCamera();
    }

    public override void UpdateState() { }
}