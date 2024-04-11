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

    public static void GetConfiguredAmountLabel(ItemPool poolToBind = null)
    {
        //amountLabel.RegisterCallback<GeometryChangedEvent, VisualElement>(NewHeightCallback, amountLabel);

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
    }

    private static void NewHeightCallback(GeometryChangedEvent evt, VisualElement visualElement)
    {
        visualElement.style.width = visualElement.resolvedStyle.height;
    }
}