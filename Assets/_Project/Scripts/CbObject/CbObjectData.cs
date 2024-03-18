using UnityEngine;
using Sirenix.OdinInspector;

public class CbObjectData : MonoBehaviour
{
    [SerializeField, Required]
    private ObjectData _cbObjectData;

    // TODO: Add getters and setters to make these available if required
    [Header("Scriptable Object Data")]
    [SerializeField, ReadOnly]
    private string _nameInUI;
    [SerializeField, ReadOnly]
    private bool _isPlaceable, _isCustomisable, _isFocusable;
    [SerializeField, ReadOnly]
    private ObjectData.PlacedPosition _placedPosition;
    [SerializeField, ReadOnly]
    private float _minSelectionHeight;

    public bool IsFocusable
    {
        get { return _isFocusable; }
        private set { _isFocusable = value; }
    }

    public ObjectData.PlacedPosition PlacedPosition
    { 
        get { return _placedPosition; }
    }

    public float MinSelectionHeight
    {
        get { return _minSelectionHeight; }
    }

    private void Start()
    {
        // TODO: Fix this horror
        if (_cbObjectData != null)
        {
            _nameInUI = _cbObjectData.UIName;
            _isPlaceable = _cbObjectData.IsPlaceable;
            _placedPosition = _cbObjectData.PlacablePosition;
            _isCustomisable = _cbObjectData.IsCustomisable;
            _minSelectionHeight = _cbObjectData.MinimumSelectionHeight;
        }

        // TODO: Check for Focus Cameras. For now, set to True.
        IsFocusable = true;
    }
}