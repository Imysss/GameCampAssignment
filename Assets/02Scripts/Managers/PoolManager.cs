using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
    //여러 prefab-key를 기준으로 Pool을 따로 관리
    private readonly Dictionary<string, IObjectPool<GameObject>> _pools = new();
    
    //하이어라키 정리용
    private readonly Dictionary<string, Transform> _poolRoots = new();
    
    //Create Pool (Prefab + Key)
    public void CreatePool(string key, GameObject prefab, int defaultCapacity = 10, int maxSize = 100)
    {
        if (_pools.ContainsKey(key))
            return;

        Transform root = new GameObject($"{key}_Pool").transform;
        root.SetParent(transform);
        _poolRoots[key] = root;
        
        var pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab);  
                obj.name = prefab.name;
                obj.SetActive(false);
                obj.transform.SetParent(root);

                return obj;
            },
            actionOnGet: obj =>
            {
                obj.SetActive(true);
                
                //Spawn 콜백
                if(obj.TryGetComponent(out ISpawnable spawnable))
                    spawnable.OnSpawn();
            },
            actionOnRelease: obj =>
            {
                obj.SetActive(false);
                obj.transform.SetParent(root);
                
                //Despawn 콜백
                if(obj.TryGetComponent(out ISpawnable spawnable))
                    spawnable.OnDespawn();
            },
            actionOnDestroy: obj =>
            {
                Destroy(obj);
            },
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
        
        _pools[key] = pool;
    }

    //Pop (Spawn)
    public GameObject Pop(GameObject prefab, string key)
    {
        //존재하지 않으면 즉시 Pool 생성
        if (!_pools.ContainsKey(key))
        {
            CreatePool(key, prefab);
        }

        GameObject go = _pools[key].Get();
        
        //originKey 저장 -> ResourceManager Destroy에서 사용
        SetOriginKey(go, key);

        return go;
    }
    
    //Push(Despawn)
    public bool Push(GameObject go)
    {
        var origin = go.GetComponent<ResourceOrigin>();
        if (origin == null || string.IsNullOrEmpty(origin.originKey))
            return false;

        return Push(origin.originKey, go);
    }
    
    //Push(Despawn)
    public bool Push(string key, GameObject go)
    {
        if (!_pools.ContainsKey(key)) 
            return false; 
        _pools[key].Release(go); 
        
        return true;
    }
    
    
    //Clear All Pools
    public void Clear()
    {
        foreach (var pool in _pools.Values)
        {
            pool.Clear();
        }
        
        _pools.Clear();
        _poolRoots.Clear();
    }
    
    //origin key 저장
    private void SetOriginKey(GameObject go, string key)
    {
        var comp = go.GetComponent<ResourceOrigin>();
        if(comp == null)
            comp = go.AddComponent<ResourceOrigin>();

        comp.originKey = key;
    }
}
