using System.Collections.Generic;

public class RiverDirector
{
    public ChunkPreset GetNextChunkPreset(IReadOnlyList<ChunkPreset> availablePresets)
    {
        // ※将来的にはここに難易度カーブや確率のロジックを実装
        int randomIndex = UnityEngine.Random.Range(0, availablePresets.Count);
        return availablePresets[randomIndex];
    }
}