using UnityEngine;

public class CourtshipEvaluator : ICourtshipEvaluatable
{
    public float CalculateSuccessRate(SalmonData playerSalmon, SalmonData targetMate)
    {
        var pTraits = playerSalmon.CourtshipTraits;
        var tTraits = targetMate.CourtshipTraits;

        // ★修正: Size を数値化して差を計算（差がない=1.0、BigとSmallの真逆=0.0）
        float sizeDiff = Mathf.Abs((int)pTraits.Size - (int)tTraits.Size);
        float sizeScore = 1.0f - (sizeDiff / 2.0f); 
        
        // 色の近さを計算
        int colorCount = System.Enum.GetValues(typeof(SalmonColor)).Length;
        int colorDiff = Mathf.Abs((int)pTraits.Color - (int)tTraits.Color);
        if (colorDiff > colorCount / 2) colorDiff = colorCount - colorDiff; 
        
        float colorScore = 1.0f - ((float)colorDiff / (colorCount / 2f)); 

        float shapeBonus = (pTraits.Hair == tTraits.Hair) ? 0.2f : 0f; 

        float rate = 0.3f + (sizeScore * 0.2f) + (colorScore * 0.3f) + shapeBonus;
        return Mathf.Clamp(rate, 0.05f, 0.95f);
    }

    public bool EvaluateCourtship(SalmonData playerSalmon, SalmonData targetMate)
    {
        float rate = CalculateSuccessRate(playerSalmon, targetMate);
        // ここで乱数とrateを比較して true/false を返す
        return Random.value <= rate;
    }
}