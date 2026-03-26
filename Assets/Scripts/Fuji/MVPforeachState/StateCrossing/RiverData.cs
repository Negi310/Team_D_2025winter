[System.Serializable]
public class RiverData
{
    public string RiverName { get; set; }

    // ==========================================
    // 内部パラメータ（7項目：基礎量からランダム配分）
    // ==========================================
    public float FlowSpeed { get; set; }       // 流れの速さ
    public float Narrowness { get; set; }      // 狭さ
    public float ObstacleDensity { get; set; } // 障害物の多さ
    public float FishDensity { get; set; }     // 小魚の多さ
    public float RivalDensity { get; set; }    // ライバルの鮭の多さ
    public float AccidentDensity { get; set; } // 流木・熊の多さ
    public float Curviness { get; set; }       // カーブの多さ

    // ==========================================
    // UI表示用パラメータ（5項目：プレイヤーに見せる用）
    // ==========================================
    public float DisplayToughness => FlowSpeed + Narrowness;      // 険しさ
    public float DisplayComplexity => ObstacleDensity;            // 複雑さ
    public float DisplayRichness => FishDensity + RivalDensity;   // 豊かさ
    public float DisplayDanger => AccidentDensity;                // 危なさ
    public float DisplayMeandering => Curviness;                  // くねり
}
