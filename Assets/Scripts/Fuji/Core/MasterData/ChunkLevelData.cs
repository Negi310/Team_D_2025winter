using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChunkLevelData", menuName = "Scriptable Objects/ChunkLevelData")]
public class ChunkLevelData : ScriptableObject
{
    public List<ChunkPreset> AvailableChunkPresets;
}
