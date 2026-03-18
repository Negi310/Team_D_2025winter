[System.Serializable]
public class SalmonData
{
    public string Name; // 個体識別用
    
    // パラメータ群
    public UpstreamStats UpstreamStats;
    public CourtshipTraits CourtshipTraits;

    // コンストラクタで初期化を強制し、不正な状態の鮭が生まれるのを防ぐ
    public SalmonData(UpstreamStats uStats, CourtshipTraits cTraits) 
    {
        this.UpstreamStats = uStats;
        this.CourtshipTraits = cTraits;
    }
}
