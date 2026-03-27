using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RearModel
{
    // 指定された5つのステータスの順番
    private readonly string[] _statOrder = { "Speed", "Jump", "Stamina", "Attack", "Intelligence" };

    public EventData[] GenerateAllEvents(EventPool pool)
    {
        var choices = new EventData[8];

        // ==========================================
        // ① 選択式イベント（ボタン0〜4の固定順で5つ生成）
        // ==========================================
        for (int i = 0; i < 5; i++)
        {
            string targetStat = _statOrder[i];
            
            // プールから該当ステータスのイベントを探す（無ければ適当なものを代用）
            var sourceSO = pool.ParameterEvents.FirstOrDefault(e => e.MainStat.ToString() == targetStat);
            if (sourceSO == null) sourceSO = pool.ParameterEvents[0];

            var baseMod = new StatModifier { TargetStatName = targetStat, Value = sourceSO.BaseGainAmount };

            // ボーナスはランダムなパラメータに付与
            string randomTarget = _statOrder[Random.Range(0, _statOrder.Length)];
            int randomBonusValue = Random.Range(sourceSO.RandomBonusRange.x, sourceSO.RandomBonusRange.y + 1);
            var bonusMod = new StatModifier { TargetStatName = randomTarget, Value = randomBonusValue };

            choices[i] = new EventData(sourceSO.EventTitle, baseMod, bonusMod, sourceSO.LinkedConversation);
        }

        // ==========================================
        // ② ランダムイベントを 3つ 生成（被りなし）
        // ==========================================
        var shuffledRandomEvents = pool.RandomEvents.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < 3; i++)
        {
            var sourceSO = shuffledRandomEvents[i % shuffledRandomEvents.Count];
            var baseMod = new StatModifier { TargetStatName = sourceSO.MainStat.ToString(), Value = sourceSO.BaseGainAmount };
            var emptyBonusMod = new StatModifier { TargetStatName = "None", Value = 0 };

            // 5番目〜7番目のインデックスに格納
            choices[i + 5] = new EventData(sourceSO.EventTitle, baseMod, emptyBonusMod, sourceSO.LinkedConversation);
        }
        
        // ★ シャッフルせずにそのまま返す（ボタンとの紐付けを固定するため）
        return choices;
    }

    public SalmonData ApplyEventResult(SalmonData currentSalmon, EventData selectedEvent)
    {
        var newStats = currentSalmon.UpstreamStats;

        void ApplyMod(StatModifier mod)
        {
            // ★修正: Mathf.Max を使って、計算結果が1未満にならないようにブロック！
            switch (mod.TargetStatName)
            {
                case "Speed": newStats.Speed = Mathf.Max(1f, newStats.Speed + mod.Value); break;
                case "Jump": newStats.Jump = Mathf.Max(1f, newStats.Jump + mod.Value); break;
                case "Stamina": newStats.Stamina = Mathf.Max(1f, newStats.Stamina + mod.Value); break;
                case "Attack": newStats.Attack = Mathf.Max(1f, newStats.Attack + mod.Value); break;
                case "Intelligence": newStats.Intelligence = Mathf.Max(1f, newStats.Intelligence + mod.Value); break;
            }
        }

        ApplyMod(selectedEvent.BaseModifier);
        ApplyMod(selectedEvent.BonusModifier);

        return new SalmonData(newStats, currentSalmon.CourtshipTraits) { Name = currentSalmon.Name };
    }
}