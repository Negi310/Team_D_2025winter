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

        // ★バグ修正: normalizedScore を渡すのをやめ、生の upstreamScore を渡すように戻しました！
        // （そうしないと距離による青天井のステータスインフレが起きません）
        for (int i = 0; i < tiers.Length; i++)
        {
            candidates[i] = CreateSalmonFromTier(tiers[i], upstreamScore);
        }

        return candidates;
    }

    private SalmonData CreateSalmonFromTier(MateGenerationSettingsSO.TierSetting tier, float upstreamScore)
    {
        float tierMultiplier = Random.Range(tier.minMultiplier, tier.maxMultiplier);
        float distanceBonusPoints = upstreamScore * 2f;
        float totalPoints = (50f * tierMultiplier) + distanceBonusPoints;

        // ==========================================
        // ★修正: 「特化型」を生み出すための尖った重み付け
        // ==========================================
        float[] weights = new float[5];
        float totalWeight = 0f;
        
        float typeRand = Random.value;

        if (typeRand < 0.2f)
        {
            // 【20%の確率】平凡型（バランス良く配分される器用貧乏）
            for (int i = 0; i < 5; i++) weights[i] = Random.Range(0.8f, 1.2f);
        }
        else if (typeRand < 0.6f)
        {
            // 【40%の確率】1点特化型（1つのステータスが異常に高い）
            int specialIndex = Random.Range(0, 5); // どのステータスを特化させるか
            for (int i = 0; i < 5; i++) 
            {
                // 特化項目には「5〜10倍」の重みを与え、それ以外は低く抑える
                weights[i] = (i == specialIndex) ? Random.Range(2.0f, 3.0f) : Random.Range(0.2f, 1.0f);
            }
        }
        else
        {
            // 【40%の確率】2点特化型（2つのステータスが高い）
            int special1 = Random.Range(0, 5);
            int special2 = Random.Range(0, 5);
            while (special1 == special2) special2 = Random.Range(0, 5); // かぶり防止

            for (int i = 0; i < 5; i++) 
            {
                // 2つの特化項目に「3〜6倍」の重みを与える
                weights[i] = (i == special1 || i == special2) ? Random.Range(1.0f, 2.5f) : Random.Range(0.2f, 1.0f);
            }
        }

        // 合計重みを計算
        for (int i = 0; i < 5; i++) totalWeight += weights[i];

        // 総ポイントを割合に応じて配分
        UpstreamStats uStats = new UpstreamStats(
            Mathf.Max(1f, totalPoints * (weights[0] / totalWeight)), // Speed
            Mathf.Max(1f, totalPoints * (weights[1] / totalWeight)), // Jump
            Mathf.Max(1f, totalPoints * (weights[2] / totalWeight)), // Stamina
            Mathf.Max(1f, totalPoints * (weights[3] / totalWeight)), // Attack
            Mathf.Max(1f, totalPoints * (weights[4] / totalWeight))  // Intelligence
        );

        CourtshipTraits cTraits = new CourtshipTraits(
            (SalmonSize)Random.Range(0, 3), (SalmonColor)Random.Range(0, 9), (SalmonHair)Random.Range(0, 3),
            Random.Range(0, 3), Random.Range(0, 4), Random.Range(0, 3)
        );

        return new SalmonData(uStats, cTraits);
    }
}