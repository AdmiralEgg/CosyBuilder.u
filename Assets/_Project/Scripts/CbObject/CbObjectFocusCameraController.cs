using Cinemachine;
using Sirenix.Utilities;
using UnityEngine;
using Sirenix.OdinInspector;
using static DictionarySerialization;
using System.Collections.Generic;
using System;
using static ObjectData;
using System.Linq;

public class CbObjectFocusCameraController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private List<GameObject> _focusCameraList;

    [SerializeField, ReadOnly]
    private SerializableDictionary<GameObject, GameObject> _focusCameraDictionaryWithParentGameObjects = new SerializableDictionary<GameObject, GameObject>();

    private CbObjectData _objectData;

    private void Awake()
    {
        _objectData = GetComponent<CbObjectData>();
    }

    private void Start()
    {
        ConfigureFocusCameras();
        AddSetTypeCameraReferences();
    }

    private void AddSetTypeCameraReferences()
    {
        if (_focusCameraList.Count == 0) return;
        if (_objectData.FocusCameraType != ObjectData.FocusCameraType.Set) return;

        foreach (var camera in _focusCameraList)
        {
            _focusCameraDictionaryWithParentGameObjects.Add(camera.transform.parent.gameObject, camera.gameObject);
        }
    }

    private void ConfigureFocusCameras()
    {
        GetComponentsInChildren<CinemachineVirtualCamera>().ForEach(camera =>
        {
            camera.Priority = 12;
            camera.gameObject.SetActive(false);

            _focusCameraList.Add(camera.gameObject);
        });

        if (_focusCameraList.Count == 0) return;
        if (_objectData.FocusCameraType == ObjectData.FocusCameraType.None)
        {
            Debug.LogWarning("Found Focus VCams, but FocusCameraType set to 'None'. Set a FocusCameraType to use Focus on this CbObject");
            return;
        }

        _objectData.IsFocusable = true;
    }

    public void EnableFocusCamera(GameObject objectFocused = null)
    {
        CinemachineVirtualCamera enabledCamera = null;

        switch (_objectData.FocusCameraType)
        {
            case ObjectData.FocusCameraType.Default:
                enabledCamera = GetBestDefaultFocusCamera();
                break;
            case ObjectData.FocusCameraType.Set:
                enabledCamera = GetBestSetFocusCamera(objectFocused);
                break;
            case ObjectData.FocusCameraType.Orbital:
                enabledCamera = GetConfiguredOrbitalCamera();
                break;
        }

        if (enabledCamera != null) Debug.Log($"Returned camera: {enabledCamera.name} ");

        enabledCamera.gameObject.SetActive(true);
    }

    public void DisableFocusCamera()
    {
        foreach (GameObject cameraGameObject in _focusCameraList)
        {
            cameraGameObject.SetActive(false);
        }
    }

    public CinemachineVirtualCamera GetBestDefaultFocusCamera()
    {
        float maxRaycastDistance = 20f;

        Dictionary<CinemachineVirtualCamera, float> cameraDistances = new Dictionary<CinemachineVirtualCamera, float>();

        foreach (GameObject cameraGameObject in _focusCameraList)
        {
            var camera = cameraGameObject.GetComponent<CinemachineVirtualCamera>();

            // raycast from the focus target to the camera
            Physics.Raycast(camera.LookAt.position, camera.transform.position, out RaycastHit hitInfo, maxRaycastDistance, ~LayerMask.GetMask("MovementPlane", "SpawnPlane"));

            if (hitInfo.collider == null)
            {
                cameraDistances.Add(camera, maxRaycastDistance);
            }
            else
            {
                cameraDistances.Add(camera, hitInfo.distance);
            }
        }

        // Get the camera with the lowest distance
        CinemachineVirtualCamera bestCamera = cameraDistances.OrderBy(kv => kv.Value).Last().Key;

        return bestCamera;
    }

    private CinemachineVirtualCamera GetBestSetFocusCamera(GameObject objectFocused)
    {
        Debug.Log($"Inside get best set focus: {objectFocused.name}");

        // get the key from the value
        CinemachineVirtualCamera foundCamera = _focusCameraDictionaryWithParentGameObjects[objectFocused].GetComponent<CinemachineVirtualCamera>();

        return foundCamera;
    }

    /// <summary>
    /// Configure the orbital camera, then return it.
    /// </summary>
    private CinemachineVirtualCamera GetConfiguredOrbitalCamera()
    {
        throw new NotImplementedException();
    }
}