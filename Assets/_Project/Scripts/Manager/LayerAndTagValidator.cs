using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static DictionarySerialization;

/// <summary>
/// Provides Enum access to Layers and Tags, and defines masks for CursorData
/// </summary>
public class LayerAndTagValidator : MonoBehaviour
{
    // Note: IgnoreRaycast is an OOTB layer provided by Unity
    public enum CbLayer { CbObject, CbObjectStatic, CbObjectBounds, CbObjectSpawnPlane, Interactable, IgnoreRaycast, SnapPoint, StaticScene, UI }
    public enum CbTag { Roof, Window, Floor, Door, Pillar, Wall }
    public enum MaskInclusionType { Include, Exclude }

    [SerializeField, Header("Creates link between Layers and CbLayers enum value")]
    private SerializableDictionary<CbLayer, string> _layerDictionary = new SerializableDictionary<CbLayer, string>();

    [SerializeField, Header("Creates link between Tags and CbTags enum value")]
    private SerializableDictionary<CbTag, string> _tagDictionary = new SerializableDictionary<CbTag, string>();

    [Header("Validated Lookups")]
    [SerializeField, ReadOnly]
    private SerializableDictionary<CbLayer, int> _validatedLayerDataLookup = new SerializableDictionary<CbLayer, int>();

    private void Awake()
    {
        ValidateLayers();
    }

    private void ValidateLayers()
    {
        // Iterate through all layers and tags, find the layer and tag values.
        foreach (KeyValuePair<CbLayer, string> layer in _layerDictionary)
        {
            int layerIdentifier = LayerMask.NameToLayer(layer.Value);

            if (layerIdentifier == -1)
            {
                Debug.LogError($"Cannot find layer {layer.Value}, check the layer exists and is correctly named in the manager.");
                continue;
            }

            _validatedLayerDataLookup.Add(layer.Key, layerIdentifier);
        }
    }

    public int GetLayerIdentifier(CbLayer layer)
    {
        return _validatedLayerDataLookup[layer];
    }

    public Dictionary<CbLayer, int> BuildLayerMaskList(CbLayer[] layers, MaskInclusionType inclusionType)
    {
        Dictionary<CbLayer, int> filteredDictionary;

        // if excluding, then populate the temp dictionary with everything
        if (inclusionType == MaskInclusionType.Exclude)
        {
            filteredDictionary = new Dictionary<CbLayer, int>(_validatedLayerDataLookup);
        }
        else
        {
            filteredDictionary = new Dictionary<CbLayer, int>();
        }

        // if it's an exclude, get everything from the list and remove the ones we don't need.
        foreach (CbLayer layer in layers)
        {
            if (inclusionType == MaskInclusionType.Include)
            {
                // add to parsedDictionary
                filteredDictionary.Add(layer, _validatedLayerDataLookup[layer]);
            }

            if (inclusionType == MaskInclusionType.Exclude)
            {
                // remove from parsedDictionary
                filteredDictionary.Remove(layer);
            }
        }

        return filteredDictionary;
    }
}