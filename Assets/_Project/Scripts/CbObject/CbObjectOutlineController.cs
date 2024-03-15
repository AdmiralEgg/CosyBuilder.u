using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectOutlineController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    float _outlineSizeFree = 2f, _outlineSizeSelected = 3f, _outlineSizePlaced = 3f;

    [SerializeField]
    Color _outlineColorFree, _outlineColorSelected, _outlineColorPlaced;

    Outline _outline;
    CbObjectStateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = GetComponent<CbObjectStateMachine>();
        
        // Add an outline component
        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineAll;
        _outline.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Enter - check state and highlight appropriately. State is: {_stateMachine.GetCurrentState()}");

        switch (_stateMachine.GetCurrentState())
        {
            case CbObjectStateMachine.CbObjectState.Free:
                SetOutlineStateFree();
                break;
            case CbObjectStateMachine.CbObjectState.Selected:
                SetOutlineStateSelected();
                break;
            case CbObjectStateMachine.CbObjectState.Placed:
                SetOutlineStatePlaced();
                break;
        }

        _outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _outline.enabled = false;
    }

    private void SetOutlineStateFree()
    {
        _outline.OutlineWidth = _outlineSizeFree;
        _outline.OutlineColor = _outlineColorFree;
    }

    private void SetOutlineStateSelected()
    {
        _outline.OutlineWidth = _outlineSizeSelected;
        _outline.OutlineColor = _outlineColorSelected;
    }

    private void SetOutlineStatePlaced()
    {
        _outline.OutlineWidth = _outlineSizePlaced;
        _outline.OutlineColor = _outlineColorPlaced;
    }
}