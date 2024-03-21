using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectData", order = 1)]
public class ObjectData : ScriptableObject
{
    public enum PlacedPosition { None, SnapPoint, Floor }
    public enum FocusCameraType { None, Default, Set, Orbital }
    
    [Required, Tooltip("Name of the object as it appears in the UI")]
    public string UIName = "Default";

    [Required, Tooltip("Prefab created on object spawn")]
    public GameObject Prefab;

    [Required, Tooltip("Set an icon which will appear in the inventory")]
    public Texture2D InventoryIcon;

    [Tooltip("Can the object hang on a SnapPoint on a wall?")]
    public PlacedPosition PlacablePosition = PlacedPosition.None;

    [Tooltip("Can the object be visually changed using the UI?")]
    public bool IsCustomisable = false;

    [Tooltip("If the object has a focus camera, what type of camera is it.")]
    public FocusCameraType FocusType = ObjectData.FocusCameraType.None;

    [Tooltip("If fixed, the object can't be selected")]
    public bool AvailableInInventory = true;

    [Tooltip("Height off the ground where the center of the gameobject can't be lower than")]
    public float MinimumSelectionHeight = 0.5f;

    [Tooltip("Which object must be focused for this to appear in the inventory?")]
    public ScriptableObject[] ParentObjects;
}