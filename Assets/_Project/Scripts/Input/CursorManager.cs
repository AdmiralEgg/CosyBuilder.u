using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Raycasts for Interactions and Customisation Points
/// Updates Cursor with prompts
/// </summary>
public class InteractionHighlighter : MonoBehaviour
{
    private enum CursorType { Main, Customize, Interact }
    
    [SerializeField, AssetsOnly]
    private Texture2D _mainCursor, _customizeCursor, _interactCursor;

    private CursorType _currentCursor;

    private void Start()
    {
        _currentCursor = CursorType.Main;
    }

    private void Update()
    {
        var hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.OnInteraction);

        if (hit.collider != null)
        {
            int interactionLayer = LayerAndTagValidator.ValidatedLayerDataLookup[LayerAndTagValidator.CbLayer.InteractionPoint];
            int customisationLayer = LayerAndTagValidator.ValidatedLayerDataLookup[LayerAndTagValidator.CbLayer.CustomisationPoint];

            if (hit.collider.gameObject.layer == interactionLayer)
            {
                _currentCursor = CursorType.Interact;
                Cursor.SetCursor(_interactCursor, Vector2.zero, CursorMode.Auto);
                //Debug.Log("Cursor interaction hit: " + hit.collider.gameObject.name);
            }

            if (hit.collider.gameObject.layer == customisationLayer)
            {
                _currentCursor = CursorType.Customize;
                Cursor.SetCursor(_customizeCursor, Vector2.zero, CursorMode.Auto);
                //Debug.Log("Cursor customisation hit: " + hit.collider.gameObject.name);
            }

            // Object will increase in emission the closer the cursor gets
        }
        else
        {
            _currentCursor = CursorType.Main;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
