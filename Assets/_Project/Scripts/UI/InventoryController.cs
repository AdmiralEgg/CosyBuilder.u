using UnityEngine;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UIElements;
using static DictionarySerialization;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class InventoryController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private SerializableDictionary<CbObjectScriptableData, List<CbObjectScriptableData>> _inventorySets = new SerializableDictionary<CbObjectScriptableData, List<CbObjectScriptableData>>();

    [SerializeField, ReadOnly]
    private SerializableDictionary<CbObjectScriptableData, ItemPool> _poolSets = new SerializableDictionary<CbObjectScriptableData, ItemPool>();

    [SerializeField, Required]
    private AssetLabelReference _cbObjectDataLabel;

    [Tooltip("Objects available while in Build mode")]
    private CbObjectScriptableData _rootSet;

    private string _inventoryElementName = "Inventory";

    private VisualElement _inventoryElement;

    private void Awake()
    {
        _rootSet = ScriptableObject.CreateInstance<CbObjectScriptableData>();
        
        _inventoryElement = GetInventoryUIElement(_inventoryElementName);

        if ( _inventoryElement != null )
        {
            BuildInventorySets();
        }
        else
        {
            Debug.LogWarning($"Could not find VisualElement {_inventoryElementName} in UI Document attached to this GameObject");
        }
    }

    private void Start()
    {
        CbObjectPlacedFocusedSubState.cbObjectFocusedScriptableData += RefreshInventoryList;
        
        GameModeStateMachine.OnStateChange = (state) =>
        {
            if (state == GameModeStateMachine.GameModeState.Build)
            {
                RefreshInventoryList();
            }
        };
    }

    private VisualElement GetInventoryUIElement(string rootVisualElementName)
    {
        // Get the inventory element
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
        return rootElement.Q<VisualElement>(rootVisualElementName);
    }

    private void BuildInventorySets()
    {
        // Create a rootSet
        _rootSet.name = "RootSet";

        _inventorySets.Add(_rootSet, new List<CbObjectScriptableData>());

        // Get all scriptable objects
        Addressables.LoadAssetsAsync<CbObjectScriptableData>(_cbObjectDataLabel, null).Completed += (cbObjectData) =>
        {
            // Put them into sets of objects with a PARENT and CHILDREN
            foreach (CbObjectScriptableData cbObject in cbObjectData.Result)
            {
                if (cbObject.AvailableAtRoot)
                {
                    _inventorySets[_rootSet].Add(cbObject);
                }

                foreach (CbObjectScriptableData cbObjectParent in cbObject.ParentObjects)
                {
                    if (_inventorySets.ContainsKey(cbObjectParent))
                    {
                        _inventorySets[_rootSet].Add(cbObject);
                    }
                    else
                    {
                        var newList = new List<CbObjectScriptableData>() { cbObject };
                        _inventorySets.Add(cbObjectParent, newList);
                    }
                }

                ItemPool itemPool = ConfigureObjectPool(cbObject);
                _poolSets.Add(cbObject, itemPool);
            }

            RefreshInventoryList();
        };
    }

    private ItemPool ConfigureObjectPool(CbObjectScriptableData cbObjectData)
    {
        GameObject poolObject = new GameObject();
        poolObject.transform.parent = this.transform;
        poolObject.name = $"{cbObjectData.UIName}_CbObjectPool";

        var itemPool = poolObject.AddComponent<ItemPool>();
        itemPool.SetupItemPool(cbObjectData.Prefab, prewarmPool: true);

        return itemPool;
    }

    private void RefreshInventoryList(CbObjectScriptableData cbObject = null)
    {
        _inventoryElement.Clear();
        
        List<CbObjectScriptableData> inventoryList = GetInventoryList(cbObject);
        
        foreach (CbObjectScriptableData inventoryObject in inventoryList)
        {
            VisualElement itemTile = InventoryTileBuilder.GetConfiguredItemTile(inventoryObject.InventoryIcon);
            VisualElement amountLabel = InventoryTileBuilder.GetConfiguredAmountLabel();

            // Add click callbacks for both elements
            itemTile.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == 1)
                {
                    Debug.Log("RIGHT Mouse down on inventory!!");
                    GameObject @object;
                    //pool.Pool.Get(out @object);
                    //@object.transform.position = new Vector3(0, 4, 0);
                }

                if (e.button == 0)
                {
                    Debug.Log("LEFT Mouse down on inventory!!");
                }
            });

            itemTile.Add(amountLabel);

            _inventoryElement.Add(itemTile);
        }
    }

    private List<CbObjectScriptableData> GetInventoryList(CbObjectScriptableData cbObject)
    {
        // Return the root inventory
        if (cbObject == null)
        {
            return _inventorySets[_rootSet];
        }

        if (_inventorySets.ContainsKey(cbObject) == false)
        {
            Debug.LogWarning($"Requested Inventory Set: {cbObject.name} cannot be found. Inventory will not refresh");
            return null;
        }

        return _inventorySets[cbObject];
    }
}