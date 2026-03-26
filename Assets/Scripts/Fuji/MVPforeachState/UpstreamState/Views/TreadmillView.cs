using System.Collections.Generic;
using UnityEngine;

public class TreadmillView : MonoBehaviour
{
    private PoolManager _poolManager;
    private readonly Dictionary<string, GameObject> _spawnedChunks = new();
    
    public void Init(PoolManager poolManager)
    {
        _poolManager = poolManager;
    }
    
    public void SpawnChunkVisually(RuntimeChunkData data)
    {
        // Presetに登録されたPrefabをPoolから取得して配置
        GameObject instance = _poolManager.Get(data.Preset.ChunkPrefab);
        instance.transform.position = new Vector3(0f, data.Position, 0f);
        _spawnedChunks.Add(data.Id, instance);
    }

    public void DespawnChunkVisually(RuntimeChunkData data)
    {
        if (_spawnedChunks.TryGetValue(data.Id, out GameObject instance))
        {
            _poolManager.Release(instance);
            _spawnedChunks.Remove(data.Id);
        }
    }
    
    public void ClearAll()
    {
        foreach (var instance in _spawnedChunks.Values)
        {
            if (instance != null) _poolManager.Release(instance);
        }
        _spawnedChunks.Clear();
    }
}