using UnityEngine;

public class MateGeneratable : IMateGeneratable
{
    public SalmonData[] GenerateCandidates(float upstreamScore, MateGenerationSettingsSO settings)
    {
    
    
        SalmonData[] candidates = new SalmonData[5];

        MateGenerationSettingsSO.TierSetting[] tiers =
        {
            settings.veryWeak,   // くそ雑魚
            settings.weak,       // 雑魚
            settings.compromise, // 弱い
            settings.normal,     // 強い
            settings.strong      // くそ強い
        };
        float normalizedScore = Mathf.Clamp01(upstreamScore / 100f);

        for (int i = 0; i < tiers.Length; i++)
        {
            candidates[i] = CreateSalmonFromTier(tiers[i], normalizedScore);
        }

        return candidates;
    }

    private SalmonData CreateSalmonFromTier(MateGenerationSettingsSO.TierSetting tier, float normalizedScore)
    {
        float tierMultiplier = Random.Range(tier.minMultiplier, tier.maxMultiplier);
        float scoreBonus = Mathf.Lerp(0.95f, 1.1f, normalizedScore);
        float finalMultiplier = tierMultiplier * scoreBonus;
        float finalDifficulty = tier.baseCourtshipDifficulty * finalMultiplier;

        UpstreamStats uStats = new UpstreamStats(
            Random.Range(1f, 5f) * finalMultiplier,
            Random.Range(1f, 5f) * finalMultiplier,
            Random.Range(1f, 5f) * finalMultiplier,
            Random.Range(1f, 5f) * finalMultiplier,
            Random.Range(1f, 5f) * finalMultiplier
        );

        CourtshipTraits cTraits = new CourtshipTraits(
            (SalmonSize)Random.Range(0, 3),         // Size
            (SalmonColor)Random.Range(0, 9),        // Color
            (SalmonHair)Random.Range(0, 3),         // Hair
            Random.Range(0, 3),                     // Eye 
            Random.Range(0, 4),                     // Eyebrow
            Random.Range(0, 3)                      // Mouth
        );

        return new SalmonData(uStats, cTraits);
    }
}
