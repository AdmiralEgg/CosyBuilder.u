using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectData", order = 1)]
public class ObjectData : ScriptableObject
{
    public enum PlacedPosition { SnapPoint, Floor }
    
    [Required, Tooltip("Name of the object as it appears in the UI")]
    public string UIName;

    [Required, Tooltip("Prefab created on object spawn")]
    public GameObject Prefab;

    [Required, Tooltip("Set an icon which will appear in the inventory")]
    public Texture2D InventoryIcon;

    [Tooltip("Can the object be placed in a fixed position on the floor?")]
    public bool IsPlaceable;

    [Tooltip("Can the object hang on a SnapPoint on a wall?")]
    public PlacedPosition PlacablePosition;

    [Tooltip("Can the object be visually changed using the UI?")]
    public bool IsCustomisable;

    public bool HasFocusSet;

    [Tooltip("If fixed, the object can't be selected")]
    public bool AvailableInInventory = true;

    [Tooltip("Unused?")]
    public float MinimumSelectionHeight;

    [Tooltip("Which object must be focused for this to appear in the inventory?")]
    public ScriptableObject[] ParentObjects;
}