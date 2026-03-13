using UnityEngine;

public class BreedCalculatable : IBreedingCalculator
{
    public SalmonData GenerateChild(SalmonData playerSalmon,SalmonData mateSalmon)
    {
        //川の遡上に関する能力値の継承
        UpstreamStats childUpstream = new UpstreamStats
        {
            Power = Inherit(playerSalmon.UpstreamStats.Power,mateSalmon.UpstreamStats.Power),

            Jump = Inherit(playerSalmon.UpstreamStats.Jump,mateSalmon.UpstreamStats.Jump),

            Cautiousness = Inherit(playerSalmon.UpstreamStats.Cautiousness,mateSalmon.UpstreamStats.Cautiousness),

            Stamina = Inherit(playerSalmon.UpstreamStats.Stamina,mateSalmon.UpstreamStats.Stamina)
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