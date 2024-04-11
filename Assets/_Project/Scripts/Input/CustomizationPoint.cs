using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.EventSystems;

public class CustomizationPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private CustomizationManager _customizationManager;
    
    private Sphere _customizationPoint;

    const int SATURATION = 70;
    const int VALUE = 50;
    const int ALPHA = 150;

    int _currentHue = 0;

    [SerializeField]
    bool _IsInFocus = true;

    void Awake()
    {
        // Add a shapes with colour changing that appears when the cursor gets closer
        _customizationPoint = gameObject.AddComponent<Sphere>();
        _customizationPoint.Radius = 0.1f;

        StartCoroutine(ColorChangeOverTime());
    }

    private IEnumerator ColorChangeOverTime()
    {
        while (_IsInFocus)
        { 
            _currentHue = ((_currentHue + 1) % 300);

            Color newColor = Color.HSVToRGB(_currentHue / 300f, SATURATION / 100f, VALUE / 100f);
            newColor.a = ALPHA / 255f;

            _customizationPoint.Color = newColor;

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // send a message to the clicked gameobject
        Debug.Log("Bring up customisation window for " + _customizationManager.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
}
