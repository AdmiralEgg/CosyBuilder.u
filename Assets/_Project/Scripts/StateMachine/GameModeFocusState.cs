using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using ImGuiNET;
using System.Xml.Linq;
using Shapes;
using UnityEditor.Rendering;

public class GameModeFocusState : BaseState<GameModeStateMachine.GameModeState>
{
    public List<CbObjectPlacedSubStateMachine> FocusList = new List<CbObjectPlacedSubStateMachine>();
    public int FocusListDepth
    {
        get { return FocusList.Count; }
    }

    private GameModeStateMachine _stateMachine;

    public GameModeFocusState(GameModeStateMachine.GameModeState key, GameModeStateMachine stateMachine) : base(key)
    {
        _stateMachine = stateMachine;
    }

    public override void EnterState(GameModeStateMachine.GameModeState lastState)
    {
        // Get the CurrentFocus set by the BuildState
        FocusListAdd(_stateMachine.CurrentFocus);
        
        // Subscribe to the Focus Revert button (space, or scroll wheel down)
        PlayerInput.GetPlayerByIndex(0).actions["SwitchView"].performed += OnRevertFocus;
        PlayerInput.GetPlayerByIndex(0).actions["FocusModeRevert"].performed += OnRevertFocus;
        CbObjectPlacedFocusedSubState.CbObjectFocused += FocusListAdd;
        ImGuiUn.Layout += OnImGuiLayout;
    }

    private void FocusListAdd(CbObjectPlacedSubStateMachine objectStateMachine)
    {
        FocusList.Insert(0, objectStateMachine);

        // set the current focus
        _stateMachine.CurrentFocus = objectStateMachine;
    }

    private void OnRevertFocus(InputAction.CallbackContext context)
    {
        // If object is selected, ignore the revert request
        if (TempSelectedStateManager.IsObjectSelected() == true) return;
        
        FocusList[0].QueueNextState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState.Default);

        // Pop the top item from the FocusTree.
        FocusList.RemoveAt(0);

        if (FocusList.Count == 0)
        {
            _stateMachine.QueueNextState(GameModeStateMachine.GameModeState.Build);
        }
        else
        {
            FocusList[0].QueueNextState(CbObjectPlacedSubStateMachine.CbObjectPlacedSubState.Focused);
        }
    }

    public override void ExitState()
    {
        _stateMachine.CurrentFocus = null;
        FocusList.Clear();

        PlayerInput.GetPlayerByIndex(0).actions["SwitchView"].performed -= OnRevertFocus;
        PlayerInput.GetPlayerByIndex(0).actions["FocusModeRevert"].performed -= OnRevertFocus;
        CbObjectPlacedFocusedSubState.CbObjectFocused -= FocusListAdd;

        ImGuiUn.Layout -= OnImGuiLayout;
    }

    public override void UpdateState() { }

    private void OnImGuiLayout()
    {
        if (ImGui.CollapsingHeader($"FocusStats", ImGuiTreeNodeFlags.DefaultOpen))
        {
            string focusObjectName = "None";

            if (_stateMachine.CurrentFocus != null) focusObjectName = _stateMachine.CurrentFocus.gameObject.name;
            
            ImGui.Text($"Current Focus: {focusObjectName}");
            ImGui.Text($"Focus List Count: {FocusListDepth}");
        }
    }
}