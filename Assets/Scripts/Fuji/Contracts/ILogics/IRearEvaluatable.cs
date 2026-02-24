using System.Collections.Generic;

public interface IRearEvaluatable
{
    // 川のデータ特徴から紐づけられた名前を生成する
    string GetRiverName(RiverData data);
    
    // イベントの実行結果（予測）を計算する
    //SalmonData GetTrainingResult(SalmonData currentSalmon, EventData selectedEvent);
    
    float GetUpstreamResult(SalmonData currentSalmon, RiverData data);
}