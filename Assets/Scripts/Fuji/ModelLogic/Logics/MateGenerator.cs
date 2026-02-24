public class MateGenerator : IMateGeneratable
{
    public SalmonData[] GenerateCandidates(float upstreamScore, MateGenerationSettingsSO settings)
    {
        // ここに、スコアを基礎量として5段階の強さに割り振るロジックをカプセル化する
        // 係数のランダム幅などの計算もここで行う
        return new SalmonData[5]; 
    }
}