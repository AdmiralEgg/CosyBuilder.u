using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using System.Collections;
using static CbObjectStateMachine;

/// <summary>
/// Only show/hide with pointer exit and enter
/// State of the CbObject is polled on update, and colouring is handled based on state change.
/// TODO: DetatchStarted, Completed and Cancelled events should be replaced with Sub-states of DetatchCompleted. Placed Sub State should be polled on Update.
/// </summary>

// NOTES: Don't turn outline component on/off, just set alpha to 0. 
// CAN'T DO! - materials still exist, so breaks outline rendering when two outlines meet
public class CbObjectOutlineController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Outline _outline;

    [SerializeField, Tooltip("As the camera gets closer to the object, the outline size should increase. In focus mode, multiply the outline sizes.")]
    float _focusStateOutlineSizeMultiplyer = 15f;

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

    [SerializeField]
    private float _highlightPauseForSeconds = 0.4f;

    [SerializeField, ReadOnly]
    private bool _pauseHighlighting = false;
    
    CbObjectStateMachine _stateMachine;
    CbObjectPlacedSubStateMachine _placedSubStateMachine;

    [SerializeField, ReadOnly]
    private CbObjectStateMachine.CbObjectState _currentState;

    private float _currentWidthMultiplyer;

    private void Awake()
    {
        // Get State machines
        _stateMachine = GetComponent<CbObjectStateMachine>();
        _placedSubStateMachine = GetComponent<CbObjectPlacedSubStateMachine>();

        // Add an outline component
        _outline = gameObject.AddComponent<Outline>();
    }

    private void Start()
    {
        _currentWidthMultiplyer = TempSelectedStateManager.GetGameModeState() == GameModeStateMachine.GameModeState.Focus ? _focusStateOutlineSizeMultiplyer : 1;

        UpdateOutlineState(CbObjectState.Free);
        _outline.enabled = false;
    }

    private void OnEnable()
    {
        _placedSubStateMachine.OnSetDetatchStartedOutlineEvent += SetDetatchStartedOutline;
        _placedSubStateMachine.OnSetDetatchCompletedOutlineEvent += SetDetatchCompletedOutline;
        _placedSubStateMachine.OnDetatchCancelled += SetDetatchCancelled;
        GameModeStateMachine.OnStateChange += RefreshOutlineOnGameStateChange;
    }

    private void Update()
    {
        CbObjectStateMachine.CbObjectState cbObjectState = _stateMachine.GetCurrentState();

        if (_currentState != cbObjectState)
        {
            UpdateOutlineState(cbObjectState);
        }
    }

    private void OnDisable()
    {
        _placedSubStateMachine.OnSetDetatchStartedOutlineEvent -= SetDetatchStartedOutline;
        _placedSubStateMachine.OnSetDetatchCompletedOutlineEvent -= SetDetatchCompletedOutline;
        _placedSubStateMachine.OnDetatchCancelled -= SetDetatchCancelled;
        GameModeStateMachine.OnStateChange -= RefreshOutlineOnGameStateChange;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_pauseHighlighting == true) return;
        if (_placedSubStateMachine.GetCurrentState() == CbObjectPlacedSubStateMachine.CbObjectPlacedSubState.Focused) return;
        if (_currentState == CbObjectStateMachine.CbObjectState.Selected) return;

        _outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_pauseHighlighting == true) return;
        if (_currentState == CbObjectStateMachine.CbObjectState.Selected) return;

        _outline.enabled = false;
    }

    private void RefreshOutlineOnGameStateChange(GameModeStateMachine.GameModeState newState)
    {
        _currentWidthMultiplyer = newState == GameModeStateMachine.GameModeState.Focus ? _focusStateOutlineSizeMultiplyer : 1;

        UpdateOutlineState(_currentState);
    }

    private void UpdateOutlineState(CbObjectStateMachine.CbObjectState newState)
    {
        switch (newState)
        {
            case CbObjectStateMachine.CbObjectState.Free:
                _outline.OutlineWidth = (_outlineSizeFree * _currentWidthMultiplyer);
                _outline.OutlineColor = _outlineColorFree;
                _outline.OutlineMode = Outline.Mode.OutlineAll;
                StartCoroutine(DeactivateAndPauseHighlighting(_highlightPauseForSeconds));
                break;
            case CbObjectStateMachine.CbObjectState.Selected:
                _outline.OutlineWidth = (_outlineSizeSelected * _currentWidthMultiplyer);
                _outline.OutlineColor = _outlineColorSelected;
                _outline.OutlineMode = Outline.Mode.OutlineAll;
                _outline.enabled = true;
                break;
            case CbObjectStateMachine.CbObjectState.Placed:
                _outline.OutlineWidth = (_outlineSizePlaced * _currentWidthMultiplyer);
                _outline.OutlineColor = _outlineColorPlaced;
                _outline.OutlineMode = Outline.Mode.OutlineVisible;
                break;
        }

        _currentState = newState;
    }

    private IEnumerator DeactivateAndPauseHighlighting(float seconds)
    {
        _outline.enabled = false;
        _pauseHighlighting = true;
        yield return new WaitForSeconds(seconds);
        _pauseHighlighting = false;
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

    private void SetDetatchCancelled()
    {
        _outline.OutlineWidth = _outlineSizeFree;
        _outline.OutlineColor = _outlineColorPlaced;
    }
}