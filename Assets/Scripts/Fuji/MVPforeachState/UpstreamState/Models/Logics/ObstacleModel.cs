using System.Collections.Generic;
using UnityEngine;

public class ObstacleModel
{
    private readonly SplineMathModel _math;
    private const float VisualMargin = 0f;
    private readonly GameSetting _settings;

    public ObstacleModel(SplineMathModel math, GameSetting settings)
    {
        _math = math;
        _settings = settings;
    }

    // ==========================================
    // 固定設置物の生成
    // ==========================================
    public List<FixedObstacleData> GenerateFixedObstacles(RuntimeChunkData chunk, IReadOnlyList<Vector2> leftBank, IReadOnlyList<Vector2> rightBank, List<FixedObstacleData> existing, RiverData river)
    {
        var results = new List<FixedObstacleData>();
        float startY = chunk.Position, endY = chunk.Position + chunk.Preset.ChunkHeight;
        
        int count = Mathf.Clamp(Mathf.RoundToInt(river.ObstacleDensity), 0, _settings.MaxFixedObstaclesPerChunk);
        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                FixedObstacleType type = Random.value > 0.5f ? FixedObstacleType.Rock : FixedObstacleType.FallenTree;
                float physicalRadius = type == FixedObstacleType.Rock ? _settings.RockRadius : _settings.FallenTreeRadius;
                float requiredSpace = physicalRadius + VisualMargin;

                float snappedY = Mathf.Floor(Random.Range(startY, endY)) + 0.5f;
                
                if (!_math.TryGetXAtY(leftBank, snappedY, out float leftX) ||
                    !_math.TryGetXAtY(rightBank, snappedY, out float rightX))
                {
                    continue; 
                }

                float safeLeft = leftX + requiredSpace / 2;
                float safeRight = rightX - requiredSpace / 2;

                if (safeLeft >= safeRight) continue;

                float snappedX = 0f;
                if (type == FixedObstacleType.FallenTree)
                {
                    float edgeX = Random.value > 0.5f ? safeLeft : safeRight;
                    snappedX = Mathf.Round(edgeX);
                }
                else
                {
                    float randomX = Random.Range(safeLeft, safeRight);
                    snappedX = Mathf.Floor(randomX) + 0.5f;
                }

                if (snappedX < safeLeft || snappedX > safeRight) continue;
                
                Vector2 pos = new Vector2(snappedX, snappedY);
                if (!IsOverlapping(pos, requiredSpace, existing) && !IsOverlapping(pos, requiredSpace, results))
                {
                    results.Add(new FixedObstacleData(type, pos, physicalRadius));
                    break;
                }
            }
        }
        return results;
    }

    private bool IsOverlapping(Vector2 pos, float requiredSpace, List<FixedObstacleData> list)
    {
        foreach (var item in list)
        {
            float totalRequiredDistance = requiredSpace + (item.Radius + VisualMargin);
            if (Vector2.Distance(pos, item.Position) < totalRequiredDistance) return true;
        }
        return false;
    }

    // ==========================================
    // 漂流物の生成（グリッドスナップ化）
    // ==========================================
    public List<DrifterData> GenerateDrifters(RuntimeChunkData chunk, IReadOnlyList<Vector2> leftBank, IReadOnlyList<Vector2> rightBank, RiverData river)
    {
        var drifters = new List<DrifterData>();
        float startY = chunk.Position + 0.5f * chunk.Preset.ChunkHeight, endY = chunk.Position + 1.5f * chunk.Preset.ChunkHeight;
        
        float totalDensity = river.FishDensity + river.RivalDensity + river.AccidentDensity;
        int count = Mathf.Clamp(Mathf.RoundToInt(totalDensity * _settings.DrifterDensityMultiplier), 0, _settings.MaxDriftersPerChunk);
        
        float fishProb = river.FishDensity / Mathf.Max(totalDensity, 1f);
        float rivalProb = fishProb + (river.RivalDensity / Mathf.Max(totalDensity, 1f));

        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                float snappedY = Mathf.Floor(Random.Range(startY, endY)) + 0.5f;
            
                if (!_math.TryGetXAtY(leftBank, snappedY, out float leftX) ||
                    !_math.TryGetXAtY(rightBank, snappedY, out float rightX)) continue;
                
                float safeLeft = leftX + 0.5f; float safeRight = rightX - 0.5f;
                if (safeLeft >= safeRight) continue;

                float snappedX = Mathf.Floor(Random.Range(safeLeft, safeRight)) + 0.5f;
                if (snappedX < safeLeft || snappedX > safeRight) continue;

                float lane = Mathf.InverseLerp(leftX, rightX, snappedX);
                Vector2 startPos = new Vector2(snappedX, snappedY);
                
                float rand = Random.value;
                DrifterType type; float speed;
                if (rand <= fishProb)       { type = DrifterType.Fish; speed = _settings.FishSpeed; }   
                else if (rand <= rivalProb) { type = DrifterType.RivalSalmon; speed = _settings.RivalSpeed; } 
                else                        { type = DrifterType.Driftwood; speed = _settings.DriftwoodSpeed; }
                
                drifters.Add(new DrifterData(type, startPos, speed, lane));
                break; 
            }
        }
        return drifters;
    }
    
    // ==========================================
    // 漂流物の移動と「迂回AI」
    // ==========================================
    public void UpdateDrifter(DrifterData drifter, float deltaTime, IReadOnlyList<Vector2> globalLeft, IReadOnlyList<Vector2> globalRight, IReadOnlyList<FixedObstacleData> obstacles)
    {
        bool isBlocked = false;
        float currentSpeed = drifter.Speed;
        
        float lookAheadY = drifter.Position.y + (drifter.Speed > 0 ? 1f : -1f);
        Vector2 forwardPos = new Vector2(drifter.Position.x, lookAheadY);

        foreach (var obs in obstacles)
        {
            if (drifter.Speed > 0 && obs.Position.y < drifter.Position.y) continue;
            if (drifter.Speed < 0 && obs.Position.y > drifter.Position.y) continue;

            if (Vector2.Distance(forwardPos, obs.Position) < (obs.Radius + 0.5f))
            {
                isBlocked = true;
                float avoidDirection = obs.Position.x > drifter.Position.x ? -1f : 1f;
                drifter.TargetLane = Mathf.Clamp(drifter.CurrentLane + (avoidDirection * 0.3f), 0.1f, 0.9f);
                break;
            }
        }

        if (isBlocked)
        {
            currentSpeed *= 1f; 
        }

        if (drifter.Type == DrifterType.RivalSalmon || drifter.Type == DrifterType.Fish || isBlocked)
        {
            float laneChangeSpeed = isBlocked ? _settings.BlockedLaneChangeSpeed : (drifter.Type == DrifterType.RivalSalmon ? _settings.RivalLaneChangeSpeed : _settings.DefaultLaneChangeSpeed);
            drifter.CurrentLane = Mathf.MoveTowards(drifter.CurrentLane, drifter.TargetLane, deltaTime * laneChangeSpeed);
            
            if (!isBlocked && Mathf.Abs(drifter.CurrentLane - drifter.TargetLane) < 0.05f)
            {
                drifter.TargetLane = Random.Range(0.1f, 0.9f);
            }
        }

        float newY = drifter.Position.y + (currentSpeed * deltaTime);
        
        bool hasLeft = _math.TryGetXAtY(globalLeft, newY, out float leftX);
        bool hasRight = _math.TryGetXAtY(globalRight, newY, out float rightX);

        float newX = drifter.Position.x;
        if (hasLeft && hasRight)
        {
            newX = Mathf.Lerp(leftX, rightX, drifter.CurrentLane);
        }

        drifter.Position = new Vector2(newX, newY);
    }
}