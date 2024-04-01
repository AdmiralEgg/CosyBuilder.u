using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using static DictionarySerialization;
using static UnityEditor.PlayerSettings;

public class CursorData : MonoBehaviour
{
    public enum LayerMaskType { CbObjectMovementMask, CbObjectOnlyMask, WithinSnapPoint }

    public static CursorData Instance;

    [SerializeField]
    private bool _runRayCastDebug = true;

    private Ray _rayFromMouseCursor;

    private LayerAndTagValidator _layerAndTagValidator;

    [SerializeField, ReadOnly]
    private List<LayerMaskTypeData> _layerMaskTypeLookup = new List<LayerMaskTypeData>();

    [SerializeField, ReadOnly]
    private SerializableDictionary<LayerMaskType, string> _raycastColliderHitDebug = new SerializableDictionary<LayerMaskType, string>();

    private void Awake()
    {
        Instance = this;
        _layerAndTagValidator = GetComponent<LayerAndTagValidator>();
        SetupLayerTypes();
    }

    private void SetupLayerTypes()
    {
        // Breaking pattern to keep indentiation to a minimum
        Instance._layerMaskTypeLookup.Add(new LayerMaskTypeData
        (
            LayerMaskType.CbObjectMovementMask,
            "For use when CbObject is in selected state to follow the cursor",
            new LayerAndTagValidator.CbLayer[]
            {
            LayerAndTagValidator.CbLayer.CbObject,
            LayerAndTagValidator.CbLayer.IgnoreRaycast,
            LayerAndTagValidator.CbLayer.SnapPoint,
            LayerAndTagValidator.CbLayer.CbObjectBounds
            },
            LayerAndTagValidator.MaskInclusionType.Exclude
        ));

        Instance._layerMaskTypeLookup.Add(new LayerMaskTypeData
        (
            LayerMaskType.CbObjectOnlyMask,
            "Debug to show CbObject raycast hits",
            new LayerAndTagValidator.CbLayer[]
            {
                LayerAndTagValidator.CbLayer.CbObject,
            },
            LayerAndTagValidator.MaskInclusionType.Include
        ));

        Instance._layerMaskTypeLookup.Add(new LayerMaskTypeData
        (
            LayerMaskType.WithinSnapPoint,
            "Check for a SnapPoint radius",
            new LayerAndTagValidator.CbLayer[]
            {
                LayerAndTagValidator.CbLayer.SnapPoint,
            },
            LayerAndTagValidator.MaskInclusionType.Include
        ));
    }

    private void FixedUpdate()
    {
        _rayFromMouseCursor = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (_runRayCastDebug)
        {
            RayCastDebug();
        }
    }

    private void RayCastDebug()
    {
        _raycastColliderHitDebug.Clear();

        foreach (LayerMaskType layer in Enum.GetValues(typeof(LayerMaskType)))
        {
            RaycastHit hit = GetRaycastHit(layer);
            string foundObject = "None";

            if (hit.collider != null)
            {
                foundObject = hit.collider.gameObject.name;
            }

            _raycastColliderHitDebug.Add(layer, foundObject);
        }
    }

    public static RaycastHit GetRaycastHit(LayerMaskType type)
    {
        int layerMask = 0;
        RaycastHit raycastHit = new RaycastHit();
        
        LayerMaskTypeData data = Instance._layerMaskTypeLookup.FirstOrDefault<LayerMaskTypeData>(x => x.Type == type);

        // if there was not match, exit early
        if (data.Equals(default(LayerMaskTypeData)) == true)
        {
            Debug.LogWarning($"LayerMaskTypeData could not be found. Type {type}");
            return raycastHit;
        }

        // Get a list filtering using the InclusionType
        Dictionary<LayerAndTagValidator.CbLayer, int> filteredLayers = Instance._layerAndTagValidator.BuildLayerMaskList(data.LayerList, data.InclusionType);

        foreach (KeyValuePair<LayerAndTagValidator.CbLayer, int> layer in filteredLayers)
        {
            layerMask |= (1 << layer.Value);
        }

        raycastHit = RunRaycast(layerMask);

        return raycastHit;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(_rayFromMouseCursor.origin, _rayFromMouseCursor.direction * 200, Color.yellow);
    }

    private static RaycastHit RunRaycast(int layerMask = -1)
    {
        RaycastHit hit;
        
        if (layerMask == -1) 
        {
            Physics.Raycast(Instance._rayFromMouseCursor, out hit, float.PositiveInfinity);
        }
        else
        {
            Physics.Raycast(Instance._rayFromMouseCursor, out hit, float.PositiveInfinity, layerMask);
        }

        return hit;
    }

    [Serializable]
    private struct LayerMaskTypeData
    {
        public LayerMaskType Type;
        public string Description;
        public LayerAndTagValidator.CbLayer[] LayerList;
        public LayerAndTagValidator.MaskInclusionType InclusionType;

        public LayerMaskTypeData(LayerMaskType type, string description, LayerAndTagValidator.CbLayer[] layerList, LayerAndTagValidator.MaskInclusionType inclusionType)
        {
            Type = type;
            LayerList = layerList;
            Description = description;
            InclusionType = inclusionType;
        }
    }
}