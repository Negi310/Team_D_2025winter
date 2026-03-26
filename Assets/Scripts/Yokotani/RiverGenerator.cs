using UnityEngine;

public class RiverGenerator
{
    public static string GenerateRiverName(RiverData p)
    {
        float max = Mathf.Max(p.FlowSpeed, p.Narrowness, p.ObstacleDensity, p.FishDensity, 
            p.RivalDensity, p.AccidentDensity, p.Curviness);//とりあえず最大値に対応した名前にする

        if (Mathf.Approximately(max, p.FlowSpeed))
        {
            return "キュウリュウ川";
        }
        else if (Mathf.Approximately(max, p.ObstacleDensity))
        {
            return "イワイワ川";
        }
        else if (Mathf.Approximately(max, p.AccidentDensity))
        {
            return "クマタクサン川";
        }
        else if (Mathf.Approximately(max, p.Narrowness))
        {
            return "ホソボソ川";
        }
        else if (Mathf.Approximately(max, p.FishDensity))
        {
            return "ウジャウジャ川";
        }
        else if (Mathf.Approximately(max, p.RivalDensity))
        {
            return "ドンパチ川";
        }
        else if (Mathf.Approximately(max, p.Curviness))
        {
            return "ウネウネ川";
        }
        else
        {
            return "フツウ川";
        }
    }
    
    public static RiverData GenerateRiver(int generation)
    {
        RiverData profile = new RiverData();

        // 1. 世代に応じた基礎量（合計分配ポイント）を決定（例: 初期10pt + 世代ごとに2pt増）
        float baseAmount = 10f + (generation * 2f);

        // 2. 7つの内部パラメータにランダムに配分するための重み付け
        float[] weights = new float[7];
        float totalWeight = 0f;
        for (int i = 0; i < 7; i++)
        {
            weights[i] = Random.Range(0.1f, 1.0f); // 最低限の値を保証
            totalWeight += weights[i];
        }

        // 3. 合計が baseAmount になるように各パラメータにポイントを配分
        profile.FlowSpeed       = (weights[0] / totalWeight) * baseAmount;
        profile.Narrowness      = (weights[1] / totalWeight) * baseAmount;
        profile.ObstacleDensity = (weights[2] / totalWeight) * baseAmount;
        profile.FishDensity     = (weights[3] / totalWeight) * baseAmount;
        profile.RivalDensity    = (weights[4] / totalWeight) * baseAmount;
        profile.AccidentDensity = (weights[5] / totalWeight) * baseAmount;
        profile.Curviness       = (weights[6] / totalWeight) * baseAmount;
        
        // 4. 傾向から川の名前を命名
        profile.RiverName = GenerateRiverName(profile);

        return profile;
    }
}
