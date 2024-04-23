using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Collections.Generic;


public class CbObjectParameters : MonoBehaviour
{
    [SerializeField, Required]
    private CbObjectScriptableData _cbObjectData;

    [SerializeField, ReadOnly]
    private CbObjectScriptableData _runTimeCbObjectData;

    public CbObjectScriptableData CbObjectData
    {
        get { return _cbObjectData; }
    }

    [Header("Scriptable Object Data")]
    [SerializeField, ReadOnly]
    private string _nameInUI;

    [SerializeField, ReadOnly]
    public CbObjectScriptableData.PlacedPosition PlacedPosition = CbObjectScriptableData.PlacedPosition.SnapPoint;

    [SerializeField, ReadOnly]
    public float GroundOffset = 0.5f;

    [SerializeField, ReadOnly]
    public float WallOffset = 0.5f;

    [SerializeField, ReadOnly]
    public float SurfaceOffset = 0.05f;

    [SerializeField, ReadOnly]
    public float MarkerRadius;

    [Header("Focus Data")]
    [SerializeField, ReadOnly]
    public CbObjectScriptableData.FocusCameraType FocusCameraType;
    
    [SerializeField, ReadOnly, Tooltip("Set to True if FocusCameraType is set, and the CbObject has a focus camera")]
    private bool _isFocusable = false;

    public bool IsFocusable
    {
        get { return _isFocusable; }
        
        set
        {
            _isFocusable = value;
        }
    }

    [Header("Customisation Data")]
    [SerializeField, ReadOnly, Tooltip("Customisation feature not available.")]
    private bool _isCustomisable = false;

    [SerializeField, ReadOnly]
    public List<Color> RandomColorList;

    [SerializeField, ReadOnly]
    public Color StaticColor;

    private void Awake()
    {
        InitializeObjectData();
    }

    private void InitializeObjectData()
    {
        if (_cbObjectData == null) return;

        _runTimeCbObjectData = ScriptableObject.Instantiate(_cbObjectData);

        FocusCameraType = _runTimeCbObjectData.FocusType;
        PlacedPosition = _runTimeCbObjectData.PlacablePosition;
        GroundOffset = _runTimeCbObjectData.GroundHeightOffset;
        WallOffset = _runTimeCbObjectData.WallHeightOffset;
        SurfaceOffset = _runTimeCbObjectData.SurfaceHeightOffset;
        StaticColor = _runTimeCbObjectData.StaticColor;
        RandomColorList = _runTimeCbObjectData.RandomColorList;
        MarkerRadius = _runTimeCbObjectData.MarkerRadius;

        _nameInUI = _runTimeCbObjectData.name;
        _isCustomisable = _runTimeCbObjectData.IsCustomisable;
    }
}