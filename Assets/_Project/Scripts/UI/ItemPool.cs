using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Pool;

public class ItemPool : MonoBehaviour
{
    [SerializeField, AssetsOnly]
    GameObject _item;

    [SerializeField, ReadOnly]
    public ObjectPool<GameObject> Pool;

    [SerializeField]
    int _maxItems = 4;

    [SerializeField, ReadOnly]
    private int _currentItemCount;

    [CreateProperty]
    public int CurrentItemCount
    {
        get => CurrentItemCount;
        set
        {
            _currentItemCount = value;
            ItemCountUpdate?.Invoke(_currentItemCount);
        }
    }

    bool _collectionChecks = false;

    [SerializeField]
    bool _prewarmPool = true;

    public event Action<int> ItemCountUpdate;

    void Awake()
    {
        // do some object pooling
        Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, _collectionChecks, 10, 20);

        if (_prewarmPool)
        {
            PrewarmPool();
        }
    }

    private void PrewarmPool()
    {
        List<GameObject> pooledItems = new List<GameObject>();
        
        for (int i = 0; i < _maxItems; i++)
        {
            pooledItems.Add(Pool.Get());
        }
            
        foreach (GameObject item in pooledItems) 
        {
            Pool.Release(item);
        }

        CurrentItemCount = Pool.CountInactive;
    }

    private void OnDestroyPoolObject(GameObject @object)
    {
        // Create a new pooled object?
        @object.gameObject.SetActive(false);
        CurrentItemCount = Pool.CountInactive;
    }

    private void OnReturnedToPool(GameObject @object)
    {
        @object.gameObject.SetActive(false);
        CurrentItemCount = Pool.CountInactive;
    }

    private void OnTakeFromPool(GameObject @object)
    {
        @object.gameObject.SetActive(true);
        CurrentItemCount = Pool.CountInactive;
    }

    private GameObject CreatePooledItem()
    {
        GameObject tmp = Instantiate(_item);
        tmp.GetComponent<ReturnToItemPool>().SetSpawnPool(this);
        tmp.transform.parent = this.transform;
        tmp.transform.position = new Vector3(8, 0, 20);
        return tmp;
    }

    public int GetMaxItems()
    {
        return _maxItems;
    }
}
