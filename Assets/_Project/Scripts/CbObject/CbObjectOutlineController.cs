using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class CbObjectOutlineController : MonoBehaviour
{
    [SerializeField]
    float _outlineSizeFree = 0.15f, _outlineSizeSelected = 0.2f, _outlineSizePlaced = 0.2f;

    [SerializeField, Tooltip("As the camera gets closer to the object, the outline size should increase. In focus mode, multiply the outline sizes.")]
    float _focusStateOutlineSizeMultiplyer = 15f;

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

    private void NewCbObjectHit(CbObjectParameters cbObject)
    {
        if (TempSelectedStateManager.IsObjectSelected() == true) return;

        if (cbObject == null)
        {
            _outline.enabled = false;
            return;
        }

        if (cbObject.gameObject != this.gameObject)
        {
            _outline.enabled = false;
            return;
        }

        if (cbObject.gameObject == this.gameObject)
        {
            CbObjectStateMachine.CbObjectState currentState = _stateMachine.GetCurrentState();
            UpdateOutlineState(currentState);

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

    private void UpdateOutlineState(CbObjectStateMachine.CbObjectState newState)
    {        
        float _focusMultiplyer = TempSelectedStateManager.GetGameModeState() 
            == GameModeStateMachine.GameModeState.Focus ? _focusStateOutlineSizeMultiplyer : 1;

        switch (newState)
        {
            case CbObjectStateMachine.CbObjectState.Free:
                _outline.OutlineWidth = (_outlineSizeFree * _focusMultiplyer);
                _outline.OutlineColor = _outlineColorFree;
                _outline.OutlineMode = Outline.Mode.OutlineAll;
                _outline.enabled = false;
                break;
            case CbObjectStateMachine.CbObjectState.Selected:
                _outline.OutlineWidth = (_outlineSizeSelected * _focusMultiplyer);
                _outline.OutlineColor = _outlineColorSelected;
                _outline.OutlineMode = Outline.Mode.OutlineAll;
                _outline.enabled = true;
                break;
            case CbObjectStateMachine.CbObjectState.Placed:
                _outline.OutlineWidth = (_outlineSizePlaced * _focusMultiplyer);
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