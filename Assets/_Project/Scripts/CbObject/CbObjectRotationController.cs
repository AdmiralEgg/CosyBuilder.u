using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectRotationController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField, ReadOnly]
    private bool _isRotating;

    public bool IsRotating
    {
        get { return _isRotating; }
        set { _isRotating = value; }
    }

    private void Awake()
    {
        IsRotating = false;
    }

    private void OnEnable()
    {
        IsRotating = false;
    }

    private void OnDisable()
    {
        IsRotating = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragged!!");
    }
    
    public void ResetRotation()
    {
        // TODO: Do this in the movement controller!
        // TODO: Snap to position over time?
        // Rotate x and z axis back to original
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsRotating = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsRotating = true;
    }
}