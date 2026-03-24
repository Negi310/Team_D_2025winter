using System.Collections.Generic;
using UnityEngine;

public class ObstacleModel
{
    private readonly SplineMathModel _math;
    private const float VisualMargin = 0f;

    public ObstacleModel(SplineMathModel math) => _math = math;

    // ==========================================
    // 固定設置物の生成
    // ==========================================
    public List<FixedObstacleData> GenerateFixedObstacles(RuntimeChunkData chunk, IReadOnlyList<Vector2> leftBank, IReadOnlyList<Vector2> rightBank, int count, List<FixedObstacleData> existing)
    {
        var results = new List<FixedObstacleData>();
        float startY = chunk.Position, endY = chunk.Position + chunk.Preset.ChunkHeight;

        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                FixedObstacleType type = Random.value > 0.5f ? FixedObstacleType.Rock : FixedObstacleType.FallenTree;
                float physicalRadius = type == FixedObstacleType.Rock ? 1f : 2.0f;
                float requiredSpace = physicalRadius + VisualMargin;

                float snappedY = Mathf.Floor(Random.Range(startY, endY)) + 0.5f;

                int dummy = 0;
                // ★修正: スプラインデータが足りず取得に失敗した場合はスキップする
                if (!_math.TryGetXAtY(leftBank, snappedY, ref dummy, out float leftX) ||
                    !_math.TryGetXAtY(rightBank, snappedY, ref dummy, out float rightX))
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
    public List<DrifterData> GenerateDrifters(RuntimeChunkData chunk, IReadOnlyList<Vector2> leftBank, IReadOnlyList<Vector2> rightBank, int count)
    {
        var drifters = new List<DrifterData>();
        float startY = chunk.Position + 0.5f * chunk.Preset.ChunkHeight, endY = chunk.Position + 1.5f * chunk.Preset.ChunkHeight;

        for (int i = 0; i < count; i++)
        {
            // 試行回数を設けて、被らない（安全な）配置場所を探す
            for (int attempt = 0; attempt < 10; attempt++)
            {
                // ★ 岩と同様に、Y座標を整数+0.5にスナップ
                float snappedY = Mathf.Floor(Random.Range(startY, endY)) + 0.5f;
                
                int dummy = 0;
                if (!_math.TryGetXAtY(leftBank, snappedY, ref dummy, out float leftX) ||
                    !_math.TryGetXAtY(rightBank, snappedY, ref dummy, out float rightX))
                {
                    continue;
                }
                
                float safeLeft = leftX + 0.5f; // 漂流物の半径を0.5と仮定
                float safeRight = rightX - 0.5f;
                if (safeLeft >= safeRight) continue;

                // ★ X座標も岩と同様に整数+0.5にスナップ
                float randomX = Random.Range(safeLeft, safeRight);
                float snappedX = Mathf.Floor(randomX) + 0.5f;
                if (snappedX < safeLeft || snappedX > safeRight) continue;

                // ★ スナップさせたX座標から、「川幅におけるレーン割合」を逆算する！
                float lane = Mathf.InverseLerp(leftX, rightX, snappedX);
                Vector2 startPos = new Vector2(snappedX, snappedY);
                
                float rand = Random.value;
                DrifterType type; float speed;
                if (rand < 0.2f)      { type = DrifterType.RivalSalmon; speed = 5.0f; } 
                else if (rand < 0.6f) { type = DrifterType.Fish; speed = 2.0f; }   
                else                  { type = DrifterType.Driftwood; speed = -4.0f; }
                
                drifters.Add(new DrifterData(type, startPos, speed, lane));
                break; // 成功したらループを抜ける
            }
        }
        return drifters;
    }
    
    // ==========================================
    // 漂流物の移動と「迂回AI」
    // ==========================================
    // ★ 引数に obstacles (固定設置物のリスト) を追加
    public void UpdateDrifter(DrifterData drifter, float deltaTime, IReadOnlyList<Vector2> globalLeft, IReadOnlyList<Vector2> globalRight, IReadOnlyList<FixedObstacleData> obstacles)
    {
        bool isBlocked = false;
        float currentSpeed = drifter.Speed;
        
        // ① 進む方向の1.5m先の座標を予測
        float lookAheadY = drifter.Position.y + (drifter.Speed > 0 ? 1f : -1f);
        Vector2 forwardPos = new Vector2(drifter.Position.x, lookAheadY);

        // ② 前方に障害物がないかチェック
        foreach (var obs in obstacles)
        {
            // 自分の進行方向にない背後の障害物は無視
            if (drifter.Speed > 0 && obs.Position.y < drifter.Position.y) continue;
            if (drifter.Speed < 0 && obs.Position.y > drifter.Position.y) continue;

            // 前方に障害物が被っているか（0.5fは漂流物の想定半径）
            if (Vector2.Distance(forwardPos, obs.Position) < (obs.Radius + 0.5f))
            {
                isBlocked = true;
                
                // 障害物が自分の右にあるなら左（-1）へ、左にあるなら右（1）へ避ける
                float avoidDirection = obs.Position.x > drifter.Position.x ? -1f : 1f;
                
                // TargetLane（横の目標）を、避ける方向へ強制的に上書きする（0.1〜0.9の範囲に収める）
                drifter.TargetLane = Mathf.Clamp(drifter.CurrentLane + (avoidDirection * 0.3f), 0.1f, 0.9f);
                break;
            }
        }

        // ③ 障害物で塞がれている間は、縦の移動速度を10%に落とし、横へ逃げる時間を稼ぐ
        if (isBlocked)
        {
            currentSpeed *= 1f; 
        }

        // ④ 横方向（レーン割合）の更新
        // 通常動かない流木（Driftwood）も、目の前に岩があれば緊急回避（isBlocked）で動く
        if (drifter.Type == DrifterType.RivalSalmon || drifter.Type == DrifterType.Fish || isBlocked)
        {
            // 避けている最中は機敏に（0.8f）動く
            float laneChangeSpeed = isBlocked ? 1.6f : (drifter.Type == DrifterType.RivalSalmon ? 0.5f : 0.2f);
            
            drifter.CurrentLane = Mathf.MoveTowards(drifter.CurrentLane, drifter.TargetLane, deltaTime * laneChangeSpeed);
            
            // 障害物がない平常時のみ、ランダムな目標を設定し直す
            if (!isBlocked && Mathf.Abs(drifter.CurrentLane - drifter.TargetLane) < 0.05f)
            {
                drifter.TargetLane = Random.Range(0.1f, 0.9f);
            }
        }

        // ⑤ 最終座標の適用
        float newY = drifter.Position.y + (currentSpeed * deltaTime);

        int dummy = 0;
        bool hasLeft = _math.TryGetXAtY(globalLeft, newY, ref dummy, out float leftX);
        bool hasRight = _math.TryGetXAtY(globalRight, newY, ref dummy, out float rightX);

        float newX = drifter.Position.x;
        if (hasLeft && hasRight)
        {
            newX = Mathf.Lerp(leftX, rightX, drifter.CurrentLane);
        }

        drifter.Position = new Vector2(newX, newY);
    }
}