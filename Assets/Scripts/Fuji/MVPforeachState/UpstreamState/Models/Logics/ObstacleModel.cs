using System.Collections.Generic;
using UnityEngine;

public class ObstacleModel
{
    private readonly SplineMathModel _math;
    public ObstacleModel(SplineMathModel math) => _math = math;

    public List<FixedObstacleData> GenerateFixedObstacles(RuntimeChunkData chunk, IReadOnlyList<Vector2> leftBank, IReadOnlyList<Vector2> rightBank, int count, List<FixedObstacleData> existing)
    {
        var results = new List<FixedObstacleData>();
        float startY = chunk.Position, endY = chunk.Position + chunk.Preset.ChunkHeight;

        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                FixedObstacleType type = Random.value > 0.5f ? FixedObstacleType.Rock : FixedObstacleType.FallenTree;
                float radius = type == FixedObstacleType.Rock ? 0.5f : 1.0f;
                float snappedY = Mathf.Floor(Random.Range(startY, endY)) + 0.5f;

                int dummy = 0;
                _math.TryGetXAtY(leftBank, snappedY, ref dummy, out float leftX);
                _math.TryGetXAtY(rightBank, snappedY, ref dummy, out float rightX);

                float safeLeft = leftX + radius, safeRight = rightX - radius;
                if (safeLeft >= safeRight) continue;

                float randomX = Random.Range(safeLeft, safeRight);
                float snappedX = type == FixedObstacleType.Rock ? Mathf.Floor(randomX) + 0.5f : Mathf.Round(randomX);

                if (snappedX < safeLeft || snappedX > safeRight) continue;
                
                Vector2 pos = new Vector2(snappedX, snappedY);
                if (!IsOverlapping(pos, radius, existing) && !IsOverlapping(pos, radius, results))
                {
                    results.Add(new FixedObstacleData(type, pos, radius));
                    break;
                }
            }
        }
        return results;
    }

    private bool IsOverlapping(Vector2 pos, float radius, List<FixedObstacleData> list)
    {
        foreach (var item in list)
            if (Vector2.Distance(pos, item.Position) < (radius + item.Radius)) return true;
        return false;
    }

    public List<DrifterData> GenerateDrifters(RuntimeChunkData chunk, IReadOnlyList<Vector2> leftBank, IReadOnlyList<Vector2> rightBank, int count, float speed)
    {
        var drifters = new List<DrifterData>();
        for (int i = 0; i < count; i++)
        {
            float randomY = Random.Range(chunk.Position, chunk.Position + chunk.Preset.ChunkHeight);
            int dummy = 0;
            _math.TryGetXAtY(leftBank, randomY, ref dummy, out float leftX);
            _math.TryGetXAtY(rightBank, randomY, ref dummy, out float rightX);

            float lane = Random.Range(0.1f, 0.9f);
            drifters.Add(new DrifterData(new Vector2(Mathf.Lerp(leftX, rightX, lane), randomY), speed) { CurrentLane = lane });
        }
        return drifters;
    }
}