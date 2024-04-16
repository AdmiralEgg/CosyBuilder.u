using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static DictionarySerialization;

/// <summary>
/// Provides Enum access to Layers and Tags, and defines masks for CursorData
/// </summary>
public class LayerAndTagValidator : MonoBehaviour
{
    LayerAndTagValidator Instance;
    
    // Note: IgnoreRaycast is an OOTB layer provided by Unity
    public enum CbLayer { CbObject, CbObjectStatic, CbObjectBounds, CbObjectSpawnPlane, IgnoreRaycast, SnapPoint, StaticScene, UI, PlaceableSurface, InteractionPoint, CustomisationPoint, OutOfBounds }
    public enum CbTag { Roof, Window, Floor, Door, Pillar, Wall }
    public enum MaskInclusionType { Include, Exclude }

    [SerializeField, Header("Creates link between Layers and CbLayers enum value")]
    private SerializableDictionary<CbLayer, string> _layerDictionary = new SerializableDictionary<CbLayer, string>();

    [SerializeField, Header("Creates link between Tags and CbTags enum value")]
    public SerializableDictionary<CbTag, string> _tagDictionary = new SerializableDictionary<CbTag, string>();

    [Header("Validated Lookups")]
    public static SerializableDictionary<CbLayer, int> ValidatedLayerDataLookup = new SerializableDictionary<CbLayer, int>();
    public static SerializableDictionary<CbTag, string> ValidatedTagDataLookup = new SerializableDictionary<CbTag, string>();

    private void Awake()
    {
        Instance = this;
        
        ValidateLayers();
        ValidateTags();
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

            ValidatedLayerDataLookup.Add(layer.Key, layerIdentifier);
        }
    }

    private void ValidateTags()
    {
        // No validation for tags yet, just make them available
        foreach (KeyValuePair<CbTag, string> tag in _tagDictionary)
        {
            ValidatedTagDataLookup.Add(tag.Key, tag.Value);
        }
    }

    public int GetLayerIdentifier(CbLayer layer)
    {
        return ValidatedLayerDataLookup[layer];
    }

    public string GetTagString(CbTag tag)
    {
        return _tagDictionary[tag];
    }

    public Dictionary<CbLayer, int> BuildLayerMaskList(CbLayer[] layers, MaskInclusionType inclusionType)
    {
        Dictionary<CbLayer, int> filteredDictionary;

        // if excluding, then populate the temp dictionary with everything
        if (inclusionType == MaskInclusionType.Exclude)
        {
            filteredDictionary = new Dictionary<CbLayer, int>(ValidatedLayerDataLookup);
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
                filteredDictionary.Add(layer, ValidatedLayerDataLookup[layer]);
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