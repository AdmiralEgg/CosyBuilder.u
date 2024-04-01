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

    private void MoveObject()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        // We've hit nothing, don't move
        if (hit.collider == null) return;

        // TODO: Change this so it takes into account the position of the mesh and we don't have to define minselectionheight
        // Follow the cursor, don't go below the min height
        if (hit.point.y < _objectData.MinSelectionHeight)
        {
            // this.transform.position = new Vector3(hit.point.x, _objectData.MinSelectionHeight, hit.point.z);
            Vector3 targetPosition = new Vector3(hit.point.x, _objectData.MinSelectionHeight, hit.point.z);

            _objectMovePosition = Vector3.MoveTowards(this.transform.position, targetPosition, 0.06f);
        }
        else
        {
            // this.transform.position = hit.point;
            //_objectMovePosition = hit.point;
            _objectMovePosition = Vector3.MoveTowards(this.transform.position, hit.point, 0.06f);
        }

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