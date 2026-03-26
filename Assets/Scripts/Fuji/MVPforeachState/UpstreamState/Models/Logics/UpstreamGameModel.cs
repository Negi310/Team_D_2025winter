using UnityEngine;

public class UpstreamGameModel
{
    private const float MinWidth = 2f;
    private const float MaxWidth = 10f;
    private const float ForwardRayLimit = 5f;

    public float CalculateForwardSpeed() => 3.0f; // 前方速度は一定

    // 横移動速度（無敵時間中は1.5倍）
    public float CalculateHorizontalSpeed(UpstreamStats stats, RayData sensor, bool isInvincible)
    {
        float widthPenalty = Mathf.InverseLerp(MinWidth, MaxWidth, sensor.TotalWidth);
        float speed = (3.0f + (stats.Speed * 0.1f)) * widthPenalty;
        return isInvincible ? speed * 1.5f : speed;
    }

    // ★修正: スタミナ消費（中心に近いほど流れが急＝消費大）
    public float CalculateDrain(UpstreamStats stats, RayData sensor, float distanceFromCenter, float deltaTime)
    {
        // 中心(0m)なら1.5倍消費、端(3m以上)なら1.0倍消費に落ち着く
        float centerPenalty = 1.0f + Mathf.Clamp01(1.0f - (distanceFromCenter / 3.0f)) * 0.5f;
        float drain = 1.0f * centerPenalty;

        if (sensor.ForwardRockDistance < ForwardRayLimit) drain *= 0.5f; 
        else if (sensor.ForwardNextRockDistance < ForwardRayLimit) drain *= 0.75f;
        else if (sensor.ForwardSalmonDistance < ForwardRayLimit) drain *= 1.5f;

        float statMultiplier = Mathf.Max(0.2f, 1.0f - (stats.Stamina * 0.02f));
        return drain * statMultiplier * deltaTime;
    }

    // ★修正: ジャンプコスト（中心に近いほど流れに乗りやすく＝消費小）
    public float CalculateJumpCost(UpstreamStats stats, RayData sensor, float distanceFromCenter)
    {
        float statDiscount = stats.Jump * 0.5f;
        float baseCost = Mathf.Max(2.0f, 15.0f - statDiscount);

        // 中心(0m)なら基本コストそのまま、端(3m以上)ならジャンプコスト1.5倍
        float jumpPenalty = 1.0f + Mathf.Clamp01(distanceFromCenter / 3.0f) * 0.5f;
        float cost = baseCost * jumpPenalty;

        return (sensor.ForwardRockDistance < ForwardRayLimit) ? cost * 0.1f : cost;
    }

    public BattleResult EvaluateRivalBattle(float myAttack)
    {
        float rivalStrength = Random.Range(5f, 15f);
        if (myAttack > rivalStrength + 5f) return BattleResult.Win;
        if (myAttack >= rivalStrength) return BattleResult.Draw;
        return BattleResult.Lose;
    }
}