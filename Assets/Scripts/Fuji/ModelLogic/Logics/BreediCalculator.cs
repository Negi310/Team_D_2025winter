public class BreedingCalculator : IBreedingCalculator
{
    public SalmonData GenerateChild(SalmonData playerSalmon, SalmonData mateSalmon)
    {
        // 両親のパラメータをベースに乱数を加味し、新しい能力値を持った子供を生成するロジック
        // 世代数(generation)のインクリメントもここで行う
        return new SalmonData(new UpstreamStats(), new CourtshipTraits());
    }
}