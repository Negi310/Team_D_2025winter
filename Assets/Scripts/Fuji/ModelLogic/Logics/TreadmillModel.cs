using System;
using System.Collections.Generic;

public class RiverTreadmillModel
{
    private readonly RiverDirector _director;

    private float _currentTopY = 0f;       // 全プリセットの長さの合計地点（次に生成する場所）
    private float _spawnTriggerY = 0f; // ひとつ前のプリセットの長さの合計地点（生成タイミング）
    
    private readonly float _spawnDistance = 20f; // 画面下から消えるまでの猶予

    private readonly LinkedList<RuntimeChunkData> _activeChunks = new();

    public event Action<RuntimeChunkData> OnChunkSpawnRequested;
    public event Action<RuntimeChunkData> OnChunkDespawnRequested;

    public RiverTreadmillModel(RiverDirector director)
    {
        _director = director;
    }

    public void UpdatePlayerPosition(float playerY)
    {
        // プレイヤーが「最新のひとつ前の頂点」に到達したら、次を作る！
        if (playerY >= _spawnTriggerY + _spawnDistance)
        {
            var oldestChunk = _activeChunks.First.Value;
            _activeChunks.RemoveFirst();
            OnChunkDespawnRequested?.Invoke(oldestChunk);
            SpawnNextChunk();
        }
    }

    // 生成処理を1つのメソッドにまとめる
    public void SpawnNextChunk()
    {
        var preset = _director.GetNextChunkPreset();
        var chunkData = new RuntimeChunkData(preset, _currentTopY);
        
        _activeChunks.AddLast(chunkData);
        
        _spawnTriggerY = _currentTopY; 
        
        _currentTopY += preset.ChunkHeight;

        OnChunkSpawnRequested?.Invoke(chunkData);
    }
}