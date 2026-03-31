using UnityEngine;

public class UpstreamGameModel
{
    private readonly GameSetting _settings;
    
    public UpstreamGameModel(GameSetting settings) => _settings = settings;

    public float CalculateForwardSpeed() => _settings.BaseForwardSpeed; 

    // 横移動速度（川幅による影響を考慮）
    public float CalculateHorizontalSpeed(UpstreamStats stats, RayData sensor, bool isInvincible)
    {
        // 川幅が狭いほど横移動しづらくなるペナルティ
        float widthFactor = Mathf.InverseLerp(_settings.RiverMinWidth, _settings.RiverMaxWidth, sensor.TotalWidth);
        float widthPenalty = Mathf.Lerp(1.0f - _settings.RiverWidthSpeedPenalty, 1.0f, widthFactor);
        
        float speed = (_settings.BaseHorizontalSpeed + (stats.Speed * _settings.HorizontalStatMultiplier)) * widthPenalty;
        return isInvincible ? speed * _settings.InvincibleSpeedMultiplier : speed;
    }

    public float CalculateDrain(UpstreamStats stats, RayData sensor, float distanceFromCenter, float deltaTime, float flowSpeed)
    {
        // 川の中心からの距離ペナルティ（中心に近いほど流れが急なので消費増）
        float centerPenalty = 1.0f + Mathf.Clamp01(1.0f - (distanceFromCenter / 3.0f)) * _settings.CenterDistanceDrainPenalty;
        float drain = _settings.BaseDrainRate * centerPenalty;

        // 前方の状況による水流変化（スリップストリームと激流）
        if (sensor.ForwardRockDistance < _settings.ForwardRayLimit) 
            drain *= _settings.RockSlipstreamDrainMultiplier;       // 岩の裏（安全地帯）
        else if (sensor.ForwardNextRockDistance < _settings.ForwardRayLimit) 
            drain *= _settings.RockSideTorrentDrainMultiplier;      // 岩の横の押し出し（激流）
        else if (sensor.ForwardSalmonDistance < _settings.ForwardRayLimit) 
            drain *= _settings.RivalSlipstreamDrainMultiplier;      // ライバルの裏（スリップストリーム）

        // 川自体の流速による影響
        float flowMultiplier = Mathf.Max(0.5f, flowSpeed / _settings.FlowSpeedNorm);
        
        // スピードステータスによる「水流を受け流す」スタミナ消費抑制
        float statMultiplier = Mathf.Max(0.2f, 1.0f - (stats.Speed * _settings.SpeedToDrainReductionRate));
        
        return drain * statMultiplier * flowMultiplier * deltaTime;
    }

    public float CalculateJumpCost(UpstreamStats stats, RayData sensor, float distanceFromCenter)
    {
        float statDiscount = stats.Jump * _settings.JumpStatDiscountMultiplier;
        float baseCost = Mathf.Max(_settings.MinJumpCost, _settings.BaseJumpCost - statDiscount);

        // 川の中心から遠い（流れに乗れない）ほどジャンプ消費増
        float jumpPenalty = 1.0f + Mathf.Clamp01(distanceFromCenter / 3.0f) * _settings.CenterDistanceJumpPenalty;
        float cost = baseCost * jumpPenalty;

        // 岩の裏から飛ぶ時は恩恵で安く飛べる
        return (sensor.ForwardRockDistance < _settings.ForwardRayLimit) ? cost * _settings.RockSlipstreamJumpMultiplier : cost;
    }

    public BattleResult EvaluateRivalBattle(float myAttack)
    {
        float rivalStrength = Random.Range(_settings.RivalMinStrength, _settings.RivalMaxStrength);
        if (myAttack > rivalStrength + 5f) return BattleResult.Win;
        if (myAttack >= rivalStrength) return BattleResult.Draw;
        return BattleResult.Lose;
    }
}