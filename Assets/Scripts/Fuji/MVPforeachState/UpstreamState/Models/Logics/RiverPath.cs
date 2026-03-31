using UnityEngine;
using System.Collections.Generic;

public class RiverPath
{
    public void AddChunkSplines(TreadmillContext context, ChunkPreset preset, float spawnY)
    {
        for (int i = 0; i < preset.LocalLeftBank.Length; i++)
        {
            context.GlobalLeftBank.Add(new Vector2(preset.LocalLeftBank[i].x, preset.LocalLeftBank[i].y + spawnY));
            context.GlobalRightBank.Add(new Vector2(preset.LocalRightBank[i].x, preset.LocalRightBank[i].y + spawnY));
            context.GlobalCenterLine.Add(new Vector2(preset.LocalCenterLine[i].x, preset.LocalCenterLine[i].y + spawnY));
        }
    }

    // ★修正: pointsPerChunk のデフォルト(10)を廃止し、正確な数を受け取って削除する
    public void RemoveOldestChunkSplines(TreadmillContext context, int countToRemove)
    {
        if (context.GlobalLeftBank.Count >= countToRemove)
        {
            context.GlobalLeftBank.RemoveRange(0, countToRemove);
            context.GlobalRightBank.RemoveRange(0, countToRemove);
            context.GlobalCenterLine.RemoveRange(0, countToRemove);
        }
    }
    
    public Vector2 GetDrifterTargetPoint(float currentY, float lookAheadDistance, float lane, IReadOnlyList<Vector2> globalLeft, IReadOnlyList<Vector2> globalRight, SplineMathModel math)
    {
        float targetY = currentY + lookAheadDistance;
        
        bool hasLeft = math.TryGetXAtY(globalLeft, targetY, out float leftX);
        bool hasRight = math.TryGetXAtY(globalRight, targetY, out float rightX);

        if (hasLeft && hasRight) return new Vector2(Mathf.Lerp(leftX, rightX, lane), targetY);
        return new Vector2(0f, targetY);
    }
}