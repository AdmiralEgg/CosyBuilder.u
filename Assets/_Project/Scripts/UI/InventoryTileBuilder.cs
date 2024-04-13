using UnityEngine;
using UnityEngine.UIElements;

public static class InventoryTileBuilder
{
    public static VisualElement GetItemTile(VisualTreeAsset tileAsset, Texture2D icon = null)
    {
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

    public static void ConfigureAmountLabel(ItemPool poolToBind, Label amountLabel)
    {
        amountLabel.RegisterCallback<GeometryChangedEvent, VisualElement>(NewHeightCallback, amountLabel);

        // set default value and bind using an event
        if (poolToBind != null)
        {
            amountLabel.text = poolToBind.GetMaxItems().ToString();

            poolToBind.ItemCountUpdate += (newAmount) =>
            {
                amountLabel.text = newAmount.ToString();
            };
        }
        else
        {
            amountLabel.text = "99";
        }
    }

    private static void NewHeightCallback(GeometryChangedEvent evt, VisualElement visualElement)
    {
        visualElement.style.width = visualElement.resolvedStyle.height;
    }
}