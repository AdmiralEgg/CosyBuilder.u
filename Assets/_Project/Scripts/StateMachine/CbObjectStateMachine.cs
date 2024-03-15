using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectStateMachine : StateMachine<CbObjectStateMachine.CbObjectState>, IPointerDownHandler, IScrollHandler
{
    [SerializeField]
    private CbObjectState _initialState;

    private Dictionary<CbObjectState, BaseState<CbObjectState>> _stateLookup = new Dictionary<CbObjectState, BaseState<CbObjectState>>();
    
    public enum CbObjectState { Free, Selected, Placed }

    private CbObjectFreeState _cbObjectFreeState;
    private CbObjectSelectedState _cbObjectSelectedState;
    private CbObjectPlacedState _cbObjectPlacedState;

    private Rigidbody _cbObjectRigidBody;
    private CbObjectLayerController _cbObjectLayerController;
    private CbObjectRotationController _cbObjectRotationController;
    private CbObjectData _cbObjectData;

    private void Awake()
    {
        _cbObjectRigidBody = GetComponent<Rigidbody>();
        _cbObjectLayerController = GetComponent<CbObjectLayerController>();
        _cbObjectRotationController = GetComponent<CbObjectRotationController>();
        _cbObjectData = GetComponent<CbObjectData>();

        _cbObjectFreeState = new CbObjectFreeState(CbObjectState.Free, _cbObjectRigidBody, _cbObjectLayerController);
        _cbObjectSelectedState = new CbObjectSelectedState(CbObjectState.Selected, _cbObjectRigidBody, _cbObjectLayerController);
        _cbObjectPlacedState = new CbObjectPlacedState(CbObjectState.Placed, _cbObjectRigidBody, _cbObjectLayerController);

        _stateLookup.Add(CbObjectState.Free, _cbObjectFreeState);
        _stateLookup.Add(CbObjectState.Selected, _cbObjectSelectedState);
        _stateLookup.Add(CbObjectState.Placed, _cbObjectPlacedState);

        CurrentState = _stateLookup[_initialState];
        QueuedState = _stateLookup[_initialState];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (CurrentState.StateKey)
        {
            case CbObjectState.Free:
                Debug.Log("Clicked object in Free mode, select it");
                _cbObjectRotationController.ResetRotation();
                QueuedState = _stateLookup[CbObjectState.Selected];
                break;
            case CbObjectState.Selected:
                Debug.Log("Clicked while in Selected mode, switch back to Free");
                QueuedState = _stateLookup[CbObjectState.Free];
                break;
            case CbObjectState.Placed:
                Debug.Log("Clicked a placed object, wait for holding");
                break;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (CurrentState.StateKey != CbObjectState.Placed) return;
        if (_cbObjectData.IsFocusable != true) return;

        Debug.Log("Scrolled - is placed and focusable, then focus");
    }

    public CbObjectState GetCurrentState()
    {
        return CurrentState.StateKey;
    }
}
