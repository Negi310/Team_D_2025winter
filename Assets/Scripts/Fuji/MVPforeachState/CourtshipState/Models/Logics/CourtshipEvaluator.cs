using UnityEngine;

public class CourtshipEvaluator : ICourtshipEvaluatable
{
    public float CalculateSuccessRate(SalmonData playerSalmon, SalmonData targetMate)
    {
        float rate = 0.5f;
        
        // 相手の強さレベルや、求愛行動特性（色・形など）の相性を計算するロジック
        return Mathf.Clamp(rate, 0.05f, 0.95f);;
    }

    public bool EvaluateCourtship(SalmonData playerSalmon, SalmonData targetMate)
    {
        float rate = CalculateSuccessRate(playerSalmon, targetMate);
        // ここで乱数とrateを比較して true/false を返す
        return Random.value <= rate;
    }
}