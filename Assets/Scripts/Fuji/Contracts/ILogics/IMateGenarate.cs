public interface IMateGeneratable
{
    // 川登りスコアと設定を受け取り、5体の配列を返す
    SalmonData[] GenerateCandidates(float upstreamScore, MateGenerationSettingsSO settings);
}