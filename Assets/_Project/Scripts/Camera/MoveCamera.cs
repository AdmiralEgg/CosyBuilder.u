using System;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MoveCamera : MonoBehaviour
{
    [Header("Generic Movement Parameters")]
    [SerializeField, ReadOnly, Tooltip("Is the camera still lerping")]
    bool _isMoving = false;

    [SerializeField, Tooltip("Amount of time it takes for a movement to complete a lerp")]
    float _movementTime = 2f;

    [SerializeField, ReadOnly, Tooltip("The new position the camera will aim for follow a movement action")]
    Vector3 _newPosition;

    [SerializeField, ReadOnly, Tooltip("The new rotation the camera will aim for follow a movement action")]
    Vector3 _newRotation;

    [SerializeField, ReadOnly, Tooltip("The movement value that will be applied to the camera on this frame")]
    Vector3 _moveLerpValue;

    [Header("Speed Boost")]
    [SerializeField, ReadOnly]
    bool _speedBoostActive;

    [SerializeField, Range(1.1f, 3f)]
    float _speedBoostMultiplyer;

    [SerializeField, ReadOnly]
    float _currentMovementMultiplyer = 1f;

    [Header("Keys Movement")]
    [SerializeField, ReadOnly, Tooltip("Are there more key movement actions to process?")]
    bool _readKeysMovement = false;

    [SerializeField, Tooltip("Speed of movement for the camera")]
    float _keysMovementSpeed = 0.1f;

    [Header("Grab & Drag Movement")]
    [SerializeField, ReadOnly, Tooltip("Are there more drag movement actions to process?")]
    bool _readGrabAndDragMovement = false;

    [SerializeField, Tooltip("Speed of movement for the camera when grabbing and dragging")]
    float _grabMovementSpeed = 0.1f;
    
    [SerializeField, ReadOnly]
    Vector3 _grabStartPoint;

    [SerializeField, ReadOnly]
    Vector3 _grabPositionDeltaClamped;

    [SerializeField]
    float _grabClampMaxLength = 3f;

    [SerializeField, ReadOnly]
    bool _pointerOverInteractableUI;

    InputAction _moveAction;

    EdgeOfScreenMoveCamera _edgeOfScreen;
    RotateCamera _rotateDirectly;
    
    // Shapes indicator gameobjects
    GameObject _grabStartIndicator;
    GameObject _newPositionIndicator;

    [SerializeField]
    private bool _disableVisualIndicators = false;

    void Start()
    {
        _newPosition = transform.position;
        _edgeOfScreen = GetComponent<EdgeOfScreenMoveCamera>();
        _rotateDirectly = GetComponent<RotateCamera>();

        // Initialise Position Indicator
        _newPositionIndicator = new GameObject();       
        Sphere newPositionSphere = _newPositionIndicator.AddComponent<Sphere>();
        newPositionSphere.Radius = 0.1f;
        newPositionSphere.Color = Color.cyan;

        _grabStartIndicator = new GameObject();
        Sphere grabStartSphere = _grabStartIndicator.AddComponent<Sphere>();
        grabStartSphere.Radius = 0.1f;
        grabStartSphere.Color = Color.green;

        if (_disableVisualIndicators)
        {
            newPositionSphere.gameObject.SetActive(false);
            grabStartSphere.gameObject.SetActive(false);
        }

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(0);

        _moveAction = playerInput.actions.FindAction("Move");

        playerInput.actions["Move"].performed += KeysMove;
        playerInput.actions["Move"].canceled += KeysMove;

        playerInput.actions["SnapMove"].performed += SnapMove;

        playerInput.actions["GrabAndDrag"].started += GrabAndDragWithSmoothing;
        playerInput.actions["GrabAndDrag"].canceled += GrabAndDragWithSmoothing;

        playerInput.actions["CameraSpeedBoost"].started += CameraSpeedBoost;
        playerInput.actions["CameraSpeedBoost"].canceled += CameraSpeedBoost;
    }

    private void Update()
    {
        // If we're over some UI, ignore
        if (EventSystem.current.IsPointerOverGameObject())
        {
            _pointerOverInteractableUI = true;
        }
        else
        {
            _pointerOverInteractableUI = false;
        }
    }

    void LateUpdate()
    {
        // If screen edge is moving, reset movement and skip calculations
        if (_edgeOfScreen.IsScreenEdgeMoving() == true)
        {
            _newPosition = transform.position;
        }
        else
        {
            CalculateMovement();
            ShowPositionTarget();
        }
    }

    private void SnapMove(InputAction.CallbackContext context)
    {
        Debug.Log("Start snap move");

        //if (_pointerOverInteractableUI == true) return;

        Debug.Log($"Flick Rotation State: {_rotateDirectly.GetFlickRotationState()}");

        // ignore if a flick rotation is taking place.
        // check flick rotation
        if (_rotateDirectly.GetFlickRotationState() == RotateCamera.FlickRotationState.FlickHeld ||
            _rotateDirectly.GetFlickRotationState() == RotateCamera.FlickRotationState.StartRunning) return;

        Debug.Log("snap past checks");

        if (context.performed)
        {
            _newPosition = GetMousePointOnMovementPlane(Input.mousePosition);
        }
    }

    private void CameraSpeedBoost(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _speedBoostActive = true;
            _currentMovementMultiplyer = _speedBoostMultiplyer;
        }

        if (context.canceled) 
        {
            _speedBoostActive = false;
            _currentMovementMultiplyer = 1;
        }
    }

    private void GrabAndDragWithSmoothing(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _readGrabAndDragMovement = true;
            //_grabStartPoint = GetMousePointOnMovementPlane();
            _grabStartPoint = GetMousePointOnScreen();

            _grabStartIndicator.transform.position = _grabStartPoint;
            _grabStartIndicator.SetActive(true);
        }

        if (context.canceled)
        {
            _readGrabAndDragMovement = false;
            _grabStartIndicator.SetActive(false);
        }
    }

    private Vector3 GetMousePointOnMovementPlane(Vector3 mousePosition, float planeHeight = 0f)
    {
        Plane movementPlane = new Plane(Vector3.up, Vector3.zero);
        Ray r = Camera.main.ScreenPointToRay(mousePosition);

        float enter;

        if (movementPlane.Raycast(r, out enter))
        {
            return new Vector3(r.GetPoint(enter).x, planeHeight, r.GetPoint(enter).z);
        }

        return Vector3.zero;
    }

    private Vector3 GetMousePointOnScreen()
    {
        return Input.mousePosition;
    }

    private void KeysMove(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            _readKeysMovement = true;
        }

        if (context.canceled)
        {
            _readKeysMovement = false;
        }
    }

    private void CalculateMovement()
    {
        if (_readKeysMovement)
        {
            Vector2 movementInput = _moveAction.ReadValue<Vector2>();
            _newPosition += ((transform.forward * movementInput.y) * _keysMovementSpeed * _currentMovementMultiplyer * Time.deltaTime);
            _newPosition += ((transform.right * movementInput.x) * _keysMovementSpeed * _currentMovementMultiplyer * Time.deltaTime);
        }

        if (_readGrabAndDragMovement)
        {
            Vector3 currentPosition = GetMousePointOnMovementPlane(Input.mousePosition);

            Vector3 grabPositionDelta = currentPosition - GetMousePointOnMovementPlane(_grabStartPoint);

            // Clamp the position
            _grabPositionDeltaClamped = Vector3.ClampMagnitude(grabPositionDelta, _grabClampMaxLength);

            _newPosition = transform.position + new Vector3 (_grabPositionDeltaClamped.x * _grabMovementSpeed * _currentMovementMultiplyer, 0, _grabPositionDeltaClamped.z * _grabMovementSpeed * _currentMovementMultiplyer);
        }

        _moveLerpValue = Vector3.Lerp(transform.position, _newPosition, _movementTime * Time.deltaTime);
        this.transform.position = _moveLerpValue;

        if (Vector3.Distance(transform.position, _newPosition) > 0.01f)
        {
            _isMoving = true;
        }
        else
        {
            // Snap to final position and stop.
            transform.position = _newPosition;
            _isMoving = false;
        }
    }

    private void ShowPositionTarget()
    {
        _newPositionIndicator.transform.position = _newPosition;
    }
}