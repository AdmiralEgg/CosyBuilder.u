using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
{
    protected BaseState<EState> CurrentState;
    protected BaseState<EState> LastState;
    protected BaseState<EState> QueuedState;

    [SerializeField, ReadOnly]
    private string _currentState, _lastState, _queuedState;

    private void Start()
    {
        CurrentState.EnterState(lastState: CurrentState.StateKey);
    }

    private void Update()
    {              
        if (CurrentState.Equals(QueuedState))
        {
            CurrentState.UpdateState();
        }
        else
        {           
            LastState = CurrentState;
            CurrentState.ExitState();
            CurrentState = QueuedState;
            CurrentState.EnterState(lastState: QueuedState.StateKey);
        }

        _currentState = CurrentState.StateKey.ToString();
        _queuedState = QueuedState.StateKey.ToString();
        
        if (LastState != null)
        {
            _lastState = LastState.StateKey.ToString();
        }
    }

    public void QueueNextState(BaseState<EState> stateToQueue)
    {
        QueuedState = stateToQueue;
    }

    public void RevertToPreviousState()
    {
        QueuedState = LastState;
    }
}