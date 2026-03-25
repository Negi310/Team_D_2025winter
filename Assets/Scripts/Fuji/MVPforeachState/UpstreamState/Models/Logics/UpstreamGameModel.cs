using UnityEngine;

public class UpstreamGameModel
{
    private const float MinWidth = 2f;
    private const float MaxWidth = 10f;
    private const float ForwardRayLimit = 5f;

    // 1. Rayの計測結果からスタミナ消費量を計算
    public float CalculateRayBasedDrain(UpstreamStats stats, RayData sensor, float deltaTime)
    {
        float t = Mathf.InverseLerp(MaxWidth, MinWidth, sensor.TotalWidth);
        float drain = 1f * (1f + t * 2f); // BaseDrain

        if (sensor.ForwardRockDistance < ForwardRayLimit) drain *= 0.5f; // 岩裏（逆流）
        else if (sensor.ForwardNextRockDistance < ForwardRayLimit) drain *= 0.25f;
        else if (sensor.ForwardSalmonDistance < ForwardRayLimit) drain *= 1.5f; // 急流

        float statMultiplier = Mathf.Max(0.2f, 1.0f - (stats.Stamina * 0.02f));
        return drain * statMultiplier * deltaTime;
    }

    // 2. 移動速度の計算（川幅やステータスを考慮）
    public float CalculateHorizontalSpeed(UpstreamStats stats, RayData sensor)
    {
        // 狭いほど横に動きづらくなる
        float widthPenalty = Mathf.InverseLerp(MinWidth, MaxWidth, sensor.TotalWidth);
        return (3.0f + (stats.Speed * 0.1f)) * widthPenalty;
    }

    public float CalculateForwardSpeed(UpstreamStats stats)
    {
        return 2.0f + (stats.Speed * 0.2f);
    }

    // 3. ジャンプのスタミナ計算
    public float CalculateJumpCost(UpstreamStats stats, RayData sensor)
    {
        float statDiscount = stats.Jump * 0.5f;
        float cost = Mathf.Max(2.0f, 15.0f - statDiscount);

        // 岩の裏（逆流）なら激安コンボ！
        return (sensor.ForwardRockDistance < ForwardRayLimit) ? cost * 0.1f : cost;
    }

    // 4. ライバルとのバトル計算
    public BattleResult EvaluateRivalBattle(float myAttack)
    {
        float rivalStrength = Random.Range(5f, 15f);
        if (myAttack > rivalStrength + 5f) return BattleResult.Win;
        if (myAttack >= rivalStrength) return BattleResult.Draw;
        return BattleResult.Lose;
    }
}