using System.Collections;
using UnityEngine;
using Shapes;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

public class CustomizationPoint : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private MaterialCustomizationController _materialCustomizationController;

    [SerializeField]
    private ColorPickerController _colorPicker;

    [SerializeField]
    bool _isSelected = false;

    [SerializeField]
    private Color _defaultPickerColor;

    [SerializeField]
    private Color _defaultMaterialColor;

    private Rectangle _customizationPoint;

    void Awake()
    {
        // Add a shapes with colour changing that appears when the cursor gets closer
        _customizationPoint = gameObject.AddComponent<Rectangle>();
        _customizationPoint.Type = Rectangle.RectangleType.HardBorder;
        _customizationPoint.Width = 0.3f;
        _customizationPoint.Height = 0.3f;
        _customizationPoint.Thickness = 0.07f;
        _customizationPoint.Dashed = false;
        _customizationPoint.Color = _defaultPickerColor;

        _colorPicker.ColorSwitch += UpdateMaterials;

        UpdateMaterials(_defaultMaterialColor);
        RemoveSelectedState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isSelected == false)
        {
            SetSelectedState();
        }
        else
        {
            RemoveSelectedState();
        }
    }

    private void SetSelectedState()
    {
        _isSelected = true;

        // hightlight the customisation point
        _customizationPoint.Color = Color.white;

        // enable the colorpicker
        _colorPicker.ShowColorPicker(true);
    }

    private void RemoveSelectedState()
    {
        _isSelected = false;

        _customizationPoint.Color = _defaultPickerColor;

        _colorPicker.ShowColorPicker(false);
    }

    private void UpdateMaterials(Color color)
    {
        Debug.Log("Update materials to: " + color.ToString());
        _materialCustomizationController.UpdateAllMaterialInstances(color);
    }
}