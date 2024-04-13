using UnityEngine;

public class InventoryObjectPoolController : MonoBehaviour
{
    public ItemPool InitializeInventoryTile(CbObjectScriptableData cbObjectData)
    {
        GameObject poolObject = new GameObject();
        poolObject.transform.parent = this.transform;
        poolObject.name = $"{cbObjectData.UIName}_CbObjectPool";

        var itemPool = poolObject.AddComponent<ItemPool>();
        itemPool.SetupItemPool(cbObjectData.Prefab, prewarmPool: true);

        return itemPool;
    }
}