using UnityEngine;
using UnityEngine.EventSystems;

public class CbObjectMovementController : MonoBehaviour, IPointerMoveHandler
{
    CbObjectStateMachine _stateMachine;
    
    private void Awake()
    {
        _stateMachine = GetComponent<CbObjectStateMachine>();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_stateMachine.GetCurrentState() != CbObjectStateMachine.CbObjectState.Selected) return;
       
        Debug.Log("Cursor has moved while the object is selected, follow the cursor!");
    }

    private void Update()
    {
        /*
        // If selected, follow the cursor
        RaycastHit hit = CursorData.GetRaycastHitExcludeObjects();

        // we've hit nothing, return
        if (hit.collider == null) return;

        bool canMove = false;
        // Check we're within bounds.
        // If the cursor is NOT hitting our bounds, don't move.
        CursorData.GetRaycastBoundsHits().ForEach(hit =>
        {
            if (hit.collider.gameObject == CurrentBounds)
            {
                canMove = true;
            }
        });

        // If we've hit a wall, but the object isn't hangable, don't move
        if (hit.collider.tag == "Wall" && _isHangable == false)
        {
            canMove = false;
        }

        if (canMove == false)
        {
            //Debug.Log("Movement checks failed, object will not move.");
            return;
        }

        // If touching a wall or snappoint (both have the 'wall' tag), rotate the hangable object
        if (hit.collider.tag == "Wall" && _isHangable == true)
        {
            this.transform.rotation = Quaternion.LookRotation(hit.collider.gameObject.transform.forward);
        }

        // If inside a snap point, with an object that can hang
        if (hit.collider.name == "SnapPoint" && _isHangable == true && hit.collider.GetComponent<SnapPoint>().InUse == false)
        {
            this.transform.position = hit.collider.transform.position;
        }
        else
        {
        
            // TODO: Change this so it takes into account the position of the mesh and we don't have to define minselectionheight
            // Follow the cursor, don't go below the min height
            if (hit.point.y < _minSelectionHeight)
            {
                this.transform.position = new Vector3(hit.point.x, _minSelectionHeight, hit.point.z);
            }
            else
            {
                this.transform.position = hit.point;
            }
        }
        */
    }

}