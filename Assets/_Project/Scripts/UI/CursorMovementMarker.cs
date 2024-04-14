using Shapes;
using UnityEngine;

public class CursorMovementMarker : MonoBehaviour
{
    [SerializeField]
    CbObjectMovementController _selectedObjectMoveController;

    [SerializeField]
    Color _markerColor = Color.white;

    Disc _markerComponent;
    
    public void Awake()
    {
        GameObject marker = new GameObject();
        marker.name = "marker";
        marker.transform.parent = this.transform;

        _markerComponent = marker.AddComponent<Disc>();
        ConfigureMarkerComponent();

        CbObjectStateMachine.OnStateChange += HandleStateChange;
    }

    private void ConfigureMarkerComponent()
    {
        _markerComponent.Radius = 0.3f;
        _markerComponent.Thickness = 0.075f;
        _markerComponent.Type = DiscType.Ring;
        _markerComponent.Dashed = true;
        
        _markerComponent.enabled = false;
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
        if (_selectedObjectMoveController.IsInsideFreeSnapPoint == true)
        {
            _markerComponent.Color = new Color(_markerColor.r, _markerColor.g, _markerColor.b, 0);
        }
        else
        {
            _markerComponent.Color = new Color(_markerColor.r, _markerColor.g, _markerColor.b, 1);
        }
    }

    private void MoveMarker()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        this.transform.position = hit.point + (hit.normal * 0.01f);

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