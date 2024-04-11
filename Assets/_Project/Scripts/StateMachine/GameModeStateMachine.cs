using ImGuiNET;
using UnityEngine;

public class GameModeStateMachine : StateMachine<GameModeStateMachine.GameModeState>
{
    [SerializeField]
    private GameModeState _initialState;

    [SerializeField]
    private CbObjectPlacedSubStateMachine _currentFocus = null;

    public CbObjectPlacedSubStateMachine CurrentFocus
    {
        get { return _currentFocus; }
        set { _currentFocus = value; }
    }

    public enum GameModeState { Build, Focus, Photo }

    private GameModeBuildState _gameModeBuildState;
    private GameModeFocusState _gameModeFocusState;
    private GameModePhotoState _gameModePhotoState;

    private void Awake()
    {
        _gameModeBuildState = new GameModeBuildState(GameModeState.Build, this);
        _gameModeFocusState = new GameModeFocusState(GameModeState.Focus, this);
        _gameModePhotoState = new GameModePhotoState(GameModeState.Photo, this);

        AddStateToLookup(GameModeState.Build, _gameModeBuildState);
        AddStateToLookup(GameModeState.Focus, _gameModeFocusState);
        AddStateToLookup(GameModeState.Photo, _gameModePhotoState);

        CurrentState = LookupState(_initialState);
        QueuedState = LookupState(_initialState);
    }

    private void OnEnable() => ImGuiUn.Layout += OnImGuiLayout;
    private void OnDisable() => ImGuiUn.Layout -= OnImGuiLayout;

    public GameModeState GetCurrentState()
    {
        return CurrentState.StateKey;
    }

    private void OnImGuiLayout()
    {
        if (ImGui.CollapsingHeader($"GameModeState", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Text($"Current State: {CurrentState}");
            ImGui.Text($"Last State: {LastState}");
        }
    }
}