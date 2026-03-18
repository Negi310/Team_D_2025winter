using UnityEngine;

[CreateAssetMenu(fileName = "ChunkPreset", menuName = "Scriptable Objects/ChunkPreset")]
public class ChunkPreset : ScriptableObject
{
    public float ChunkHeight = 100f; // チャンクの長さ
    public GameObject ChunkPrefab;   // 生成するプレハブ
}
