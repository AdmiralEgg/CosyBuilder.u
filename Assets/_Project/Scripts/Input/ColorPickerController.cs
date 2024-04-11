using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;

public class ColorPickerController : MonoBehaviour
{
    [SerializeField, Required]
    public UIDocument UiDocument;

    // Can't access the color palette texture from the visual element, so need to do this.
    [SerializeField, Required]
    public Texture2D ColorPaletteTexture;
    
    [SerializeField]
    private CustomizationManager _customizationManager;

    [SerializeField, ReadOnly]
    private Material _selectedMaterial;

    [SerializeField, ReadOnly]
    private Material[] _selectedMaterialGroup;

    [SerializeField]
    public float ColorSelectorValue;

    private VisualElement _colorPickerElement;

    private bool _colorPickerVisible = false;
    
    [SerializeField]
    private float _pickerHideTime = 3f;

    [SerializeField, Required]
    private AssetReference cbStyleSheet;

    private float _pickerElapsedTime = 0f;
    private bool _pickerHoveredOver = false;

    public static Action<Color> ColorSwitch;

    void Awake()
    {
        // Disable this unless a configurable object selected?
        VisualElement rootElement = UiDocument.rootVisualElement;
        _colorPickerElement = rootElement.Q<VisualElement>("ColorPicker");

        cbStyleSheet.LoadAssetAsync<StyleSheet>().Completed += 
            (StyleSheet) => 
            {
                _colorPickerElement.styleSheets.Add(StyleSheet.Result);

                _colorPickerElement.AddToClassList("colorpicker-visible");
                _colorPickerElement.AddToClassList("colorpicker-hidden");

                Slider colorSelector = _colorPickerElement.Q<Slider>("CBSlider");

                // On update of the color selector, send an event...
                ColorSelectorValue = colorSelector.value;

                colorSelector.RegisterCallback<ChangeEvent<float>>(OnSliderChangeEvent);

                // Subscribe
                //BuildFreeCursorController.ShowColorPicker += OnShowColorPicker;
            };
    }

    private void OnEnable()
    {
        //BuildFreeCursorController.ShowColorPicker += OnShowColorPicker;
    }

    private void OnDisable()
    {
        //BuildFreeCursorController.ShowColorPicker -= OnShowColorPicker;
    }

    // Set the color picker starting value to the current ToColor value of the shader
    private void OnShowColorPicker(GameObject selectedObject)
    {
        _selectedMaterial = null;
        _selectedMaterialGroup = null;

        // Check whether selectedObject can be color swapped
        Color foundColor;
        Material foundMaterial;
        Material[] foundMaterialGroup;

        bool objectHasPickableColor = GetSelectedObjectMaterialData(selectedObject, out foundColor, out foundMaterial, out foundMaterialGroup);

        // If object has no swap value, return
        if (objectHasPickableColor == false)
        {
            Debug.Log($"Cannot find a _ToColor value in this game objects: {selectedObject.name}. On this material: {foundMaterial.name}");
            return;
        }

        // If the picker is already visible, return UNLESS the material we're swapping is different.
        // TODO: Add in the UNLESS
        if (_colorPickerVisible == true) return;

        // Set selected materials
        SetSelectedMaterials(foundMaterial, foundMaterialGroup);

        // Set the current picker value to the found color
        SetPickerValue(foundColor);

        _colorPickerElement.RemoveFromClassList("colorpicker-hidden");
        _colorPickerVisible = true;

        // wait 3 seconds and remove (unless user hovers over it)
        StartCoroutine(HideColorPicker(_colorPickerElement));

        _colorPickerVisible = false;
    }

    private bool GetSelectedObjectMaterialData(GameObject selectedObject, out Color colorValue, out Material selectedMaterial, out Material[] selectedMaterialGroup)
    {
        colorValue = new Color();
        selectedMaterial = null;
        selectedMaterialGroup = new Material[] { };

        // Get the material
        selectedMaterial = selectedObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;

        // If we can't find a material, return false
        if (selectedMaterial == null) return false;

        colorValue = selectedMaterial.GetColor("_ToColor");

        // If we can't find a color from the shader, return false
        if (colorValue == Color.clear) return false;

        // Search for a material group (if a group has been assigned to this class)
        if (_customizationManager != null)
        {
            //List<Material> allMaterials = _materialGroupManager.FindAllWallMaterials(selectedObject);
            List<Material> allMaterials = _customizationManager.FindAllGroupedWalls(selectedObject);

            if (allMaterials == null)
            {
                selectedMaterialGroup = null;
            }
            else
            {
                selectedMaterialGroup = allMaterials.ToArray();
            }
        }

        return true;
    }

    private void SetPickerValue(Color c)
    {
        Debug.Log($"TODO: Set initial picker value to: {c}");
        
        /*
        int pixelSelected = (int)(ColorPaletteTexture.width * (selectorValueChange.newValue / 100));

        Color colorValue = ColorPaletteTexture.GetPixel(pixelSelected, 0);

        MaterialToUpdateTest.SetColor("_ToColor", colorValue);
        */
    }

    private void SetSelectedMaterials(Material selectedMaterial, Material[] selectedMaterialGroup = null)
    {
        _selectedMaterial = selectedMaterial;
        _selectedMaterialGroup = selectedMaterialGroup;
    }

    private void OnSliderChangeEvent(ChangeEvent<float> selectorValueChange)
    {
        int pixelSelected = (int)(ColorPaletteTexture.width * (selectorValueChange.newValue / 100));

        Color colorValue = ColorPaletteTexture.GetPixel(pixelSelected, 0);

        // If there's a material group, update it all, else update the material instance.
        if (_selectedMaterialGroup != null)
        {
            foreach (Material m in _selectedMaterialGroup)
            {
                m.SetColor("_ToColor", colorValue);
            }
        }
        else
        {
            _selectedMaterial.SetColor("_ToColor", colorValue);
        }
    }

    private IEnumerator HideColorPicker(VisualElement colorPicker)
    {
        _pickerElapsedTime = 0;
        
        colorPicker.RegisterCallback<MouseOverEvent>(ResetClock);
        colorPicker.RegisterCallback<MouseOutEvent>(StartClock);

        while (_pickerElapsedTime < _pickerHideTime)
        {
            if (_pickerHoveredOver == true)
            {
                yield return null;
            }
            else
            {
                _pickerElapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        _colorPickerElement.AddToClassList("colorpicker-hidden");

        colorPicker.UnregisterCallback<MouseOverEvent>(ResetClock);
        colorPicker.UnregisterCallback<MouseOutEvent>(StartClock);
    }

    private void ResetClock(MouseOverEvent m)
    {
        // set clock back to zero
        _pickerElapsedTime = 0;

        // set 'hovering'
        _pickerHoveredOver = true;
    }

    private void StartClock(MouseOutEvent m)
    {
        // set hovering to false
        _pickerHoveredOver = false;
    }
}
