using System.Collections.Generic;
using UnityEngine;

public class RiverDirector
{
    private readonly GameSetting _settings;

    public RiverDirector(GameSetting settings)
    {
        _settings = settings;
    }
    
    public ChunkPreset GetNextChunkPreset(IReadOnlyList<ChunkPreset> availablePresets, RiverData currentRiver, ChunkConnector requiredEntry)
    {
        if (availablePresets.Count == 0) return null;

        var validPresets = new List<ChunkPreset>();
        foreach (var p in availablePresets)
        {
            if (p.EntryType == requiredEntry)
            {
                validPresets.Add(p);
            }
        }

        // 万が一、繋がるチャンクが1つも存在しない場合のフェイルセーフ（エラー落ち防止）
        if (validPresets.Count == 0)
        {
            Debug.LogWarning($"[RiverDirector] 入口 '{requiredEntry}' に対応するチャンクが見つかりません！強制的に全チャンクから選びます。");
            validPresets.AddRange(availablePresets);
        }

        // 2. 川のパラメータを 0.0 ~ 1.0 の割合に正規化（※上限は仮に10fとしています。ゲームバランスに合わせて調整してください）
        float normalizedRiverCurve = Mathf.Clamp01(currentRiver.Curviness / _settings.RiverNormMax);
        float normalizedRiverNarrow = Mathf.Clamp01(currentRiver.Narrowness / _settings.RiverNormMax);

        // 3. 各チャンクの「選ばれやすさ（Weight）」を計算する
        float[] weights = new float[validPresets.Count];
        float totalWeight = 0f;

        for (int i = 0; i < validPresets.Count; i++)
        {
            var preset = validPresets[i];

            // チャンクの特性と川の要求の「差」を計算（0.0 が完全一致、1.0 が真逆）
            float curveDiff = Mathf.Abs(normalizedRiverCurve - preset.CurvinessScore);
            float narrowDiff = Mathf.Abs(normalizedRiverNarrow - preset.NarrownessScore);
            
            // 差が小さいほど「相性が良い（＝重みを大きくする）」ための計算
            // 基礎点1.0 から減点していく。最低でも0.1の確率は残す（完全排除はしない）
            float matchScore = 1.0f - ((curveDiff + narrowDiff) / 2f);
            float weight = Mathf.Max(0.1f, matchScore); 

            // もし完全一致レベル(差がほとんどない)なら、確率にボーナス（2倍など）をかけて圧倒的に出やすくする
            if (matchScore > _settings.MatchBonusThreshold) weight *= _settings.MatchBonusMultiplier;

            weights[i] = weight;
            totalWeight += weight;
        }

        // 4. 重みに基づいたルーレット抽選（Weighted Random Selection）
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeightSum = 0f;

        for (int i = 0; i < validPresets.Count; i++)
        {
            currentWeightSum += weights[i];
            if (randomValue <= currentWeightSum)
            {
                return validPresets[i]; // ルーレットに当たったチャンクを返す！
            }
        }

        // （※計算誤差などでループを抜けたら、保険として最初の要素を返す）
        return validPresets[0];
    }
}