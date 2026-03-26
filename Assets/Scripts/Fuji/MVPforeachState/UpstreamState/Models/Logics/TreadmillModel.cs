using System.Collections.Generic;

public class TreadmillModel
{
    private readonly RiverDirector _director;

    public TreadmillModel(RiverDirector director) => _director = director;

    public void UpdatePlayerPosition(SessionContext sessionContext, TreadmillContext treadmillContext, IReadOnlyList<ChunkPreset> presets, float playerY, out List<RuntimeChunkData> spawned, out List<RuntimeChunkData> despawned)
    {
        spawned = new List<RuntimeChunkData>();
        despawned = new List<RuntimeChunkData>();
        
        while (playerY >= treadmillContext.SpawnTriggerY + treadmillContext.SpawnDistance)
        {
            var newChunk = SpawnNextChunk(sessionContext, treadmillContext, presets);
            spawned.Add(newChunk);
        }

        while (treadmillContext.ActiveChunks.Count > 0)
        {
            var oldestChunk = treadmillContext.ActiveChunks.First.Value;
            if (oldestChunk.Position + oldestChunk.Preset.ChunkHeight < playerY - 20f)
            {
                treadmillContext.ActiveChunks.RemoveFirst();
                despawned.Add(oldestChunk);
            }
            else
            {
                break;
            }
        }
    }

    public RuntimeChunkData SpawnNextChunk(SessionContext sessionContext, TreadmillContext treadmillContext, IReadOnlyList<ChunkPreset> presets)
    {
        // ★修正: 初期値を代入しておくことでC#のコンパイルエラーを防ぐ
        ChunkConnector requiredEntry = ChunkConnector.Aj; 
        if (treadmillContext.ActiveChunks.Count > 0)
        {
            requiredEntry = treadmillContext.ActiveChunks.Last.Value.Preset.ExitType;
        }

        var preset = _director.GetNextChunkPreset(presets, sessionContext.CurrentRiver, requiredEntry);
        var chunkData = new RuntimeChunkData(preset, treadmillContext.CurrentTopY);
        
        treadmillContext.ActiveChunks.AddLast(chunkData);
        treadmillContext.SpawnTriggerY = treadmillContext.CurrentTopY; 
        treadmillContext.CurrentTopY += preset.ChunkHeight;
        
        return chunkData;
    }
}