using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class ChunkPoolManager : IDisposable
{
    // ① プール本体（実家）
    private readonly Dictionary<GameObject, ObjectPool<GameObject>> _pools = new();
    
    // ② ★追加：貸し出し台帳（どの子が、どの実家から出たか）
    // Key = 貸し出したクローン(Instance), Value = 元のプレハブ(OriginalPrefab)
    private readonly Dictionary<GameObject, GameObject> _spawnedInstancesMap = new();

    private readonly Transform _poolRoot;

    public ChunkPoolManager(Transform poolRoot)
    {
        _poolRoot = poolRoot;
    }

    public GameObject GetChunk(GameObject prefab, Vector3 spawnPosition)
    {
        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = new ObjectPool<GameObject>(
                createFunc: () => Object.Instantiate(prefab, _poolRoot), // ★名札(AddComponent)を貼らなくてよくなる！
                actionOnGet: obj => {
                    obj.transform.position = spawnPosition;
                    obj.gameObject.SetActive(true);
                },
                actionOnRelease: obj => obj.gameObject.SetActive(false),
                actionOnDestroy: obj => Object.Destroy(obj)
            );
        }

        // プールから出す
        GameObject instance = _pools[prefab].Get();

        // ★ 台帳に記録する「このクローンは、このプレハブから出ましたよ」
        _spawnedInstancesMap[instance] = prefab;

        return instance;
    }

    public void ReleaseChunk(GameObject instance)
    {
        // ★ GetComponent（名札の確認）が不要になる！
        // 台帳を調べて、実家（OriginalPrefab）を割り出す
        if (_spawnedInstancesMap.TryGetValue(instance, out GameObject originalPrefab))
        {
            if (_pools.ContainsKey(originalPrefab))
            {
                _pools[originalPrefab].Release(instance);
            }
            // 台帳から消す
            _spawnedInstancesMap.Remove(instance);
        }
        else
        {
            // 台帳にない謎のオブジェクトなら普通に壊す
            Object.Destroy(instance);
        }
    }

    public void Dispose()
    {
        foreach (var pool in _pools.Values) pool.Dispose();
        _pools.Clear();
        _spawnedInstancesMap.Clear(); // 台帳も破棄

        if (_poolRoot != null) Object.Destroy(_poolRoot.gameObject);
    }
}