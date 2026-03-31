using UnityEngine;

public class CourtshipEvaluator : ICourtshipEvaluatable
{
    private readonly GameSetting _settings;

    public CourtshipEvaluator(GameSetting settings)
    {
        _settings = settings;
    }

    public float CalculateSuccessRate(SalmonData playerSalmon, SalmonData targetMate)
    {
        var pTraits = playerSalmon.CourtshipTraits;
        var tTraits = targetMate.CourtshipTraits;

        float sizeDiff = Mathf.Abs((int)pTraits.Size - (int)tTraits.Size);
        float sizeScore = 1.0f - (sizeDiff / 2.0f); 
        
        int colorCount = System.Enum.GetValues(typeof(SalmonColor)).Length;
        int colorDiff = Mathf.Abs((int)pTraits.Color - (int)tTraits.Color);
        if (colorDiff > colorCount / 2) colorDiff = colorCount - colorDiff; 
        
        float colorScore = 1.0f - ((float)colorDiff / (colorCount / 2f)); 

        float shapeBonus = (pTraits.Hair == tTraits.Hair) ? _settings.CourtshipShapeBonus : 0f; 

        float rate = _settings.CourtshipBaseRate + (sizeScore * _settings.CourtshipSizeWeight) + (colorScore * _settings.CourtshipColorWeight) + shapeBonus;
        return Mathf.Clamp(rate, _settings.CourtshipMinRate, _settings.CourtshipMaxRate);
    }

    public bool EvaluateCourtship(SalmonData playerSalmon, SalmonData targetMate)
    {
        return Random.value <= CalculateSuccessRate(playerSalmon, targetMate);
    }
}