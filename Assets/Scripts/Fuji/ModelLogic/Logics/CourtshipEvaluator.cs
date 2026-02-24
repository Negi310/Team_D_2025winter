public class CourtshipEvaluator : ICourtshipEvaluatable
{
    public float CalculateSuccessRate(SalmonData playerSalmon, SalmonData targetMate)
    {
        // 相手の強さレベルや、求愛行動特性（色・形など）の相性を計算するロジック
        return 0.0f;
    }

    public bool EvaluateCourtship(SalmonData playerSalmon, SalmonData targetMate)
    {
        float rate = CalculateSuccessRate(playerSalmon, targetMate);
        // ここで乱数とrateを比較して true/false を返す
        return true; 
    }
}