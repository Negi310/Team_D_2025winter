using System.Collections.Generic;
using UnityEngine;

public class TreadmillView : MonoBehaviour
{
    private ChunkPoolManager _poolManager;
    private readonly Dictionary<string, GameObject> _spawnedChunks = new();
    
    public void Init(ChunkPoolManager poolManager)
    {
        _poolManager = poolManager;
    }
    
    public void SpawnChunkVisually(RuntimeChunkData data)
    {
        // Presetに登録されたPrefabをPoolから取得して配置
        Vector3 spawnPos = new Vector3(0, data.Position, 0);
        GameObject instance = _poolManager.GetChunk(data.Preset.ChunkPrefab, spawnPos);
        _spawnedChunks.Add(data.Id, instance);
    }

    public void DespawnChunkVisually(RuntimeChunkData data)
    {
        if (_spawnedChunks.TryGetValue(data.Id, out GameObject instance))
        {
            _poolManager.ReleaseChunk(instance);
            _spawnedChunks.Remove(data.Id);
        }
    }
}