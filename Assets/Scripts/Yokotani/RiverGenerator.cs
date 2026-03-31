using UnityEngine;

public class RiverGenerator
{
    // ... (GenerateRiverName メソッドはそのままなので省略) ...
    public static string GenerateRiverName(RiverData p)
    {
        float max = Mathf.Max(p.FlowSpeed, p.Narrowness, p.ObstacleDensity, p.FishDensity, 
            p.RivalDensity, p.AccidentDensity, p.Curviness);

        if (Mathf.Approximately(max, p.FlowSpeed)) return "キュウリュウ川";
        else if (Mathf.Approximately(max, p.ObstacleDensity)) return "イワイワ川";
        else if (Mathf.Approximately(max, p.AccidentDensity)) return "クマタクサン川";
        else if (Mathf.Approximately(max, p.Narrowness)) return "ホソボソ川";
        else if (Mathf.Approximately(max, p.FishDensity)) return "ウジャウジャ川";
        else if (Mathf.Approximately(max, p.RivalDensity)) return "ドンパチ川";
        else if (Mathf.Approximately(max, p.Curviness)) return "ウネウネ川";
        else return "フツウ川";
    }

    public static RiverData GenerateRiver(int generation)
    {
        RiverData profile = new RiverData();
        float baseAmount = 10f + (generation * 2f);

        // ==========================================
        // ★修正: 川の特性（尖り具合）を決定する
        // ==========================================
        float[] weights = new float[7];
        float totalWeight = 0f;
        
        float typeRand = Random.value;

        if (typeRand < 0.2f)
        {
            // 【20%】特徴のないフツウ川
            for (int i = 0; i < 7; i++) weights[i] = Random.Range(0.8f, 1.2f);
        }
        else if (typeRand < 0.5f)
        {
            // 【30%】1点特化（例：異常に岩だらけ、など）
            int specialIndex = Random.Range(0, 7);
            for (int i = 0; i < 7; i++) 
                weights[i] = (i == specialIndex) ? Random.Range(5.0f, 10.0f) : Random.Range(0.1f, 1.0f);
        }
        else
        {
            // 【50%】2点特化（例：細くて激流、小魚とライバルが大量など）
            // ※川は2点特化のほうが面白くなりやすいため確率を高めにしています
            int special1 = Random.Range(0, 7);
            int special2 = Random.Range(0, 7);
            while (special1 == special2) special2 = Random.Range(0, 7);

            for (int i = 0; i < 7; i++) 
                weights[i] = (i == special1 || i == special2) ? Random.Range(4.0f, 8.0f) : Random.Range(0.1f, 1.0f);
        }

        // 合計重みの算出
        for (int i = 0; i < 7; i++) totalWeight += weights[i];

        // 分配
        profile.FlowSpeed       = (weights[0] / totalWeight) * baseAmount;
        profile.Narrowness      = (weights[1] / totalWeight) * baseAmount;
        profile.ObstacleDensity = (weights[2] / totalWeight) * baseAmount;
        profile.FishDensity     = (weights[3] / totalWeight) * baseAmount;
        profile.RivalDensity    = (weights[4] / totalWeight) * baseAmount;
        profile.AccidentDensity = (weights[5] / totalWeight) * baseAmount;
        profile.Curviness       = (weights[6] / totalWeight) * baseAmount;
        
        profile.RiverName = GenerateRiverName(profile);

        return profile;
    }
}