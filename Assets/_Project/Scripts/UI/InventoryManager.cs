using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;
using ImGuiNET;
using static DictionarySerialization;

public class InventoryManager : MonoBehaviour
{
    [SerializeField, Required]
    private AssetLabelReference _cbObjectDataLabel;

    [SerializeField, Required]
    private VisualTreeAsset _itemTileAsset;

    [Header("Inventory Lookups")]
    [SerializeField, ReadOnly]
    private SerializableDictionary<CbObjectScriptableData, List<CbObjectScriptableData>> _inventorySetLookup = new SerializableDictionary<CbObjectScriptableData, List<CbObjectScriptableData>>();

    [SerializeField, ReadOnly]
    private SerializableDictionary<CbObjectScriptableData, VisualElement> _itemTileLookup = new SerializableDictionary<CbObjectScriptableData, VisualElement>();

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

            // TODO: Get a reference to the current focus from the GameModeStateManager.
            if (state == GameModeStateMachine.GameModeState.Focus)
            {
                // Get the object focused (GameModeStateMachine.CurrentFocus)

                // Refresh inventory with the focus object data
            }
        };

        // Hide inventory on blending
        SendCameraBlendEvents.CameraBlendStarted += HideInventory;
        SendCameraBlendEvents.CameraBlendFinished += ShowInventory;
    }

    private void OnEnable() => ImGuiUn.Layout += OnImGuiLayout;
    private void OnDisable() => ImGuiUn.Layout -= OnImGuiLayout;

    private void HideInventory()
    {
        Debug.Log("Hide inventory");
        _inventoryContainerVisualElement.AddToClassList("hidden");
    }

    private void ShowInventory()
    {
        Debug.Log("Show inventory");
        _inventoryContainerVisualElement.RemoveFromClassList("hidden");
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
                
                VisualElement itemTile = InstantiateItemTile(cbObject);

                _itemTileLookup.Add(cbObject, itemTile);

                // Initialize InventoryPool
                InstantiateInventoryPool(cbObject, itemTile);

                // Set callbacks
                SetInventoryTileCallbacks(_itemTileLookup[cbObject], _inventoryPoolLookup[cbObject]);
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

    private void InstantiateInventoryPool(CbObjectScriptableData cbObject, VisualElement itemTile)
    {
        ItemPool newPool = _inventoryPoolController.InitializeInventoryTile(cbObject);

        // get link the item value to the pool
        Label itemAmount = itemTile.Q<Label>("ItemAmountLabel");
        InventoryTileBuilder.ConfigureAmountLabel(newPool, itemAmount);

        _inventoryPoolLookup.Add(cbObject, newPool);
    }

    /// <summary>
    /// Adds a CbObject to an inventorySet, then initializes an inventory tile and stores it in a dictionary.
    /// </summary>
    /// <param name="cbObject"></param>
    private VisualElement InstantiateItemTile(CbObjectScriptableData cbObject)
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

        return InventoryTileBuilder.GetItemTile(_itemTileAsset, cbObject.InventoryIcon);
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
            VisualElement inventoryTile = _itemTileLookup[inventoryObject];

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

        itemTile.RegisterCallback<MouseUpEvent>(e =>
        {
            // Spawn Item
            if (e.button == 0)
            {
                if (pool.CurrentItemCount == 0)
                {
                    Debug.Log("No items to spawn, return");
                    return;
                }
                
                GameObject cbObject;
                pool.Pool.Get(out cbObject);
                cbObject.GetComponent<CbObjectStateMachine>().QueueNextState(CbObjectStateMachine.CbObjectState.Spawned);
                StartCoroutine(HideItemTile(itemTile));
            }

            // Customise Item
            if (e.button == 1)
            {
                Debug.Log("RIGHT Mouse down on inventory. Customise Window.");
            }
        });
    }

    private IEnumerator HideItemTile(VisualElement itemTile)
    {
        itemTile.visible = false;

        yield return new WaitForSeconds(0.6f);

        itemTile.visible = true;
    }

    private void OnImGuiLayout()
    {
        if (ImGui.CollapsingHeader("Inventory", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Refresh Inventory"))
            {
                RefreshInventoryList();
            }
        }
    }
}