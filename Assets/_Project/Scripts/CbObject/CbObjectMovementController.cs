using MoreMountains.Tools;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectMovementController : MonoBehaviour
{
    CbObjectData _objectData;
    CbObjectRotationController _objectRotationController;

    [SerializeField, MMReadOnly]
    private bool _isInsideFreeSnapPoint = false;

    public bool IsInsideFreeSnapPoint
    {
        get { return _isInsideFreeSnapPoint; }
    }

    [SerializeField, MMReadOnly]
    private SnapPoint _activeSnapPoint;

    public SnapPoint ActiveSnapPoint
    {
        get { return _activeSnapPoint; }
        set { _activeSnapPoint = value; }
    }

    private void Awake()
    {
        _objectData = GetComponent<CbObjectData>();
        _objectRotationController = GetComponent<CbObjectRotationController>();
    }

    private void Update()
    {
        //CbObjectBoundsCheck();

        // Rotation Check?

        if (_objectData.PlacedPosition == ObjectData.PlacedPosition.SnapPoint)
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
        _isInsideFreeSnapPoint = true;
    }

    private void MoveObject()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        // We've hit nothing, don't move
        if (hit.collider == null) return;

        // TODO: Change this so it takes into account the position of the mesh and we don't have to define minselectionheight
        // Follow the cursor, don't go below the min height
        if (hit.point.y < _objectData.MinSelectionHeight)
        {
            this.transform.position = new Vector3(hit.point.x, _objectData.MinSelectionHeight, hit.point.z);
        }
        else
        {
            this.transform.position = hit.point;
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