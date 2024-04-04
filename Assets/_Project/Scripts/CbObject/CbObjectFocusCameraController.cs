using Cinemachine;
using Sirenix.Utilities;
using UnityEngine;
using Sirenix.OdinInspector;
using static DictionarySerialization;
using System.Collections.Generic;
using System;
using System.Linq;

public class CbObjectFocusCameraController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private List<CinemachineVirtualCamera> _focusCameraList;

    [SerializeField, ReadOnly]
    private SerializableDictionary<GameObject, GameObject> _focusCameraDictionaryWithParentGameObjects = new SerializableDictionary<GameObject, GameObject>();

    private CbObjectParameters _objectData;

    private void Awake()
    {
        _objectData = GetComponent<CbObjectParameters>();
    }

    private void Start()
    {
        FindFocusCameras();

        if (_focusCameraList.Count != 0)
        {
            _objectData.IsFocusable = true;
        }

        foreach (var camera in _focusCameraList)
        {
            ConfigureFocusCameras(camera);
        }
    }

    private void FindFocusCameras()
    {
        if (_objectData.FocusCameraType == CbObjectScriptableData.FocusCameraType.None) return;
        
        GetComponentsInChildren<CinemachineVirtualCamera>().ForEach(camera =>
        {
            _focusCameraList.Add(camera);
        });
    }

    private void ConfigureFocusCameras(CinemachineVirtualCamera cam)
    {
        cam.Priority = 12;
        cam.gameObject.SetActive(false);

        if (_objectData.FocusCameraType == CbObjectScriptableData.FocusCameraType.Set)
        {
            _focusCameraDictionaryWithParentGameObjects.Add(cam.transform.parent.gameObject, cam.gameObject);
        };
    }

    public void EnableFocusCamera(GameObject objectFocused = null)
    {
        CinemachineVirtualCamera enabledCamera = null;

        switch (_objectData.FocusCameraType)
        {
            case CbObjectScriptableData.FocusCameraType.Default:
                enabledCamera = GetBestDefaultFocusCamera();
                break;
            case CbObjectScriptableData.FocusCameraType.Set:
                enabledCamera = GetBestSetFocusCamera(objectFocused);
                break;
            case CbObjectScriptableData.FocusCameraType.Orbital:
                enabledCamera = GetConfiguredOrbitalCamera();
                break;
        }

        if (enabledCamera != null) Debug.Log($"Returned camera: {enabledCamera.name} ");

        enabledCamera.gameObject.SetActive(true);
    }

    public void DisableFocusCamera()
    {
        foreach (CinemachineVirtualCamera cameraGameObject in _focusCameraList)
        {
            cameraGameObject.gameObject.SetActive(false);
        }
    }

    public CinemachineVirtualCamera GetBestDefaultFocusCamera()
    {
        float maxRaycastDistance = 20f;

        Dictionary<CinemachineVirtualCamera, float> cameraDistances = new Dictionary<CinemachineVirtualCamera, float>();

        foreach (CinemachineVirtualCamera camera in _focusCameraList)
        {
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