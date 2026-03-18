using System;

public class RuntimeChunkData
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public ChunkPreset Preset { get; }
    public float StartY { get; }

    public RuntimeChunkData(ChunkPreset preset, float startY)
    {
        Preset = preset;
        StartY = startY;
    }
}
