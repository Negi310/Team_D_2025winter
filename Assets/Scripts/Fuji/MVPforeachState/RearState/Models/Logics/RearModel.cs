using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RearModel
{
    private readonly TargetStatType[] _allStats = { TargetStatType.Power, TargetStatType.Jump, TargetStatType.Cautiousness, TargetStatType.Stamina };

    public EventData[] GenerateAllEvents(EventPool pool)
    {
        // 最終的に返す8つのイベントを格納する配列
        var choices = new EventData[8];

        // ==========================================
        // ① 選択式イベント（パラメータ特化）を 5つ 生成
        // ==========================================
        for (int i = 0; i < 5; i++)
        {
            // プールからベースとなるイベントをランダムに選ぶ（あるいは順番に出す等も可能）
            var sourceSO = pool.ParameterEvents[Random.Range(0, pool.ParameterEvents.Count)];

            // 基礎パラメータ変動はマスターデータ通り（固定）
            var baseMod = new StatModifier
                { TargetStatName = sourceSO.MainStat.ToString(), Value = sourceSO.BaseGainAmount };

            // ★ 仕様：選択式イベントのランダム要素は「Bonus」のみ
            TargetStatType randomTarget = _allStats[Random.Range(0, _allStats.Length)];
            int randomBonusValue = Random.Range(sourceSO.RandomBonusRange.x, sourceSO.RandomBonusRange.y + 1);
            var bonusMod = new StatModifier { TargetStatName = randomTarget.ToString(), Value = randomBonusValue };

            choices[i] = new EventData(sourceSO.EventTitle, baseMod, bonusMod, sourceSO.LinkedConversation);
        }

        // ==========================================
        // ② ランダムイベントを 3つ 生成（Poolから選出）
        // ==========================================
        // ※ ランダムイベントが重複しても良いなら単純な Random.Range。
        // ※ 重複させない（被りなし）ならシャッフルして上から3つ取るのが定石です。

        // ここでは「重複なし」で3つ選ぶ安全な設計にします
        var shuffledRandomEvents = pool.RandomEvents.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < 3; i++)
        {
            // プール内のランダムイベントが3つ未満だった場合のエラー防止
            var sourceSO = shuffledRandomEvents[i % shuffledRandomEvents.Count];

            // ランダムイベントは「マスターデータに設定されたまま」の値を使う（Bonus等はなし）
            var baseMod = new StatModifier
                { TargetStatName = sourceSO.MainStat.ToString(), Value = sourceSO.BaseGainAmount };
            var emptyBonusMod = new StatModifier { TargetStatName = "None", Value = 0 };

            // 5番目〜7番目のインデックスに格納 (i + 5)
            choices[i + 5] =
                new EventData(sourceSO.EventTitle, baseMod, emptyBonusMod, sourceSO.LinkedConversation);
        }
        return choices.OrderBy(x => Random.value).ToArray();
    }

    public SalmonData ApplyEventResult(SalmonData currentSalmon, EventData selectedEvent)
    {
        // 現在のステータスをコピー
        var newStats = currentSalmon.UpstreamStats;

        // BaseとBonusの両方を適用するローカル関数
        void ApplyMod(StatModifier mod)
        {
            switch (mod.TargetStatName)
            {
                case "Power": newStats.Power += mod.Value; break;
                case "Jump": newStats.Jump += mod.Value; break;
                case "Cautiousness": newStats.Cautiousness += mod.Value; break;
                case "Stamina": newStats.Stamina += mod.Value; break;
            }
        }

        ApplyMod(selectedEvent.BaseModifier);
        ApplyMod(selectedEvent.BonusModifier);

        // 反映後の新しい鮭を返す
        return new SalmonData(newStats, currentSalmon.CourtshipTraits) { Name = currentSalmon.Name };
    }
    
    // どのパラメータが合計でいくつ上がるか（Base + Bonus）を計算する辞書
    public StatModifier AddGain(StatModifier mod)
    {
        var predictedGains = new Dictionary<string, int>();
        
        if (predictedGains.ContainsKey(mod.TargetStatName))
            predictedGains[mod.TargetStatName] += mod.Value;
        else
            predictedGains.Add(mod.TargetStatName, mod.Value);
        return mod;
    }
}