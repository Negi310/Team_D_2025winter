using UnityEngine;
using System.Collections.Generic;

public class RiverPath
{
    public void AddChunkSplines(TreadmillContext context, ChunkPreset preset, float spawnY)
    {
        // プリセットのローカル座標に生成Y座標を足して、グローバル座標として末尾に結合
        for (int i = 0; i < preset.LocalLeftBank.Length; i++)
        {
            context.GlobalLeftBank.Add(new Vector2(preset.LocalLeftBank[i].x, preset.LocalLeftBank[i].y + spawnY));
            context.GlobalRightBank.Add(new Vector2(preset.LocalRightBank[i].x, preset.LocalRightBank[i].y + spawnY));
            context.GlobalCenterLine.Add(new Vector2(preset.LocalCenterLine[i].x, preset.LocalCenterLine[i].y + spawnY));
        }
    }

    public void RemoveOldestChunkSplines(TreadmillContext context, int pointsPerChunk = 10)
    {
        // 最も古い（後ろに過ぎ去った）チャンクの点群を先頭から削除
        if (context.GlobalLeftBank.Count >= pointsPerChunk)
        {
            context.GlobalLeftBank.RemoveRange(0, pointsPerChunk);
            context.GlobalRightBank.RemoveRange(0, pointsPerChunk);
            context.GlobalCenterLine.RemoveRange(0, pointsPerChunk);
        }
    }
    
    public Vector2 GetDrifterTargetPoint(float currentY, float lookAheadDistance, float lane, IReadOnlyList<Vector2> globalLeft, IReadOnlyList<Vector2> globalRight, SplineMathModel math)
    {
        float targetY = currentY + lookAheadDistance;
        int dummy = 0;
        bool hasLeft = math.TryGetXAtY(globalLeft, targetY, ref dummy, out float leftX);
        bool hasRight = math.TryGetXAtY(globalRight, targetY, ref dummy, out float rightX);

        if (hasLeft && hasRight) return new Vector2(Mathf.Lerp(leftX, rightX, lane), targetY);
        return new Vector2(0f, targetY);
    }
}