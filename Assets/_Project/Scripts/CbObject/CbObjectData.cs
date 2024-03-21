using UnityEngine;
using Sirenix.OdinInspector;

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
    private bool _isFocusable = false;

    public bool IsFocusable
    {
        get { return _isFocusable; }
        private set { _isFocusable = value; }
    }

    [SerializeField, ReadOnly]
    private float _minSelectionHeight = 0.5f;

    public float MinSelectionHeight
    {
        get { return _minSelectionHeight; }
        private set { _minSelectionHeight = value; }
    }

    private void Start()
    {
        if (_cbObjectData != null)
        {
            NameInUI = _cbObjectData.UIName;
            PlacedPosition = _cbObjectData.PlacablePosition;
            IsCustomisable = _cbObjectData.IsCustomisable;
            MinSelectionHeight = _cbObjectData.MinimumSelectionHeight;
            IsFocusable = _cbObjectData.HasFocusSet;
        }
    }
}