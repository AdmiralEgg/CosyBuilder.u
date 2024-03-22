using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Shapes;
using MoreMountains.Tools;

public class RotateCamera : MonoBehaviour
{
    private enum RotationType { Slerp, Lerp }

    public enum FlickRotationState { Idle, FlickHeld, StartRunning, Running }

    [SerializeField, ReadOnly]
    bool _isRotating = false;

    [SerializeField, ReadOnly]
    FlickRotationState _flickRotationState;

    [Header("Direct Rotation")]
    [SerializeField, Range(0.01f, 0.5f)]
    float _slerpRatio;

    [SerializeField, Range(0.01f, 0.5f), Tooltip("Applying a set rotation speed")]
    float _flickRotateSlerpRatio;

    [SerializeField, ReadOnly]
    private float _flickVerticalDistance;

    [SerializeField, ReadOnly]
    private float _flickVerticalDistanceAsScreenPercentage;

    [SerializeField, ReadOnly, Tooltip("Control the speed of the spin")]
    float _flickSpeed;

    [SerializeField, Tooltip("Distance As Screen Percentage value threshold for a 90 degree rotation turning into a 180 degree rotation."), Range(0.1f, 0.9f)]
    float _fullSpinDistanceThreshold = 0.6f;

    private float _flickStartTime, _flickEndTime;
    private float _flickDuration;
    private float _flickRotateStartScreenPointY, _flickRotateEndScreenPointY;

    private void Start()
    {
        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(0);

        playerInput.actions["RotateImmediate"].performed += SnapRotation;

        _flickRotationState = FlickRotationState.Idle;

        playerInput.actions["FlickRotate"].started += FlickRotate;
        playerInput.actions["FlickRotate"].performed += FlickRotate;
        playerInput.actions["FlickRotate"].canceled += FlickRotate;
    }

    private void FlickRotate(InputAction.CallbackContext context)
    {
        if (_isRotating == true) return;

        if (context.started)
        {
            _flickRotateStartScreenPointY = Input.mousePosition.y;
            _flickStartTime = Time.time;
        }

        if (context.performed)
        {
            _flickRotationState = FlickRotationState.FlickHeld;
        }

        if (context.canceled)
        {
            // If we haven't performed the hold, don't do anything.
            //if (_flickRotateStartIndicator.activeSelf == false) return;
            if (_flickRotationState != FlickRotationState.FlickHeld) return;

            _flickRotateEndScreenPointY = Input.mousePosition.y;
            _flickEndTime = Time.time;

            // Check it
            _flickVerticalDistance = _flickRotateStartScreenPointY - _flickRotateEndScreenPointY;
            _flickVerticalDistanceAsScreenPercentage = _flickVerticalDistance / Screen.height;
            _flickSpeed = _flickVerticalDistanceAsScreenPercentage / (_flickEndTime - _flickStartTime);

            // if the flick is too small, ignore it and let the snap run
            if (Mathf.Abs(_flickVerticalDistanceAsScreenPercentage) < 0.1)
            {
                Debug.Log($"Flick too small. Size {_flickVerticalDistanceAsScreenPercentage}");
                _flickRotationState = FlickRotationState.Idle;
            }
            else
            {
                _flickRotationState = FlickRotationState.StartRunning;
                _isRotating = true;
            }
        }
    }

    private void SnapRotation(InputAction.CallbackContext context)
    {
        if (_isRotating) return;

        float rotationAmount = 90f * context.ReadValue<float>();
        Debug.Log("Rotation amount:" + rotationAmount);

        _isRotating = true;

        StartCoroutine(RotateSlerp(rotationAmount, _slerpRatio));
    }

    private IEnumerator MultiRotateSlerp(float rotationInDegrees, int amountOfRotations, float ratio = 0.02f)
    {
        Debug.Log("Amount of rotations" + amountOfRotations);

        for (int i = 0; i < amountOfRotations; i++)
        {
            Quaternion _flickTargetRotation = transform.rotation * Quaternion.Euler(0f, rotationInDegrees, 0f);

            while (Quaternion.Angle(transform.rotation, _flickTargetRotation) > 1f)
            {
                this.transform.rotation = Quaternion.Slerp(transform.rotation, _flickTargetRotation, ratio);
                yield return null;
            }

            transform.rotation = _flickTargetRotation;
        }

        _flickRotationState = FlickRotationState.Idle;
        _isRotating = false;
    }

    private IEnumerator RotateSlerp(float rotationInDegrees, float ratio = 0.02f)
    {
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0f, rotationInDegrees, 0f);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ratio);
            yield return null;
        }

        transform.rotation = targetRotation;
        _isRotating = false;
    }

    private Vector3 GetMousePointOnMovementPlane(float planeHeight = 0f)
    {
        Plane movementPlane = new Plane(Vector3.up, Vector3.zero);
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

        float enter;

        if (movementPlane.Raycast(r, out enter))
        {
            return new Vector3(r.GetPoint(enter).x, planeHeight, r.GetPoint(enter).z);
        }

        return Vector3.zero;
    }

    private void Update()
    {
        if (_flickRotationState == FlickRotationState.StartRunning)
        {
            float rotationAmount = 90f;
            int numberOfRotations = 1;

            // full spin threshold
            if (Mathf.Abs(_flickVerticalDistanceAsScreenPercentage) > _fullSpinDistanceThreshold)
            {
                numberOfRotations = 2;
            }

            // Clockwise: 1, 2
            if (_flickSpeed > 0)
            {
                StartCoroutine(MultiRotateSlerp(rotationAmount, numberOfRotations, _flickRotateSlerpRatio));
                Debug.Log("Rotate clockwise");
            }
            else if (_flickSpeed < 0)
            {
                StartCoroutine(MultiRotateSlerp(-rotationAmount, numberOfRotations, _flickRotateSlerpRatio));
                Debug.Log("Rotate anticlockwise");
            }
            else
            {
                Debug.Log($"No rotation, flick speed: {_flickSpeed}");
                //_isRotating = false;
            }

            _flickRotationState = FlickRotationState.Running;
        }
    }

    public FlickRotationState GetFlickRotationState()
    {
        return _flickRotationState;
    }
}