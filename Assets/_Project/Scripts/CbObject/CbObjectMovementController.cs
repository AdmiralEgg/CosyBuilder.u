using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using TMPro;

public class CbObjectMovementController : MonoBehaviour
{
    CbObjectParameters _objectData;

    [SerializeField, ReadOnly]
    private bool _isInsideFreeSnapPoint = false;

    public bool IsInsideFreeSnapPoint
    {
        get { return _isInsideFreeSnapPoint; }
    }

    [SerializeField, ReadOnly]
    private SnapPoint _activeSnapPoint;

    private Vector3 _objectMovePosition;

    public SnapPoint ActiveSnapPoint
    {
        get { return _activeSnapPoint; }
        set { _activeSnapPoint = value; }
    }

    private void Awake()
    {
        _objectData = GetComponent<CbObjectParameters>();
    }

    private void Update()
    {
        //CbObjectBoundsCheck();
        if (_objectData.PlacedPosition == CbObjectScriptableData.PlacedPosition.SnapPoint)
        {
            SnapPointRadiusCheck();
        }
        
        if (_isInsideFreeSnapPoint == false)
        {
            MoveObject();
        }
    }

    /// <summary>
    /// Checks whether the cursor is inside a SnapPoint collider, and the SnapPoint collider is not InUse.
    /// If it is, snap the object to the SnapPoint position.
    /// </summary>
    private void SnapPointRadiusCheck()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.WithinSnapPoint);

        if (hit.collider == null)
        {
            _isInsideFreeSnapPoint = false;
            _activeSnapPoint = null;
            return;
        }

        SnapPoint snapPoint = hit.collider.GetComponent<SnapPoint>();

        if (snapPoint.InUse == true)
        {
            _isInsideFreeSnapPoint = false;
            _activeSnapPoint = null;
            return;
        }

        _activeSnapPoint = snapPoint;

        this.transform.position = hit.collider.transform.position;

        // TODO: We want a hover over the snappoint before placing down
        //this.transform.position = new Vector3
        //(
        //    hit.collider.transform.position.x,
        //    hit.collider.transform.position.y,
        //    transform.position.z
        //);

        _isInsideFreeSnapPoint = true;
    }

    public void MoveSpawnedObject()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        this.transform.position = new Vector3(hit.point.x, _objectData.GroundOffset, hit.point.z);
    }

    private void MoveObject()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        // We've hit nothing
        if (hit.collider == null) return;

        // TODO: Add offsets for Ground and Surface faces. 
        
        Vector3 positionOffsets = new Vector3(
            hit.normal.x * _objectData.WallOffset,
            hit.normal.y * _objectData.GroundOffset,
            hit.normal.z * _objectData.WallOffset
        );

        Vector3 targetPosition = (hit.point + positionOffsets);
        _objectMovePosition = Vector3.MoveTowards(this.transform.position, targetPosition, 0.15f);

        this.transform.position = _objectMovePosition;
    }

    private void OnDrawGizmos()
    {
        if (_objectMovePosition != null)
        {
            Gizmos.DrawWireSphere(_objectMovePosition, 0.1f);
        }
    }

    private static void CbObjectBoundsCheck()
    {
        //// Check we're within bounds.
        //// If the cursor is NOT hitting our bounds, don't move.
        //CursorData.GetRaycastBoundsHits().ForEach(hit =>
        //{
        //    if (hit.collider.gameObject == CurrentBounds)
        //    {
        //        canMove = true;
        //    }
        //});
    }
}