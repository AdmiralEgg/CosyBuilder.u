using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using System;
using UnityEngine.InputSystem.UI;
using UnityEngine.Pool;
using Unity.Properties;

public class InventoryConfigure : MonoBehaviour
{
    [SerializeField]
    ItemPool[] _inventoryUIObjectPools;

    [SerializeField]
    InputSystemUIInputModule _uiInput;

    [SerializeField]
    Mesh _inventoryItemMesh;

    int _currentItems;

    private void Awake()
    {
        var inventoryUI = GetComponent<UIDocument>();

        // Get the parent and set to ignore clicks
        VisualElement inventoryParent = inventoryUI.rootVisualElement.Q<VisualElement>("Inventory");
        inventoryParent.pickingMode = PickingMode.Ignore;

        // foreach required item pool, build a UI element
        foreach (ItemPool pool in _inventoryUIObjectPools)
        {
            //_inventoryItem = _inventoryUI.rootVisualElement.Q<VisualElement>("InvItem1");
            VisualElement itemTile = GetConfiguredItemTile();

            itemTile.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == 0)
                {
                    Debug.Log("LEFT Mouse down on inventory!!");
                    GameObject @object;
                    pool.Pool.Get(out @object);
                    @object.transform.position = new Vector3(0, 4, 0);
                }

                if (e.button == 1) 
                {
                    Debug.Log($"Right mouse down, pop up config window for {pool.name}");
                }
            });

            VisualElement amountLabel = GetConfiguredAmountLabel(pool);

            // Add Child Elements
            itemTile.Add(amountLabel);

            // Add to parent
            inventoryParent.Add(itemTile);
        }
    }

    private void UpdateItemCount()
    {

    }

    private VisualElement  GetConfiguredItemTile()
    {
        VisualElement itemTile = new VisualElement();

        // size
        itemTile.style.maxHeight = new StyleLength(new Length(100, LengthUnit.Percent));
        itemTile.style.minHeight = new StyleLength(new Length(100, LengthUnit.Percent));

        // boarder
        itemTile.style.borderLeftWidth = 3f;
        itemTile.style.borderRightWidth = 3f;
        itemTile.style.borderTopWidth = 3f;
        itemTile.style.borderBottomWidth = 3f;
        itemTile.style.borderTopLeftRadius = 5f;
        itemTile.style.borderTopRightRadius = 5f;
        itemTile.style.borderBottomLeftRadius = 5f;
        itemTile.style.borderBottomRightRadius = 5f;

        // margins
        itemTile.style.marginLeft = 3f;
        itemTile.style.marginRight = 3f;

        // boarder colour
        StyleColor boarderStyle = new StyleColor();
        boarderStyle.value = Color.grey;
        itemTile.style.borderLeftColor = boarderStyle;
        itemTile.style.borderRightColor = boarderStyle;
        itemTile.style.borderTopColor = boarderStyle;
        itemTile.style.borderBottomColor = boarderStyle;

        itemTile.RegisterCallback<GeometryChangedEvent, VisualElement>(NewHeightCallback, itemTile);

        return itemTile;
    }

    private VisualElement GetConfiguredAmountLabel(ItemPool poolToBind)
    {
        VisualElement amountLabel = new VisualElement();

        amountLabel.style.position = Position.Absolute;
        amountLabel.style.right = -5;
        amountLabel.style.bottom = -5;

        // boarder thickness
        amountLabel.style.borderLeftWidth = 3f;
        amountLabel.style.borderRightWidth = 3f;
        amountLabel.style.borderTopWidth = 3f;
        amountLabel.style.borderBottomWidth = 3f;
        amountLabel.style.borderTopLeftRadius = 5f;
        amountLabel.style.borderTopRightRadius = 5f;
        amountLabel.style.borderBottomLeftRadius = 5f;
        amountLabel.style.borderBottomRightRadius = 5f;

        // background color
        var backgroundColor = new StyleColor();
        backgroundColor.value = Color.white;
        amountLabel.style.backgroundColor = backgroundColor;

        // boarder colour
        StyleColor boarderStyle = new StyleColor();
        boarderStyle.value = Color.grey;
        amountLabel.style.borderLeftColor = boarderStyle;
        amountLabel.style.borderRightColor = boarderStyle;
        amountLabel.style.borderTopColor = boarderStyle;
        amountLabel.style.borderBottomColor = boarderStyle;

        amountLabel.BringToFront();

        amountLabel.RegisterCallback<GeometryChangedEvent, VisualElement>(NewHeightCallback, amountLabel);

        Label inventoryItemAmountLabel = new Label();

        // set default value and bind using an event
        inventoryItemAmountLabel.text = poolToBind.GetMaxItems().ToString();

        poolToBind.ItemCountUpdate += (newAmount) =>
        {
            inventoryItemAmountLabel.text = newAmount.ToString();
        };

        inventoryItemAmountLabel.style.textShadow = new StyleTextShadow();
        inventoryItemAmountLabel.style.marginBottom = 0;
        inventoryItemAmountLabel.style.marginTop = 0;
        inventoryItemAmountLabel.style.marginLeft = 0;
        inventoryItemAmountLabel.style.marginRight = 0;
        inventoryItemAmountLabel.style.paddingBottom = 0;
        inventoryItemAmountLabel.style.paddingTop = 0;
        inventoryItemAmountLabel.style.paddingLeft = 0;
        inventoryItemAmountLabel.style.paddingRight = 0;
        inventoryItemAmountLabel.style.alignSelf = Align.Center;
        inventoryItemAmountLabel.style.color = Color.black;

        amountLabel.pickingMode = PickingMode.Ignore;
        inventoryItemAmountLabel.pickingMode = PickingMode.Ignore;

        amountLabel.Add(inventoryItemAmountLabel);

        return amountLabel;
    }

    private void NewHeightCallback(GeometryChangedEvent evt, VisualElement visualElement)
    {
        visualElement.style.width = visualElement.resolvedStyle.height;
    }
}