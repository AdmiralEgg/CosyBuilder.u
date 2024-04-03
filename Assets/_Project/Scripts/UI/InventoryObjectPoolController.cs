using UnityEngine;
using UnityEngine.UIElements;

public class InventoryObjectPoolController : MonoBehaviour
{
    public ItemPool InitializeInventoryTile(CbObjectScriptableData cbObjectdata)
    {
        Debug.Log($"Got new inventory item {cbObjectdata}");
        
        ItemPool itemPool = ConfigureObjectPool(cbObjectdata);

        return itemPool;
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
}