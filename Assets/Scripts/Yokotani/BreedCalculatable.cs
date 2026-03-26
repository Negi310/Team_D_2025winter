using UnityEngine;

public class BreedCalculatable : IBreedingCalculator
{
    public SalmonData GenerateChild(SalmonData playerSalmon,SalmonData mateSalmon)
    {
        //川の遡上に関する能力値の継承
        UpstreamStats childUpstream = new UpstreamStats
        {
            Speed = Inherit(playerSalmon.UpstreamStats.Speed,mateSalmon.UpstreamStats.Speed),

            Jump = Inherit(playerSalmon.UpstreamStats.Jump,mateSalmon.UpstreamStats.Jump),

            Stamina = Inherit(playerSalmon.UpstreamStats.Stamina,mateSalmon.UpstreamStats.Stamina),
            
            Attack = Inherit(playerSalmon.UpstreamStats.Attack,mateSalmon.UpstreamStats.Attack),
            
            Intelligence = Inherit(playerSalmon.UpstreamStats.Intelligence,mateSalmon.UpstreamStats.Intelligence)
        };

        //求愛に関する能力値の継承
        CourtshipTraits childCourtship = new CourtshipTraits
        {
            Size = Inherit(playerSalmon.CourtshipTraits.Size,mateSalmon.CourtshipTraits.Size),

            ColorValue = Inherit(playerSalmon.CourtshipTraits.ColorValue,mateSalmon.CourtshipTraits.ColorValue),

            ShapeValue = Inherit(playerSalmon.CourtshipTraits.ShapeValue,mateSalmon.CourtshipTraits.ShapeValue)
        };

        SalmonData child = new SalmonData(childUpstream, childCourtship);

        return child;
    }

    private float Inherit(float a, float b)
    {
        float average = (a + b) / 2f; //親の能力値の平均値を子の能力値にしてみた。次の行ぐらいで調整できる。

        float result = average;

        return result;
    }
}