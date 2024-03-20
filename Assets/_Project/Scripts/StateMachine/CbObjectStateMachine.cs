using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectStateMachine : StateMachine<CbObjectStateMachine.CbObjectState>, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private CbObjectState _initialState;

    public enum CbObjectState { Free, Selected, Placed }

    private CbObjectFreeState _cbObjectFreeState;
    private CbObjectSelectedState _cbObjectSelectedState;
    private CbObjectPlacedState _cbObjectPlacedState;

    private Rigidbody _cbObjectRigidBody;

    public Rigidbody CbObjectRigidBody
    {
        get { return _cbObjectRigidBody; }
    }

    private CbObjectLayerController _cbObjectLayerController;
    private CbObjectRotationController _cbObjectRotationController;
    private CbObjectMovementController _cbObjectMovementController;
    private CbObjectAudioController _cbObjectAudioController;
    private CbObjectPlacedSubStateMachine _cbObjectPlacedSubStateMachine;
    private CbObjectData _cbObjectData;

    public Action<PointerEventData> OnPointerDownEvent, OnPointerUpEvent, OnScrollEvent;

    private void Awake()
    {
        _cbObjectData = GetComponent<CbObjectData>();
        _cbObjectRigidBody = GetComponent<Rigidbody>();
        _cbObjectLayerController = GetComponent<CbObjectLayerController>();
        _cbObjectRotationController = GetComponent<CbObjectRotationController>();
        _cbObjectMovementController = GetComponent<CbObjectMovementController>();
        _cbObjectAudioController = GetComponent<CbObjectAudioController>();
        _cbObjectPlacedSubStateMachine = GetComponent<CbObjectPlacedSubStateMachine>();

        _cbObjectPlacedSubStateMachine.enabled = false;

        _cbObjectFreeState = new CbObjectFreeState(CbObjectState.Free, this);
        _cbObjectSelectedState = new CbObjectSelectedState(CbObjectState.Selected, this);
        _cbObjectPlacedState = new CbObjectPlacedState(CbObjectState.Placed, this, _cbObjectPlacedSubStateMachine);

        AddStateToLookup(CbObjectState.Free, _cbObjectFreeState);
        AddStateToLookup(CbObjectState.Selected, _cbObjectSelectedState);
        AddStateToLookup(CbObjectState.Placed, _cbObjectPlacedState);

        CurrentState = LookupState(_initialState);
        QueuedState = LookupState(_initialState);
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

    public CbObjectState GetCurrentState()
    {
        return CurrentState.StateKey;
    }

    public void SetLayers(CbObjectLayerController.LayerState newLayer)
    {
        _cbObjectLayerController.SetLayers(newLayer);
    }

    public void PlayOneShotAudio(string audio)
    {
        _cbObjectAudioController.PlayOneShot(audio);
    }
    
    public void UpdateMovementComponent(bool isActive)
    {
        _cbObjectMovementController.enabled = isActive;
    }

    public void UpdateRotationComponent(bool isActive)
    {
        _cbObjectRotationController.enabled = isActive;
    }

    public SnapPoint GetActiveSnapPoint()
    {
        return _cbObjectMovementController.ActiveSnapPoint;
    }

    public void AffixObjectToSnapPoint(bool affix)
    {
        if (affix == true)
        {
            Debug.Log("moving object to snappoint position");
            this.transform.position = GetActiveSnapPoint().transform.position;
        }
        
        _cbObjectMovementController.ActiveSnapPoint.InUse = affix;
    }

    public void ResetRotation()
    {
        _cbObjectRotationController.ResetToDefaultRotation();
    }

    public bool IsRotating()
    {
        return _cbObjectRotationController.IsRotating;
    }
}