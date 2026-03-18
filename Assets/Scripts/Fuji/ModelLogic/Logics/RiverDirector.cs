using System.Collections.Generic;
using UnityEngine;

public class RiverDirector
{
    private readonly List<ChunkPreset> _availablePresets;

    public RiverDirector(List<ChunkPreset> availablePresets)
    {
        _availablePresets = availablePresets;
    }

    public ChunkPreset GetNextChunkPreset()
    {
        // ※将来的にはここに難易度カーブや確率のロジックを実装
        int randomIndex = UnityEngine.Random.Range(0, _availablePresets.Count);
        return _availablePresets[randomIndex];
    }
}