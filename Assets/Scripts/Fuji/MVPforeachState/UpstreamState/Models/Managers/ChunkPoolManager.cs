using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class PoolManager : IDisposable
{
    // ① プール本体（実家）
    private readonly Dictionary<GameObject, ObjectPool<GameObject>> _pools = new();
    
    // ② ★追加：貸し出し台帳（どの子が、どの実家から出たか）
    // Key = 貸し出したクローン(Instance), Value = 元のプレハブ(OriginalPrefab)
    private readonly Dictionary<GameObject, GameObject> _spawnedInstancesMap = new();

    private readonly Transform _poolRoot;

    public PoolManager(Transform poolRoot)
    {
        _poolRoot = poolRoot;
    }

    public GameObject Get(GameObject prefab)
    {
        if (!_pools.ContainsKey(prefab))
        {
            // 初めて要求されたプレハブなら、専用のプールを自動生成する（遅延初期化）
            _pools[prefab] = new ObjectPool<GameObject>(
                createFunc: () => Object.Instantiate(prefab, _poolRoot),
                actionOnGet: obj => obj.gameObject.SetActive(true),
                actionOnRelease: obj => obj.gameObject.SetActive(false),
                actionOnDestroy: obj => Object.Destroy(obj)
            );
        }

        GameObject instance = _pools[prefab].Get();
        _spawnedInstancesMap[instance] = prefab; // 台帳に記録

        return instance;
    }

    // ★ 変更点: 名前を汎用的な Release に変更
    public void Release(GameObject instance)
    {
        if (_spawnedInstancesMap.TryGetValue(instance, out GameObject originalPrefab))
        {
            if (_pools.ContainsKey(originalPrefab))
            {
                _pools[originalPrefab].Release(instance);
            }
            _spawnedInstancesMap.Remove(instance);
        }
        else
        {
            // 台帳にない謎のオブジェクト（手動で置かれたもの等）なら普通に壊す
            Object.Destroy(instance);
        }
    }

    public void Dispose()
    {
        foreach (var pool in _pools.Values) pool.Dispose();
        _pools.Clear();
        _spawnedInstancesMap.Clear();

        if (_poolRoot != null) Object.Destroy(_poolRoot.gameObject);
    }
}