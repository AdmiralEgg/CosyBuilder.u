using System;
using UnityEngine;

public class GameModeStateMachine : StateMachine<GameModeStateMachine.GameModeState>
{
    [SerializeField]
    private GameModeState _initialState;

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
}