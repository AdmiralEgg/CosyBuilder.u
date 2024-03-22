using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CbObjectPlacedSubStateMachine : StateMachine<CbObjectPlacedSubStateMachine.CbObjectPlacedSubState>, IPointerDownHandler, IPointerUpHandler, IScrollHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum CbObjectPlacedSubState { Default, Detatching, Focused }

    [field: SerializeField, Range(0.2f, 0.5f), Tooltip("Time for a hold to register.")]
    private float _detatchHoldStartTime = 0.2f;

    public float DetatchHoldStartTime
    {   
        get { return _detatchHoldStartTime; }
        private set { _detatchHoldStartTime = value; } 
    }

    [field: SerializeField, Range(0.5f, 1f), Tooltip("Time for detatch animation to complete.")]
    private float _detatchHoldAnimationTime = 0.5f;

    public float DetatchHoldAnimationTime 
    {
        get { return _detatchHoldAnimationTime; }
        private set { _detatchHoldAnimationTime = value; }
    }

    [SerializeField]
    private CbObjectPlacedSubState _initialPlacedSubState;

    private CbObjectStateMachine _parentStateMachine;

    private CbObjectPlacedDefaultSubState _defaultSubState;
    private CbObjectPlacedDetatchingSubState _detatchingSubState;
    private CbObjectPlacedFocusedSubState _focusedSubState;

    public Action<PointerEventData> OnPointerDownEvent, OnPointerUpEvent, OnScrollEvent;
    public Action OnSetDetatchStartedOutlineEvent, OnSetDetatchCompletedOutlineEvent, OnPointerEnterEvent, OnPointerExitEvent;

    private void Awake()
    {
        Rigidbody _cbObjectRigidBody = GetComponent<Rigidbody>();
        _parentStateMachine = GetComponent<CbObjectStateMachine>();

        // create substates
        _defaultSubState = new CbObjectPlacedDefaultSubState(CbObjectPlacedSubState.Default, this, _cbObjectRigidBody);
        _detatchingSubState = new CbObjectPlacedDetatchingSubState(CbObjectPlacedSubState.Detatching, this, _cbObjectRigidBody);
        _focusedSubState = new CbObjectPlacedFocusedSubState(CbObjectPlacedSubState.Focused, this, _cbObjectRigidBody);

        AddStateToLookup(CbObjectPlacedSubState.Default, _defaultSubState);
        AddStateToLookup(CbObjectPlacedSubState.Detatching, _detatchingSubState);
        AddStateToLookup(CbObjectPlacedSubState.Focused, _focusedSubState);

        CurrentState = LookupState(_initialPlacedSubState);
        QueuedState = LookupState(_initialPlacedSubState);
    }

    public CbObjectParameters GetObjectData()
    {
        return _parentStateMachine.CbObjectData;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // TODO: This event will be sent to all state machines?!
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

    public void SetDetatchStartedOutline()
    {
        OnSetDetatchStartedOutlineEvent?.Invoke();
    }

    public void SetDetatchCompletedOutline()
    {
        OnSetDetatchCompletedOutlineEvent?.Invoke();
    }

    // Subscribe the UI element to this
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke();
    }
}