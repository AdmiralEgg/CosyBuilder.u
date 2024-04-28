using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ColorPickerController : MonoBehaviour
{
    [SerializeField, Required]
    public UIDocument UiDocument;

    // Can't access the color palette texture from the visual element, so need to do this.
    [SerializeField, Required]
    public Texture2D ColorPaletteTexture;
    
    private VisualElement _colorPickerElement;
    private Slider _colorSliderElement;
    
    public Action<Color> ColorSwitch;

    void Awake()
    {
        // Disable this unless a configurable object selected?
        VisualElement rootElement = UiDocument.rootVisualElement;
        _colorPickerElement = rootElement.Q<VisualElement>("ColorPicker");

        _colorSliderElement = _colorPickerElement.Q<Slider>("CBSlider");
        _colorSliderElement.RegisterCallback<ChangeEvent<float>>(OnSliderChangeEvent);
    }

    private void Start()
    {
        ShowColorPicker(false);
    }

    private void OnSliderChangeEvent(ChangeEvent<float> selectorValueChange)
    {
        int pixelSelected = (int)(ColorPaletteTexture.width * (selectorValueChange.newValue / 100));

        Color colorValue = ColorPaletteTexture.GetPixel(pixelSelected, 0);

        Debug.Log("Slider change, update color");
        ColorSwitch?.Invoke(colorValue);
    }

    public void ShowColorPicker(bool showPicker)
    {
        if (showPicker == false)
        {
            // add the hide class
            _colorPickerElement.AddToClassList("colorpicker-hidden");
            return;
        }

        // remove the hidden tag
        _colorPickerElement.RemoveFromClassList("colorpicker-hidden");
    }
}