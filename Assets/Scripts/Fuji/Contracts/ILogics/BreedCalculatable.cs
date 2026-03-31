using UnityEngine;

public class BreedCalculatable : IBreedingCalculator
{
    public SalmonData GenerateChild(SalmonData playerSalmon,SalmonData mateSalmon)
    {
        //川の遡上に関する能力値の継承
        UpstreamStats childUpstream = new UpstreamStats
        {
            Speed = Mathf.Max(1f,Inherit(playerSalmon.UpstreamStats.Speed,mateSalmon.UpstreamStats.Speed)),

            Jump = Mathf.Max(1f,Inherit(playerSalmon.UpstreamStats.Jump,mateSalmon.UpstreamStats.Jump)),

            Stamina = Mathf.Max(1f,Inherit(playerSalmon.UpstreamStats.Stamina,mateSalmon.UpstreamStats.Stamina)),
            
            Attack = Mathf.Max(1f,Inherit(playerSalmon.UpstreamStats.Attack,mateSalmon.UpstreamStats.Attack)),
            
            Intelligence = Mathf.Max(1f,Inherit(playerSalmon.UpstreamStats.Intelligence,mateSalmon.UpstreamStats.Intelligence))
        };

        var pTraits = playerSalmon.CourtshipTraits;
        var mTraits = mateSalmon.CourtshipTraits;
        //求愛に関する能力値の継承
        int pSizeInt = (int)pTraits.Size;
        int mSizeInt = (int)mTraits.Size;
        int childSizeInt = Mathf.RoundToInt((pSizeInt + mSizeInt) / 2f + Random.Range(-0.5f, 0.5f));
        SalmonSize childSize = (SalmonSize)Mathf.Clamp(childSizeInt, 0, 2);

        CourtshipTraits childCourtship = new CourtshipTraits(
            childSize,
            Random.value > 0.5f ? pTraits.Color : mTraits.Color,
            Random.value > 0.5f ? pTraits.Hair : mTraits.Hair,
            Random.value > 0.5f ? pTraits.EyeIndex : mTraits.EyeIndex,
            Random.value > 0.5f ? pTraits.EyebrowIndex : mTraits.EyebrowIndex,
            Random.value > 0.5f ? pTraits.MouthIndex : mTraits.MouthIndex
        );

        return new SalmonData(childUpstream, childCourtship);
    }

    private float Inherit(float a, float b)
    {
        float average = (a + b) / 2f; //親の能力値の平均値を子の能力値にしてみた。次の行ぐらいで調整できる。

        float result = average;

        return result;
    }
}