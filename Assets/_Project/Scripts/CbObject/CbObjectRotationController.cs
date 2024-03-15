using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectRotationController : MonoBehaviour, IDragHandler
{
    [SerializeField, ReadOnly]
    private bool _isRotating = false;

    CbObjectStateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = GetComponent<CbObjectStateMachine>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_stateMachine.GetCurrentState() != CbObjectStateMachine.CbObjectState.Selected) return;
        
        if (eventData.dragging)
        {
            Debug.Log("Dragged - if selected, then start rotating");
        }
    }
    
    public void ResetRotation()
    {
        // TODO: Do this in the movement controller!
        // TODO: Snap to position over time?
        // Rotate x and z axis back to original
        Debug.Log("Resetting CbObject rotation");
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}