using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    private readonly Dictionary<string, UnityEngine.Object> _cache = new();
    public bool IsPreloadComplete { get; private set; } = false;
    
    //라벨 기반 일괄 로딩
    public void Preload(string label = "preload")
    {
        LoadAllAsync<UnityEngine.Object>(label, (key, count, total) =>
        {
            if (count == total)
            {
                Debug.Log("Preload Complete");
                IsPreloadComplete = true;
            }
        });
    }
    
    //Load - 캐시된 것만 반환
    public T Load<T>(string key) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.Log($"[ResourceManager] {key} is null or empty");
            return null;
        }

        if (_cache.TryGetValue(key, out var obj))
        {
            return obj as T;
        }
        
        //Sprite 키 추가 보정
        if (typeof(T) == typeof(Sprite))
        {
            string spriteKey = key + ".sprite";
            if (_cache.TryGetValue(spriteKey, out var sprite))
            {
                return sprite as T;
            }
        }

        return null;
    }
    
    //Instantiate - 캐시된 prefab 기반
    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>(key);

        if (prefab == null)
        {
            Debug.Log($"[ResourceManager] Instantiate failed: prefab not loaded -> {key}");
            return null;
        }
        
        //Pooling
        if (pooling)
            return;

        GameObject go = UnityEngine.Object.Instantiate(prefab, parent);
        go.name = prefab.name;

        SetOriginKey(go, key);
        return go;
    }
    
    //Destroy (Pool -> fallback Destroy)
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        string key = GetOriginKey(go);
        if (!string.IsNullOrEmpty(key))
        {
            if (Push(key, go))
                return;
        }
        
        UnityEngine.Object.Destroy(go);
    }
    

    //Addressables 비동기 로딩
    public async Task<T> LoadAsync<T>(string key) where T : UnityEngine.Object
    {
        //캐시 존재 시 즉시 반환
        if (_cache.TryGetValue(key, out var cached))
            return cached as T;
        
        //Sprite 보정 키
        string loadKey = key;
        if (key.EndsWith(".sprite"))
        {
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";
        }

        AsyncOperationHandle<T> op = Addressables.LoadAssetAsync<T>(loadKey);
        await op.Task;

        if (op.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"[ResourceManager] Failed to load: {key}");
            return null;
        }
        
        //캐시에 저장
        if (!_cache.ContainsKey(key))
        {
            _cache.Add(key, op.Result);
        }

        return op.Result;
    }

    //라벨 기반 일괄 로딩
    public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        var handle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

        handle.Completed += async (op) =>
        {
            int count = 0;
            int total = op.Result.Count;

            foreach (var loc in op.Result)
            {
                string key = loc.PrimaryKey;
                if (key.EndsWith(".sprite"))
                {
                    await LoadAsync<Sprite>(key);
                }
                else
                {
                    await LoadAsync<T>(key);
                }

                count++;
                callback?.Invoke(key, count, total);
            }
        };
    }
    
    //original Key 저장/획득
    private void SetOriginKey(GameObject go, string key)
    {
        var holder = go.GetComponent<ResourceOrigin>();
        if (holder == null)
            holder = go.AddComponent<ResourceOrigin>();

        holder.originKey = key;
    }

    private string GetOriginKey(GameObject go)
    {
        var holder = go.GetComponent<ResourceOrigin>();
        return holder != null ? holder.originKey : null;
    }
    
    //Clear Cache
    public void Clear()
    {
        _cache.Clear();
        IsPreloadComplete = false=;
    }
}
