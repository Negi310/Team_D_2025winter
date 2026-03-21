using System.Collections.Generic;

public class TreadmillContext
{
    public float CurrentTopY = 0f; // 全プリセットの長さの合計地点（次に生成する場所）
    public float SpawnTriggerY = 0f; // ひとつ前のプリセットの長さの合計地点（生成タイミング）
    public float SpawnDistance = 20f; // 画面下から消えるまでの猶予
    public LinkedList<RuntimeChunkData> ActiveChunks = new();
}
