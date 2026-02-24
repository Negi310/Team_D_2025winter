/// <summary>
/// 遺伝と次世代のパラメータ算出に専念するインターフェース
/// 実行タイミング: 求愛判定が true (成功) になった直後
/// </summary>
public interface IBreedingCalculator
{
    // 両親のデータを受け取り、新しい個体を返す
    SalmonData GenerateChild(SalmonData playerSalmon, SalmonData mateSalmon, float BreedRate);
}