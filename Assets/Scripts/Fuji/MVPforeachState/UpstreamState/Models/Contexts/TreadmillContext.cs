using System.Collections.Generic;
using UnityEngine;

public class TreadmillContext
{
    public float CurrentTopY = 0f; // 全プリセットの長さの合計地点（次に生成する場所）
    public float SpawnTriggerY = 0f; // ひとつ前のプリセットの長さの合計地点（生成タイミング）
    public float SpawnDistance = 2f; // 画面下から消えるまでの猶予
    public LinkedList<RuntimeChunkData> ActiveChunks = new();
    public List<Vector2> GlobalLeftBank { get; }
    public List<Vector2> GlobalRightBank { get; }
    public List<Vector2> GlobalCenterLine { get; }

    public TreadmillContext()
    {
        CurrentTopY = 0f;
        SpawnTriggerY = 0f;
        ActiveChunks = new LinkedList<RuntimeChunkData>();
        
        // メモリ再確保を防ぐため、あらかじめ容量（5チャンク×10点）を確保
        GlobalLeftBank = new List<Vector2>(50);
        GlobalRightBank = new List<Vector2>(50);
        GlobalCenterLine = new List<Vector2>(50);
    }
}
