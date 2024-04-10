using UnityEngine;
using UnityEngine.UIElements;

public static class InventoryTileBuilder
{
    public static VisualElement GetNewConfTile(VisualTreeAsset tileAsset, Texture2D icon = null)
    {
        //VisualElement ve = new VisualElement(tileAsset);
        TemplateContainer itemTileContainer = tileAsset.Instantiate();

        VisualElement itemTile = itemTileContainer.contentContainer.Query<VisualElement>("ItemTile");

        // Resize correctly
        itemTile.style.maxHeight = new StyleLength(new Length(100, LengthUnit.Percent));
        itemTile.style.minHeight = new StyleLength(new Length(100, LengthUnit.Percent));

        // Callback to ensure UI element remains square
        itemTile.RegisterCallback<GeometryChangedEvent, VisualElement>(NewHeightCallback, itemTile);
        
        if (icon != null)
        {
            itemTile.style.backgroundImage = new StyleBackground(icon);
        }

        return itemTileContainer;
    }

    public static VisualElement GetConfiguredItemTile(Texture2D icon = null)
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

        // background
        if (icon != null)
        {
            itemTile.style.backgroundImage = new StyleBackground(icon);
        }

        itemTile.RegisterCallback<GeometryChangedEvent, VisualElement>(NewHeightCallback, itemTile);

        return itemTile;
    }

    public static VisualElement GetConfiguredAmountLabel(ItemPool poolToBind = null)
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
        if (poolToBind != null)
        {
            inventoryItemAmountLabel.text = poolToBind.GetMaxItems().ToString();

            poolToBind.ItemCountUpdate += (newAmount) =>
            {
                inventoryItemAmountLabel.text = newAmount.ToString();
            };
        }
        else
        {
            inventoryItemAmountLabel.text = "99";
        }

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

    private static void NewHeightCallback(GeometryChangedEvent evt, VisualElement visualElement)
    {
        visualElement.style.width = visualElement.resolvedStyle.height;
    }
}