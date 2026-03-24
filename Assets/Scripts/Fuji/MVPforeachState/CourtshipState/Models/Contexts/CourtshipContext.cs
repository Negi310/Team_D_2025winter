// 【Local Model】求愛フェーズ中のみ存在するコンテキスト
public class CourtshipContext
{
    // 場に出た5体の候補
    public SalmonData[] Candidates { get; set; }

    // 現在UIで選択されている相手（まだ決定していない）
    public SalmonData SelectedMate { get; set; }
    
    // 現在選択されている相手との求愛成功率
    public float CurrentSuccessRate { get; set; }
    
    public float LastRunScore { get; set; }
}