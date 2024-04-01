using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class CbObjectOutlineController : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    float _outlineSizeFree = 0.15f, _outlineSizeSelected = 0.2f, _outlineSizePlaced = 0.2f;

    [SerializeField]
    Color _outlineColorFree = new Color(0.7f, 0.7f, 0.7f, 0.5f);
    [SerializeField]
    Color _outlineColorSelected = new Color(1f, 1f, 1f, 0.8f);
    [SerializeField]
    Color _outlineColorPlaced = new Color(0.73f, 0f, 0f, 0.7f);
    [SerializeField]
    Color _outlineColorDetatchStarted = new Color(0.9f, 0.33f, 0f, 0.5f);
    [SerializeField]
    Color _outlineColorDetatchCompleted = new Color(0.1f, 0.66f, 0f, 0.5f);

    Outline _outline;
    CbObjectStateMachine _stateMachine;
    CbObjectPlacedSubStateMachine _placedSubStateMachine;

    [SerializeField, ReadOnly]
    private CbObjectStateMachine.CbObjectState _lastCheckedState;

    private void Awake()
    {
        _stateMachine = GetComponent<CbObjectStateMachine>();
        _placedSubStateMachine = GetComponent<CbObjectPlacedSubStateMachine>();

        // Add an outline component
        _outline = gameObject.AddComponent<Outline>();
        UpdateOutlineState(CbObjectStateMachine.CbObjectState.Free);
        _outline.enabled = false;

        CursorData.OnCbObjectHit += NewCbObjectHit;
    }

    private void NewCbObjectHit(CbObjectParameters @object)
    {
        if (TempSelectedStateManager.IsObjectSelected() == true) return;

        if (@object == null)
        {
            Debug.Log("Hit nothing, turn off outline");
            _outline.enabled = false;
            return;
        }

        if (@object.gameObject != this.gameObject)
        {
            Debug.Log("Did not hit me, turn off outline");
            _outline.enabled = false;
            return;
        }

        if (@object.gameObject == this.gameObject)
        {
            Debug.Log("Hit me!" + @object.name);
            _outline.enabled = true;
            return;
        }

    }

    private void OnEnable()
    {
        _placedSubStateMachine.OnSetDetatchStartedOutlineEvent += SetDetatchStartedOutline;
        _placedSubStateMachine.OnSetDetatchCompletedOutlineEvent += SetDetatchCompletedOutline;
    }

    private void OnDisable()
    {
        _placedSubStateMachine.OnSetDetatchStartedOutlineEvent -= SetDetatchStartedOutline;
        _placedSubStateMachine.OnSetDetatchCompletedOutlineEvent -= SetDetatchCompletedOutline;
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    // TODO: Check whether a CbObject is already selected!
    //    if (TempSelectedStateManager.IsObjectSelected() == true) return;

    //    _outline.enabled = true;
    //}
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    // If we're in the selected state, don't turn off the outline
    //    if (_stateMachine.GetCurrentState() == CbObjectStateMachine.CbObjectState.Selected) return;
        
    //    _outline.enabled = false;
    //}

    private void UpdateOutlineState(CbObjectStateMachine.CbObjectState newState)
    {
        switch (newState)
        {
            case CbObjectStateMachine.CbObjectState.Free:
                _outline.OutlineWidth = _outlineSizeFree;
                _outline.OutlineColor = _outlineColorFree;
                _outline.OutlineMode = Outline.Mode.OutlineAll;
                _outline.enabled = false;
                break;
            case CbObjectStateMachine.CbObjectState.Selected:
                _outline.OutlineWidth = _outlineSizeSelected;
                _outline.OutlineColor = _outlineColorSelected;
                _outline.OutlineMode = Outline.Mode.OutlineAll;
                _outline.enabled = true;
                break;
            case CbObjectStateMachine.CbObjectState.Placed:
                _outline.OutlineWidth = _outlineSizePlaced;
                _outline.OutlineColor = _outlineColorPlaced;
                _outline.OutlineMode = Outline.Mode.OutlineVisible;
                _outline.enabled = false;
                break;
        }

        _lastCheckedState = newState;
    }

    private void LateUpdate()
    {
        // check the state of the CbObject. Swap outline if state has changed
        CbObjectStateMachine.CbObjectState currentState = _stateMachine.GetCurrentState();

        //Debug.Log($"Current State: {currentState}. Last Known: {_lastCheckedState}");

        if (_lastCheckedState != currentState)
        {
            UpdateOutlineState(currentState);
        }
    }

    private void SetDetatchStartedOutline()
    {
        _outline.OutlineWidth = _outlineSizeFree;
        _outline.OutlineColor = _outlineColorDetatchStarted;
    }

    private void SetDetatchCompletedOutline()
    {
        _outline.OutlineWidth = _outlineSizeFree;
        _outline.OutlineColor = _outlineColorDetatchCompleted;
    }
}