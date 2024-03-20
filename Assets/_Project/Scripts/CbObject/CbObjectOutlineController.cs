using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectOutlineController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    float _outlineSizeFree = 2f, _outlineSizeSelected = 3f, _outlineSizePlaced = 3f;

    [SerializeField]
    Color _outlineColorFree, _outlineColorSelected, _outlineColorPlaced, _outlineColorDetatchReady;

    Outline _outline;
    CbObjectStateMachine _stateMachine;
    CbObjectPlacedSubStateMachine _placedSubStateMachine;

    private void Awake()
    {
        _stateMachine = GetComponent<CbObjectStateMachine>();
        _placedSubStateMachine = GetComponent<CbObjectPlacedSubStateMachine>();

        // Add an outline component
        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineAll;
        _outline.enabled = false;
    }

    private void OnEnable()
    {
        _stateMachine.OnStateChange += UpdateOutlineState;
        _placedSubStateMachine.OnDetatchCompleted += SetDetatchOutline;
    }

    private void OnDisable()
    {
        _stateMachine.OnStateChange -= UpdateOutlineState;
        _placedSubStateMachine.OnDetatchCompleted -= SetDetatchOutline;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateOutlineState(_stateMachine.GetCurrentState());
        _outline.enabled = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // If we're in the selected state, don't turn off the outline
        if (_stateMachine.GetCurrentState() == CbObjectStateMachine.CbObjectState.Selected) return;
        
        _outline.enabled = false;
    }

    private void UpdateOutlineState(CbObjectStateMachine.CbObjectState newState)
    {
        switch (newState)
        {
            case CbObjectStateMachine.CbObjectState.Free:
                _outline.OutlineWidth = _outlineSizeFree;
                _outline.OutlineColor = _outlineColorFree;
                break;
            case CbObjectStateMachine.CbObjectState.Selected:
                _outline.OutlineWidth = _outlineSizeSelected;
                _outline.OutlineColor = _outlineColorSelected;
                break;
            case CbObjectStateMachine.CbObjectState.Placed:
                _outline.OutlineWidth = _outlineSizePlaced;
                _outline.OutlineColor = _outlineColorPlaced;
                break;
        }
    }

    private void SetDetatchOutline()
    {
        _outline.OutlineWidth = _outlineSizeFree;
        _outline.OutlineColor = _outlineColorDetatchReady;
    }

    public void HideOutline(bool setHidden)
    {
        if (setHidden)
        {
            _outline.OutlineWidth = 0;
        }
        
        _outline.enabled = true;
    }
}