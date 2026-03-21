using System.Collections.Generic;

public class TreadmillModel
{
    private readonly RiverDirector _director;

    public TreadmillModel(RiverDirector director)
    {
        _director = director;
    }

    public TreadmillUpdateResult UpdatePlayerPosition(TreadmillContext context, IReadOnlyList<ChunkPreset> presets, float playerY)
    {
        var result = new TreadmillUpdateResult();
        
        if (playerY >= context.SpawnTriggerY + context.SpawnDistance)
        {
            var oldestChunk = context.ActiveChunks.First.Value;
            context.ActiveChunks.RemoveFirst();
            result.DespawnedChunk = oldestChunk;
            result.SpawnedChunk = SpawnNextChunk(context, presets);
        }
        return result;
    }

    // 生成処理を1つのメソッドにまとめる
    public RuntimeChunkData SpawnNextChunk(TreadmillContext context, IReadOnlyList<ChunkPreset> presets)
    {
        var preset = _director.GetNextChunkPreset(presets);
        var chunkData = new RuntimeChunkData(preset, context.CurrentTopY);
        
        context.ActiveChunks.AddLast(chunkData);
        context.SpawnTriggerY = context.CurrentTopY; 
        context.CurrentTopY += preset.ChunkHeight;
        
        return chunkData;
    }
}