using Sirenix.Utilities;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class CbObjectLayerController : MonoBehaviour
{
    public enum LayerState { CbObject, CbObjectStatic }

    // TODO Should this be in the CbObjectData?
    [SerializeField]
    private LayerState _initialLayerState = LayerState.CbObject;

    [SerializeField, ReadOnly]
    private LayerState _currentLayerState;

    [SerializeField, ReadOnly]
    private Dictionary<LayerState, LayerMask> _layerMaskLookup = new Dictionary<LayerState, LayerMask>();

    private const string OBJECTLAYERNAME = "CbObject";
    private const string STATICSCENELAYERNAME = "CbObjectStatic";

    [SerializeField, ReadOnly, Tooltip("References to the GameObjects which will be updated with new Layers")]
    private List<GameObject> _layersToSet;

    private void Awake()
    {
        _layerMaskLookup.Add(LayerState.CbObject, LayerMask.NameToLayer(OBJECTLAYERNAME));
        _layerMaskLookup.Add(LayerState.CbObjectStatic, LayerMask.NameToLayer(STATICSCENELAYERNAME));

        FindLayersToSet();
        SetLayers(_initialLayerState); 
    }

    private void FindLayersToSet()
    {
        _layersToSet.Add(this.gameObject);
        
        this.gameObject.GetComponentsInChildren<Collider>().ForEach(collider =>
        {
            // Don't update any trigger or colorchange colliders
            if (collider.isTrigger == false && collider.gameObject.layer != LayerMask.NameToLayer("ColorChanger"))
            {
                _layersToSet.Add(collider.transform.gameObject);
            }
        });
    }

    public void SetLayers(LayerState newLayerState)
    {
        LayerMask newLayerMask = _layerMaskLookup[newLayerState];

        foreach (GameObject objectWithLayer in _layersToSet) 
        {
            objectWithLayer.layer = newLayerMask;
        }

        _currentLayerState = newLayerState;
    }
}