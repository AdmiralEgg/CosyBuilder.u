using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameModeFocusState : BaseState<GameModeStateMachine.GameModeState>
{
    [SerializeField]
    public CbObjectData CurrentFocus = null;

    [SerializeField]
    public List<CbObjectData> FocusTree = new List<CbObjectData>();

    [SerializeField]
    public int FocusTreeDepth
    {
        get { return FocusTree.Count; }
    }

    private GameModeStateMachine _stateMachine;

    public GameModeFocusState(GameModeStateMachine.GameModeState key, GameModeStateMachine stateMachine) : base(key)
    {
        _stateMachine = stateMachine;
    }

    public override void EnterState(GameModeStateMachine.GameModeState lastState)
    {
        // Subscribe to the Focus Revert button (space, or scroll wheel down)
        PlayerInput.GetPlayerByIndex(0).actions["SwitchView"].performed += OnRevertFocus;
        CbObjectPlacedFocusedSubState.CbObjectFocused += OnObjectFocusEvent;
    }

    private void OnObjectFocusEvent(CbObjectData data)
    {
        // add object to the focus tree, switch camera to that.
    }

    private void OnRevertFocus(InputAction.CallbackContext context)
    {
        Debug.Log("Reverting focus, pick the next off the Focus tree and switch camera to that. If no more on the tree, queue build mode");
    }

    public override void ExitState()
    {
    }

    public override void UpdateState() { }
}