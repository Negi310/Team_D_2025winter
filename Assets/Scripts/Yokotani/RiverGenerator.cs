using UnityEngine;

public class RiverGenerator
{
    public static string GenerateRiverName(out int power, out int jump, out int cautiousness, out int stamina)
    {
        power = Random.Range(1, 8);
        jump = Random.Range(1, 8);
        cautiousness = Random.Range(1, 8);
        stamina = Random.Range(1, 8);

        int max = Mathf.Max(power, jump, cautiousness, stamina);//とりあえず最大値に対応した名前にする

        if (max == power)
        {
            return "キュウリュウ川";
        }
        else if (max == jump)
        {
            return "イワイワ川";
        }
        else if (max == cautiousness)
        {
            return "クマタクサン川";
        }
        else
        {
            return "シンドイ川";
        }
    }
}
