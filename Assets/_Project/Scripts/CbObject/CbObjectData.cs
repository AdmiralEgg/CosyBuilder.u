using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using Sirenix.Utilities;
using System.Collections.Generic;
using static DictionarySerialization;

public class CbObjectData : MonoBehaviour
{
    [SerializeField, Required]
    private ObjectData _cbObjectData;

    [Header("Scriptable Object Data")]
    [SerializeField, ReadOnly]
    private string _nameInUI = "Default";

    public string NameInUI
    {
        get { return _nameInUI; }
        private set { _nameInUI = value; }
    }

    [SerializeField, ReadOnly]
    private ObjectData.PlacedPosition _placedPosition = ObjectData.PlacedPosition.SnapPoint;

    public ObjectData.PlacedPosition PlacedPosition
    {
        get { return _placedPosition; }
        private set { _placedPosition = value; }
    }

    [SerializeField, ReadOnly]
    private bool _isCustomisable = false;

    public bool IsCustomisable
    {
        get { return _isCustomisable; }
        private set { _isCustomisable = value; }
    }

    [SerializeField, ReadOnly]
    private float _minSelectionHeight = 0.5f;

    public float MinSelectionHeight
    {
        get { return _minSelectionHeight; }
        private set { _minSelectionHeight = value; }
    }

    [Header("Focus Data")]
    [SerializeField, ReadOnly]
    private bool _isFocusable = false;

    public bool IsFocusable
    {
        get { return _isFocusable; }
        set { _isFocusable = value; }
    }

    [SerializeField, ReadOnly]
    private ObjectData.FocusCameraType _focusCameraType;
    public ObjectData.FocusCameraType FocusCameraType
    {
        get { return _focusCameraType; }
        private set { _focusCameraType = value; }
    }

    private void Awake()
    {
        InitializeObjectData();
    }

    private void InitializeObjectData()
    {
        if (_cbObjectData == null) return;

        NameInUI = _cbObjectData.UIName;
        PlacedPosition = _cbObjectData.PlacablePosition;
        IsCustomisable = _cbObjectData.IsCustomisable;
        MinSelectionHeight = _cbObjectData.MinimumSelectionHeight;
        FocusCameraType = _cbObjectData.FocusType;
    }
}