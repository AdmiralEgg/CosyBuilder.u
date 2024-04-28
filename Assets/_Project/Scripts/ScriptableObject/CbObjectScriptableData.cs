using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CbObject", order = 1)]
public class CbObjectScriptableData : ScriptableObject
{
    public enum PlacedPosition { None, SnapPoint, Floor }
    public enum FocusCameraType { None, Default, Set, Orbital }
    
    [Header("Inventory Data")]
    [Required, Tooltip("Name of the object as it appears in the UI")]
    public string UIName = "Default";

    [Required, Tooltip("Prefab created on object spawn")]
    public GameObject Prefab;

    [Tooltip("Which object must be focused for this to appear in the inventory?")]
    public CbObjectScriptableData[] ParentObjects;

    [Header("Inventory Data")]
    [Required, Tooltip("Set an icon which will appear in the inventory")]
    public Texture2D InventoryIcon;

    [Tooltip("If fixed, the object can't be selected")]
    public bool AvailableInInventory = true;

    [Tooltip("How many are available")]
    public int NumberAvailable = 3;

    [Tooltip("Is the object available when zoomed out.")]
    public bool AvailableAtRoot = true;

    [Header("Movement Data")]
    [Tooltip("Height off the ground where the center of the gameobject can't be lower than")]
    public float GroundHeightOffset = 0.5f;

    [Tooltip("Height off walls where the center of the gameobject can't be lower than")]
    public float WallHeightOffset = 0.25f;

    [Tooltip("Hover height from surfaces")]
    public float SurfaceHeightOffset = 0.05f;

    [Tooltip("Can the object hang on a SnapPoint on a wall?")]
    public PlacedPosition PlacablePosition = PlacedPosition.None;

    [Tooltip("Radius of the marker for object when selected")]
    public float MarkerRadius = 0.3f;

    [Header("Interaction Data")]
    [Tooltip("If the object has a focus camera, what type of camera is it.")]
    public FocusCameraType FocusType = CbObjectScriptableData.FocusCameraType.None;

    [Header("Customisation Data")]
    [Tooltip("Single static colour to apply to object")]
    public Color StaticColor;

    [Tooltip("List of random colours to apply to object")]
    public List<Color> RandomColorList;

    [Tooltip("Can the object be visually changed using the UI?")]
    public bool IsCustomisable = false;
}