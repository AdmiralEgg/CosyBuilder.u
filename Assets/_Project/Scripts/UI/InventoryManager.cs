using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UIElements;
using static DictionarySerialization;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    [SerializeField, Required]
    private AssetLabelReference _cbObjectDataLabel;

    [Header("Inventory Lookups")]
    [SerializeField, ReadOnly]
    private SerializableDictionary<CbObjectScriptableData, List<CbObjectScriptableData>> _inventorySetLookup = new SerializableDictionary<CbObjectScriptableData, List<CbObjectScriptableData>>();

    [SerializeField, ReadOnly]
    private SerializableDictionary<CbObjectScriptableData, VisualElement> _inventoryTileLookup = new SerializableDictionary<CbObjectScriptableData, VisualElement>();

    [SerializeField, ReadOnly]
    private SerializableDictionary<CbObjectScriptableData, ItemPool> _inventoryPoolLookup = new SerializableDictionary<CbObjectScriptableData, ItemPool>();

    [Tooltip("Objects available while in Build mode")]
    private CbObjectScriptableData _rootSet;
    
    [SerializeField, Required]
    private UIDocument _inventoryUIDocument;

    private VisualElement _inventoryContainerVisualElement;
    
    private InventoryObjectPoolController _inventoryPoolController;

    private const string INVENTORY_ELEMENT_NAME = "Inventory";
    private const string ROOT_SET_NAME = "RootSet";

    private void Awake()
    {
        // Create a rootSet
        _rootSet = ScriptableObject.CreateInstance<CbObjectScriptableData>();
        _rootSet.name = ROOT_SET_NAME;
        _inventorySetLookup.Add(_rootSet, new List<CbObjectScriptableData>());

        // Get a reference to the pool controller
        _inventoryPoolController = GetComponent<InventoryObjectPoolController>();

        VisualElement rootElement = _inventoryUIDocument.rootVisualElement;
        _inventoryContainerVisualElement = rootElement.Q<VisualElement>(INVENTORY_ELEMENT_NAME);

        if (_inventoryContainerVisualElement != null )
        {
            BuildInventorySets();
        }
        else
        {
            Debug.LogWarning($"Could not find VisualElement {INVENTORY_ELEMENT_NAME} in UI Document attached to this GameObject");
        }
    }

    private void Start()
    {
        CbObjectPlacedFocusedSubState.CbObjectFocusedScriptableData += RefreshInventoryList;
        
        GameModeStateMachine.OnStateChange = (state) =>
        {
            if (state == GameModeStateMachine.GameModeState.Build)
            {
                RefreshInventoryList();
            }
        };
    }

    private void BuildInventorySets()
    {
        // Get all scriptable objects
        Addressables.LoadAssetsAsync<CbObjectScriptableData>(_cbObjectDataLabel, null).Completed += (cbObjectData) =>
        {
            // Put them into sets of objects with a PARENT and CHILDREN
            foreach (CbObjectScriptableData cbObject in cbObjectData.Result)
            {
                if (CanObjectSpawn(cbObject) == false) continue;
                
                InstantiateInventoryItem(cbObject);

                // Initialize InventoryPool
                InstantiateInventoryPool(cbObject);

                // Set callbacks
                SetInventoryTileCallbacks(_inventoryTileLookup[cbObject], _inventoryPoolLookup[cbObject]);
            }

            RefreshInventoryList();
        };
    }

    private bool CanObjectSpawn(CbObjectScriptableData cbObject)
    {
        // if no root and no parents
        if (cbObject.AvailableAtRoot == false && cbObject.ParentObjects.Length == 0)
        {
            Debug.LogWarning($"Object {cbObject.name} is not available to spawn. Please check CbObjectData.");
            return false;
        }

        if (cbObject.Prefab == null)
        {
            Debug.LogWarning($"Object {cbObject.name} does not have an attached prefab. Please check CbObjectData");
            return false;
        }

        return true;
    }

    private void InstantiateInventoryPool(CbObjectScriptableData cbObject)
    {
        ItemPool newPool = _inventoryPoolController.InitializeInventoryTile(cbObject);
        
        _inventoryPoolLookup.Add(cbObject, newPool);
    }

    /// <summary>
    /// Adds a CbObject to an inventorySet, then initializes an inventory tile and stores it in a dictionary.
    /// </summary>
    /// <param name="cbObject"></param>
    private void InstantiateInventoryItem(CbObjectScriptableData cbObject)
    {
        if (cbObject.AvailableAtRoot == true)
        {
            //TODO: Don't add it if already exists
            _inventorySetLookup[_rootSet].Add(cbObject);
        }

        foreach (CbObjectScriptableData cbObjectParent in cbObject.ParentObjects)
        {
            if (_inventorySetLookup.ContainsKey(cbObjectParent))
            {
                _inventorySetLookup[cbObjectParent].Add(cbObject);
            }
            else
            {
                var newList = new List<CbObjectScriptableData>() { cbObject };
                _inventorySetLookup.Add(cbObjectParent, newList);
            }
        }

        VisualElement itemTile = InventoryTileBuilder.GetConfiguredItemTile(cbObject.InventoryIcon);
        VisualElement amountLabel = InventoryTileBuilder.GetConfiguredAmountLabel();

        itemTile.Add(amountLabel);

        _inventoryTileLookup.Add(cbObject, itemTile);
    }

    /// <summary>
    /// Clears the inventory elements, then picks visual elements required in the refresh from the InventoryTile pool and adds them to the inventory.
    /// </summary>
    /// <param name="cbObject"></param>
    private void RefreshInventoryList(CbObjectScriptableData cbObject = null)
    {
        _inventoryContainerVisualElement.Clear();
        
        List<CbObjectScriptableData> inventoryList = GetInventoryList(cbObject);

        if (inventoryList == null) return;

        foreach (CbObjectScriptableData inventoryObject in inventoryList)
        {
            VisualElement inventoryTile = _inventoryTileLookup[inventoryObject];

            if (inventoryTile == null) continue;

            // fetch element from the InventoryTile dictionary
            _inventoryContainerVisualElement.Add(inventoryTile);
        }
    }

    private List<CbObjectScriptableData> GetInventoryList(CbObjectScriptableData cbObject)
    {
        // Return the root inventory
        if (cbObject == null)
        {
            return _inventorySetLookup[_rootSet];
        }

        if (_inventorySetLookup.ContainsKey(cbObject) == false)
        {
            Debug.LogWarning($"Requested Inventory Set: {cbObject.name} cannot be found. Inventory will not refresh");
            return null;
        }

        return _inventorySetLookup[cbObject];
    }

    private void SetInventoryTileCallbacks(VisualElement itemTile, ItemPool pool)
    {
        itemTile.RegisterCallback<MouseDownEvent>(e =>
        {
            Debug.Log("UI Inventory Button Clicked");
            // TODO: Play button click down animation
            // Play button click down sound
            // Register callback for hover exit, on exit pop button back up

            itemTile.RegisterCallback<MouseLeaveEvent>(e =>
            {
                Debug.Log("Mouse Exit, pop button back up and take no action");
            });
        });

        // Add click callbacks for both elements
        itemTile.RegisterCallback<MouseUpEvent>(e =>
        {
            if (e.button == 0)
            {
                GameObject cbObject;
                pool.Pool.Get(out cbObject);
                cbObject.GetComponent<CbObjectStateMachine>().QueueNextState(CbObjectStateMachine.CbObjectState.Spawned);
            }

            if (e.button == 1)
            {
                Debug.Log("RIGHT Mouse down on inventory. Customise Window.");
            }

            StartCoroutine(HideItemTile(itemTile));
        });
    }

    private IEnumerator HideItemTile(VisualElement itemTile)
    {
        itemTile.visible = false;

        yield return new WaitForSeconds(0.6f);

        itemTile.visible = true;
    }
}