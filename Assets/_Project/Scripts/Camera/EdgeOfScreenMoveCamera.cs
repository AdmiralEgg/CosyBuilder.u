using Sirenix.OdinInspector;
using UnityEngine;

public class EdgeOfScreenMoveCamera : MonoBehaviour
{
    [SerializeField, Range(0.9f, 0.999f)]
    float _edgePercentage = 0.98f;

    [SerializeField, ReadOnly]
    float _upperBoundsX, _lowerBoundsX, _upperBoundsY, _lowerBoundsY;

    [SerializeField, ReadOnly]
    float _boundsPercentageOverX, _boundsPercentageOverY;

    [SerializeField]
    float _movementSpeed = 1f;

    [SerializeField, ReadOnly]
    Vector3 _currentMousePosition;

    [SerializeField, ReadOnly]
    float _easedMovementSpeedX, _easedMovementSpeedY;

    [SerializeField, ReadOnly, Range(0.75f, 1)]
    float _minimumMoveSpeed = 0.9f;

    [SerializeField, ReadOnly]
    bool _isMoving;

    private void Start()
    {
        _isMoving = false;
        
        // TODO: This should be in game mode, when Exit modal appears cursor should unlock.
        Cursor.lockState = CursorLockMode.Confined;

        _upperBoundsX = Screen.width * _edgePercentage;
        _lowerBoundsX = Screen.width * (1 - _edgePercentage);
        _upperBoundsY = Screen.height * _edgePercentage;
        _lowerBoundsY = Screen.height * (1 - _edgePercentage);
    }

    private void Update()
    {
        CheckScreenEdge();
    }

    public bool IsScreenEdgeMoving()
    {
        return _isMoving;
    }

    private void CheckScreenEdge()
    {
        float _easedMovementSpeedXLeft = 0;
        float _easedMovementSpeedXRight = 0;
        float _easedMovementSpeedYForward = 0;
        float _easedMovementSpeedYBack = 0;

        _currentMousePosition = Input.mousePosition;

        // if game is unfocused, do nothing
        if (Application.isFocused == false) return;
        if (Cursor.lockState != CursorLockMode.Confined) return;

        // if we're in the editor, do nothing
        if (Application.isEditor == true) return;

        if (_currentMousePosition.x >= _upperBoundsX)
        {
            // Edge of screen X (right)
            _boundsPercentageOverX = Mathf.Clamp(((Input.mousePosition.x - _upperBoundsX) / (Screen.width - _upperBoundsX)), 0, 1); // returns a value between 0 and 1. 
            _easedMovementSpeedXRight = (_movementSpeed * _minimumMoveSpeed) + Mathf.Lerp(0, (_movementSpeed * (1 - _minimumMoveSpeed)), _boundsPercentageOverX);
            
            //this.transform.Translate(Vector3.right * _easedMovementSpeedX * Time.deltaTime, Space.Self);
        }

        if (_currentMousePosition.x <= _lowerBoundsX)
        {
            //Edge of screen X (left)
            _boundsPercentageOverX = Mathf.Clamp(((_lowerBoundsX - Input.mousePosition.x) / _lowerBoundsX), 0, 1);  // returns a value between 0 and 1.

            // Always apply 90% of movement speed, and apply 10% on top the closer we are to the edge for easing.
            _easedMovementSpeedXLeft = (_movementSpeed * _minimumMoveSpeed) + Mathf.Lerp(0, (_movementSpeed * (1 - _minimumMoveSpeed)), _boundsPercentageOverX);
            
            
            //this.transform.Translate(Vector3.left * _easedMovementSpeedX * Time.deltaTime, Space.Self);
        }

        if (_currentMousePosition.y >= _upperBoundsY)
        {
            // Edge of screen Y (up)
            _boundsPercentageOverY = Mathf.Clamp(((Input.mousePosition.y - _upperBoundsY) / (Screen.height - _upperBoundsY)), 0, 1); // returns a value between 0 and 1. 
            _easedMovementSpeedYForward = (_movementSpeed * _minimumMoveSpeed) + Mathf.Lerp(0, (_movementSpeed * (1 - _minimumMoveSpeed)), _boundsPercentageOverY);
            
            
            //this.transform.Translate(Vector3.forward * _easedMovementSpeedY * Time.deltaTime, Space.Self);
        }

        if (_currentMousePosition.y <= _lowerBoundsY)
        {
            //Edge of screen Y (down)
            _boundsPercentageOverY = Mathf.Clamp(((_lowerBoundsY - Input.mousePosition.y) / _lowerBoundsY), 0, 1); // returns a value between 0 and 1. 

            // Always apply 90% of movement speed, and apply 10% on top the closer we are to the edge for easing.
            _easedMovementSpeedYBack = (_movementSpeed * _minimumMoveSpeed) + Mathf.Lerp(0, (_movementSpeed * (1 - _minimumMoveSpeed)), _boundsPercentageOverY);
            
            
            //this.transform.Translate(Vector3.back * _easedMovementSpeedY * Time.deltaTime, Space.Self);
        }

        if (_easedMovementSpeedXLeft != 0 ||
            _easedMovementSpeedXRight != 0 ||
            _easedMovementSpeedYForward != 0 ||
            _easedMovementSpeedYBack != 0)
        {
            this.transform.Translate(Vector3.left * _easedMovementSpeedXLeft * Time.deltaTime, Space.Self);
            this.transform.Translate(Vector3.right * _easedMovementSpeedXRight * Time.deltaTime, Space.Self);
            this.transform.Translate(Vector3.forward * _easedMovementSpeedYForward * Time.deltaTime, Space.Self);
            this.transform.Translate(Vector3.back * _easedMovementSpeedYBack * Time.deltaTime, Space.Self);
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }
    }
}