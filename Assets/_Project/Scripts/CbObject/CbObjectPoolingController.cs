using Sirenix.OdinInspector;
using UnityEngine;

public class ReturnToItemPool : MonoBehaviour
{
    [SerializeField, ReadOnly]
    ItemPool _spawnPool;

    public void SetSpawnPool(ItemPool spawnPool)
    {
        _spawnPool = spawnPool;
    }

    public void ReturnToSpawnPool()
    {
        _spawnPool.Pool.Release(this.gameObject);
    }
}
