using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Raycasts for Interactions and Customisation Points
/// Updates Cursor with prompts
/// </summary>
public class InteractionHighlighter : MonoBehaviour
{
    private void Update()
    {
        var hit = CursorData.GetRaycastHit(CursorData.LayerMaskType.OnInteraction);

        if (hit.collider != null)
        {
            int interactionLayer = LayerAndTagValidator.ValidatedLayerDataLookup[LayerAndTagValidator.CbLayer.InteractionPoint];
            int customisationLayer = LayerAndTagValidator.ValidatedLayerDataLookup[LayerAndTagValidator.CbLayer.CustomisationPoint];

            if (hit.collider.gameObject.layer == interactionLayer)
            {
                //Debug.Log("Cursor interaction hit: " + hit.collider.gameObject.name);
            }

            if (hit.collider.gameObject.layer == customisationLayer)
            {
                //Debug.Log("Cursor customisation hit: " + hit.collider.gameObject.name);
            }

            // Object will increase in emission the closer the cursor gets
        }
    }
}
