using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectPlacedSubStateMachine : StateMachine<CbObjectPlacedSubStateMachine.CbObjectPlacedSubState>, IPointerDownHandler, IPointerUpHandler, IScrollHandler
{
    public enum CbObjectPlacedSubState { Default, Detatching }

    [field: SerializeField, Range(0.2f, 0.5f), Tooltip("Time for a hold to register.")]
    public float DetatchHoldStartTime { get; private set; }

    [field: SerializeField, Range(0.5f, 1f), Tooltip("Time for detatch animation to complete.")]
    public float DetatchHoldAnimationTime { get; private set; }

    [SerializeField]
    private CbObjectPlacedSubState _initialPlacedSubState;

    private CbObjectStateMachine _parentStateMachine;

    private CbObjectPlacedDefaultSubState _defaultSubState;
    private CbObjectPlacedDetatchingSubState _detatchingSubState;

    public Action<PointerEventData> OnPointerDownEvent, OnPointerUpEvent, OnScrollEvent;

    private void Awake()
    {
        Rigidbody _cbObjectRigidBody = GetComponent<Rigidbody>();
        _parentStateMachine = GetComponent<CbObjectStateMachine>();

        // create substates
        _defaultSubState = new CbObjectPlacedDefaultSubState(CbObjectPlacedSubState.Default, this, _cbObjectRigidBody);
        _detatchingSubState = new CbObjectPlacedDetatchingSubState(CbObjectPlacedSubState.Detatching, this, _cbObjectRigidBody);

        AddStateToLookup(CbObjectPlacedSubState.Default, _defaultSubState);
        AddStateToLookup(CbObjectPlacedSubState.Detatching, _detatchingSubState);

        CurrentState = LookupState(_initialPlacedSubState);
        QueuedState = LookupState(_initialPlacedSubState);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        OnScrollEvent?.Invoke(eventData);
    }

    public void DetatchComplete()
    {
        Debug.Log("Detatch complete!");
        _parentStateMachine.QueueNextState(CbObjectStateMachine.CbObjectState.Selected);
    }
}