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
    private Color _defaultColor;

    private Coroutine _colorChangeOverTime;

    private Rectangle _customizationPoint;

    private const int SATURATION = 70;
    private const int VALUE = 50;
    private const int ALPHA = 255;

    private int _currentHue = 0;

    void Awake()
    {
        // Add a shapes with colour changing that appears when the cursor gets closer
        _customizationPoint = gameObject.AddComponent<Rectangle>();
        _customizationPoint.Type = Rectangle.RectangleType.HardBorder;
        _customizationPoint.Width = 0.3f;
        _customizationPoint.Height = 0.3f;
        _customizationPoint.Thickness = 0.07f;
        _customizationPoint.Dashed = false;

        _colorPicker.ColorSwitch += UpdateMaterials;

        //UpdateMaterials(_defaultColor);
        UpdateMaterials(Color.white);
        RemoveSelectedState();
    }

    private IEnumerator ColorChangeOverTime()
    {
        _currentHue = ((_currentHue + 1) % 300);

        Color newColor = Color.HSVToRGB(_currentHue / 300f, SATURATION / 100f, VALUE / 100f);
        newColor.a = ALPHA / 255f;

        _customizationPoint.Color = newColor;

        yield return new WaitForSeconds(0.05f);
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
        StopCoroutine(_colorChangeOverTime);
        _customizationPoint.Color = Color.white;

        // Get the current material colour
        Color currentMaterialColor = _materialCustomizationController.GetCurrentMaterialColor();

        // enable the colorpicker
        _colorPicker.ShowColorPicker(true);
    }

    private void RemoveSelectedState()
    {
        _isSelected = false;
        _colorPicker.ShowColorPicker(false);

        _colorChangeOverTime = StartCoroutine(ColorChangeOverTime());
    }

    private void UpdateMaterials(Color color)
    {
        Debug.Log("Update materials to: " + color.ToString());
        _materialCustomizationController.UpdateAllMaterialInstances(color);
    }
}
