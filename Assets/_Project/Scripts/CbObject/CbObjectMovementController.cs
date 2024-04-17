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

    //[SerializeField, Tooltip("Maximum speed the object follows the cursor at"), Range(0.15f, 1f)]
    private float _objectCursorFollowSpeed = 0.5f;

    private void Awake()
    {
        _objectData = GetComponent<CbObjectParameters>();
    }

    private void Update()
    {
        if (_objectData.PlacedPosition == CbObjectScriptableData.PlacedPosition.SnapPoint)
        {
            SnapPointRadiusCheck();
        }

        if (_isInsideFreeSnapPoint == false)
        {
            // Check Placable Surface Hit
            Vector3 placeableSurface = GetPlacableSurfaceNormal();

            MoveObject(placeableSurface);
        }
    }

    /// <summary>
    /// Checks whether the cursor is inside a SnapPoint collider, and the SnapPoint collider is not InUse.
    /// If it is, snap the object to the SnapPoint position.
    /// </summary>
    private void SnapPointRadiusCheck()
    {
        RaycastHit snapPointHit = CursorData.GetRaycastHit(CursorData.LayerMaskType.WithinSnapPoint);

        if (snapPointHit.collider == null)
        {
            _isInsideFreeSnapPoint = false;
            _activeSnapPoint = null;
            return;
        }

        SnapPoint snapPoint = snapPointHit.collider.GetComponent<SnapPoint>();

        if (snapPoint.InUse == true)
        {
            _isInsideFreeSnapPoint = false;
            _activeSnapPoint = null;
            return;
        }

        RaycastHit surfaceHit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        Vector3 positionOffsets = new Vector3(
            surfaceHit.normal.x * _objectData.WallOffset,
            surfaceHit.normal.y * _objectData.GroundOffset,
            surfaceHit.normal.z * _objectData.WallOffset
        );

        this.transform.position = (snapPointHit.collider.transform.position + positionOffsets);

        _activeSnapPoint = snapPoint;
        _isInsideFreeSnapPoint = true;
    }

    public void MoveSpawnedObject()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        if (hit.collider != null) 
        { 
            Debug.Log("Spawn hit: " + hit.collider.name);
            this.transform.position = new Vector3(hit.point.x, _objectData.GroundOffset, hit.point.z);
        }
        else
        {
            Debug.LogWarning("No MovementMask hit on initial spawn");
        }
    }

    private void MoveObject(Vector3 placeableSurfaceNormal)
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        // We've hit nothing
        if (hit.collider == null) return;

        Vector3 positionOffsets = Vector3.zero;

        if (placeableSurfaceNormal != Vector3.zero)
        {
            positionOffsets = new Vector3(
                placeableSurfaceNormal.x * _objectData.SurfaceOffset,
                placeableSurfaceNormal.y * _objectData.SurfaceOffset,
                placeableSurfaceNormal.z * _objectData.SurfaceOffset
            );
        }
        else
        {
            positionOffsets = new Vector3(
                hit.normal.x * _objectData.WallOffset,
                hit.normal.y * _objectData.GroundOffset,
                hit.normal.z * _objectData.WallOffset
            );
        }

        Vector3 targetPosition = (hit.point + positionOffsets);
        _objectMovePosition = Vector3.MoveTowards(this.transform.position, targetPosition, _objectCursorFollowSpeed);

        this.transform.position = _objectMovePosition;
    }

    private Vector3 GetPlacableSurfaceNormal()
    {
        RaycastHit placeableSurface = CursorData.GetRaycastHit(CursorData.LayerMaskType.OnPlaceableSurface);

        if (placeableSurface.transform == null)
        {
            return Vector3.zero;
        }

        // if placeable surface is the selected object, ignore it
        if (placeableSurface.transform.gameObject == this.gameObject)
        {
            return Vector3.zero;
        }

        return placeableSurface.normal;
    }

    private void OnDrawGizmos()
    {
        if (_objectMovePosition != null)
        {
            Gizmos.DrawWireSphere(_objectMovePosition, 0.1f);
        }
    }
}