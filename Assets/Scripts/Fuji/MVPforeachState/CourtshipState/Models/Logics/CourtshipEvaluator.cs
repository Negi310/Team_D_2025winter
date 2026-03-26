using UnityEngine;

public class CourtshipEvaluator : ICourtshipEvaluatable
{
    public float CalculateSuccessRate(SalmonData playerSalmon, SalmonData targetMate)
    {
        // ① お互いのステータスの「差」から基本相性を計算（差が小さいほど高得点）
        float sizeScore = 1.0f - Mathf.Clamp01(Mathf.Abs(playerSalmon.CourtshipTraits.Size - targetMate.CourtshipTraits.Size) / 10f);
        float colorScore = 1.0f - Mathf.Clamp01(Mathf.Abs(playerSalmon.CourtshipTraits.ColorValue - targetMate.CourtshipTraits.ColorValue) / 10f);
        
        // ② プレイヤー自身の魅力（形）によるボーナス
        float shapeBonus = playerSalmon.CourtshipTraits.ShapeValue / 10f; 

        // 基礎成功率30% + 相性による増減
        float rate = 0.3f + (sizeScore * 0.2f) + (colorScore * 0.2f) + (shapeBonus * 0.3f);
        
        // どんなに相性が良くても悪くても、5%〜95%の間に収める
        return Mathf.Clamp(rate, 0.05f, 0.95f);
    }

    public bool EvaluateCourtship(SalmonData playerSalmon, SalmonData targetMate)
    {
        float rate = CalculateSuccessRate(playerSalmon, targetMate);
        // ここで乱数とrateを比較して true/false を返す
        return Random.value <= rate;
    }
}