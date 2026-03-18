using System;

public class RuntimeChunkData
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public ChunkPreset Preset { get; }
    public float Position { get; }

    public RuntimeChunkData(ChunkPreset preset, float position)
    {
        Preset = preset;
        Position = position;
    }
}
