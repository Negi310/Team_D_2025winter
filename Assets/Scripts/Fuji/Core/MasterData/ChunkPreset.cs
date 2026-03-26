using UnityEngine;

[CreateAssetMenu(fileName = "ChunkPreset", menuName = "Scriptable Objects/ChunkPreset")]
public class ChunkPreset : ScriptableObject
{
    public float ChunkHeight = 10f; // チャンクの長さ
    public GameObject ChunkPrefab;   // 生成するプレハブ
    public Vector2[] LocalLeftBank = new Vector2[10];
    public Vector2[] LocalRightBank = new Vector2[10];
    public Vector2[] LocalCenterLine = new Vector2[10];
    public ChunkConnector EntryType; // このチャンクの入り口の形
    public ChunkConnector ExitType;
    [Range(0f, 1f)] public float CurvinessScore;
    [Range(0f, 1f)] public float NarrownessScore;
}
