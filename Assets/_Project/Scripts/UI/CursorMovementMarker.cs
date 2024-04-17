using Shapes;
using System;
using UnityEngine;

public class CursorMovementMarker : MonoBehaviour
{
    enum MarkerState { Default, OutOfBounds }
    
    [SerializeField]
    CbObjectMovementController _selectedObjectMoveController;

    [SerializeField]
    Color _defaultColor = Color.white;

    [SerializeField]
    Color _outOfBoundsColor = Color.red;

    Disc _markerComponent;

    private MarkerState _currentState;
    
    public void Awake()
    {
        GameObject marker = new GameObject();
        marker.name = "marker";
        marker.transform.parent = this.transform;

        _markerComponent = marker.AddComponent<Disc>();
        ConfigureMarkerComponent(MarkerState.Default);
        _markerComponent.enabled = false;

        CbObjectStateMachine.OnStateChange += HandleStateChange;
    }

    private void ConfigureMarkerComponent(MarkerState newState)
    {
        switch (newState)
        {
            case MarkerState.Default:
                _markerComponent.Radius = 0.3f;
                _markerComponent.Thickness = 0.075f;
                _markerComponent.Type = DiscType.Ring;
                _markerComponent.Color = _defaultColor;
                _markerComponent.Dashed = true;
                break;

            case MarkerState.OutOfBounds:
                _markerComponent.Radius = 0.1f;
                _markerComponent.Thickness = 0.075f;
                _markerComponent.Type = DiscType.Ring;
                _markerComponent.Color = _outOfBoundsColor;
                _markerComponent.Dashed = false;
                break;
        }

        _currentState = newState;
    }

    private void Update()
    {
        if (_markerComponent.enabled == true)
        {
            MoveMarker();
            CheckSnapPoint();
        }
    }

    private void CheckSnapPoint()
    {
        Color currentColor = _markerComponent.Color;

        if (_selectedObjectMoveController.IsInsideFreeSnapPoint == true)
        {
            _markerComponent.Color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
        }
        else
        {
            _markerComponent.Color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
        }
    }

    private void MoveMarker()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        this.transform.position = hit.point + (hit.normal * 0.05f);

        // get the normal of the hit point
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
    }

    private void HandleStateChange(CbObjectStateMachine.CbObjectState state)
    {
        if (state == CbObjectStateMachine.CbObjectState.Selected)
        {
            _selectedObjectMoveController = TempSelectedStateManager.SelectedObject.GetComponent<CbObjectMovementController>();

            _markerComponent.enabled = true;
            return;
        }

        _markerComponent.enabled = false;
    }
}