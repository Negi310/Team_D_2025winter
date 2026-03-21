/// <summary>
/// 求愛の成功率計算と成否判定に専念するインターフェース
/// 実行タイミング: プレイヤーがUIで相手を選択し、「求愛する」ボタンを押した瞬間
/// </summary>
public interface ICourtshipEvaluatable
{
    // 成功率を算出する（UI表示用にも使えるように分けると便利）
    float CalculateSuccessRate(SalmonData playerSalmon, SalmonData targetMate);

    // 算出した成功率と乱数を用いて、最終的な成否を判定する
    bool EvaluateCourtship(SalmonData playerSalmon, SalmonData targetMate);
}