using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CbObjectRotationController : MonoBehaviour
{
    public enum SnapRotationType { None, Quick, Reset, Wall }
    
    [SerializeField]
    private GameObject _rotationIndicator;

    [SerializeField, ReadOnly]
    private Quaternion _currentRotationDebug;

    [Header("Player controlled rotation")]
    [SerializeField, ReadOnly]
    private bool _isRotating = false;

    public bool IsRotating
    {
        get { return _isRotating; }
        set { _isRotating = value; }
    }

    [SerializeField, ReadOnly]
    private Quaternion _targetRotation;

    [SerializeField]
    private float _rotationSpeedMultiplyer = 1.2f;

    [Header("Cb controlled rotation")]
    [SerializeField, ReadOnly]
    private SnapRotationType _activeSnapRotationType = SnapRotationType.None;

    public bool IsSnapRotating
    {
        get 
        {
            if (_activeSnapRotationType == SnapRotationType.None) return false;
            return true;
        }
    }

    [SerializeField, ReadOnly]
    private Quaternion _targetSnapRotation;

    [SerializeField]
    private float _snapRotationDuration = 1.5f;
    [SerializeField]
    private float _quickRotationDuration = 0.2f;

    private CbObjectAudioController _audioController;
    private Coroutine _snapRotateCoroutine;

    private void Awake()
    {
        _audioController = GetComponent<CbObjectAudioController>();
    }

    private void OnEnable()
    {
        PlayerInput.GetPlayerByIndex(0).actions["QuickRotateObject"].performed += OnQuickRotate;

        // TODO: Move back to IScrollHander (or a custom version of this) if possible. See note on 19/03.
        PlayerInput.GetPlayerByIndex(0).actions["RotateObject"].performed += OnRotate;
    }

    private void OnDisable()
    {
        PlayerInput.GetPlayerByIndex(0).actions["QuickRotateObject"].performed -= OnQuickRotate;
        PlayerInput.GetPlayerByIndex(0).actions["RotateObject"].performed -= OnRotate;
    }

    public void OnQuickRotate(InputAction.CallbackContext callbackData)
    {
        // Get nearest 90 degree world angle
        int remainder = Mathf.RoundToInt(transform.eulerAngles.y % 90);
        int rotationAmount;

        // If there's any remainder, get closest. Otherwise increment by 90.
        if (remainder != 0)
        {
            rotationAmount = GetClosestAngle(transform.eulerAngles.y);
        }
        else
        {
            rotationAmount = Mathf.RoundToInt(transform.eulerAngles.y) + 90;
        }

        SnapRotation(Quaternion.AngleAxis(rotationAmount, Vector3.up), _quickRotationDuration, SnapRotationType.Quick);

        _audioController.PlayOneShot("play a sound to indicate a quick rotation");
    }

    public int GetClosestAngle(float angle)
    {
        // Normalize the angle to be within [0, 360)
        angle = (Mathf.RoundToInt(angle) % 360 + 360) % 360;

        // Determine the closest multiple of 90
        int closestMultiple = Mathf.RoundToInt(angle / 90.0f) * 90;

        // Calculate the absolute difference between the input angle and the closest multiple
        float difference = Mathf.Abs(angle - closestMultiple);

        // If the difference is greater than 45, choose the next multiple
        if (difference > 45)
        {
            closestMultiple += 90;
        }

        return closestMultiple;
    }

    public void OnRotate(InputAction.CallbackContext callbackData)
    {
        if (IsSnapRotating == true) return;
        
        if (IsRotating == false)
        {
            IsRotating = true;
            _targetRotation = transform.rotation;
        }

        float clampedScrollValue = Mathf.Clamp(callbackData.ReadValue<Vector2>().y, -1, 1);

        _targetRotation.eulerAngles += new Vector3(0, (_targetRotation.y + (clampedScrollValue * _rotationSpeedMultiplyer)), 0);
    }

    private void Update()
    {
        // update value for debug
        _currentRotationDebug = transform.rotation;

        RotateOnWallCollision();
    }

    private void LateUpdate()
    {
        if (IsRotating)
        { 
            RotateOverTime();
        }
    }

    private void RotateOverTime()
    {
        float angleCompleteThreshold = 0.5f;

        if (Quaternion.Angle(transform.rotation, _targetRotation) > angleCompleteThreshold)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime);
            return;
        }

        transform.rotation = _targetRotation;
        IsRotating = false;
    }

    public void ResetToDefaultRotation()
    {
        Debug.Log("Resetting rotation");
        //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        SnapRotation(Quaternion.Euler(0, transform.eulerAngles.y, 0), _snapRotationDuration, SnapRotationType.Reset);
    }

    /// <summary>
    /// Quick rotation on reset or snap to wall. Ignores all other rotation requests.
    /// </summary>
    public void SnapRotation(Quaternion targetRotation, float rotationDuration, SnapRotationType rotationType)
    {
        if (IsSnapRotating == true) return;
        if (transform.rotation == targetRotation) return;

        // Cancel active rotation
        IsRotating = false;
        _targetRotation = transform.rotation;
        _activeSnapRotationType = rotationType;

        _audioController.PlayOneShot("play a sound to indicate a CB controlled fast rotation");
        _snapRotateCoroutine = StartCoroutine(RunSnapRotate(targetRotation, rotationDuration));
    }

    public IEnumerator RunSnapRotate(Quaternion targetRotation, float rotationDuration)
    {
        float timeElapsed = 0;

        while (timeElapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeElapsed / rotationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = targetRotation;
        _activeSnapRotationType = SnapRotationType.None;
    }

    private void RotateOnWallCollision()
    {
        RaycastHit hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.CbObjectMovementMask);

        if (hit.collider == null) return;
        if (hit.collider.tag != "Wall") return;

        // stop any snapping in progress and prioritise the wall snap
        if (_activeSnapRotationType == SnapRotationType.Reset)
        {
            StopCoroutine(_snapRotateCoroutine);
            _activeSnapRotationType = SnapRotationType.None;
        }

        SnapRotation(Quaternion.LookRotation(hit.collider.gameObject.transform.forward), _snapRotationDuration, SnapRotationType.Wall);
    }
}