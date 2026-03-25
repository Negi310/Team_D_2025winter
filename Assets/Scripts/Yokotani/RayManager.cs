using UnityEngine;

//Rayによる処理、そこからのスタミナの消費を管理してる

public class RayManager : MonoBehaviour
{
    public float rayDistance = 10f;

    public float stamina = 100f; //あとで鮭のスタミナを参照するようにする
    public float baseDrain = 1f;

    public float minWidth = 2f;
    public float maxWidth = 10f;

    float leftDistance;
    float rightDistance;
    float rockDistance;
    float nextRockDistance;
    float salmonDistance;

    public float forwardRayDistance = 5f;
    public float rockReduceRate = 0.5f;
    public float flowPenalty = 1.5f;

    public LayerMask wallLayer;
    public LayerMask rockLayer;
    public LayerMask nextRockLayer;
    public LayerMask salmonLayer;

    public RayData MeasureEnvironment()
    {
        var data = new RayData
        {
            LeftDistance = rayDistance, RightDistance = rayDistance,
            ForwardRockDistance = forwardRayDistance, ForwardNextRockDistance = forwardRayDistance, ForwardSalmonDistance = forwardRayDistance
        };
        
        MeasureWidth(data);
        //ConsumeStamina(data);
        CheckForwardRock(data);

        return data;
    }
    
    void MeasureWidth(RayData data)
    {
        RaycastHit hit;

        // 左
        if (Physics.Raycast(transform.position, -transform.right, out hit, rayDistance,wallLayer))
        {
            data.LeftDistance = hit.distance;
        }
        else
        {
            data.LeftDistance = rayDistance;
        }

        // 右
        if (Physics.Raycast(transform.position, transform.right, out hit, rayDistance,wallLayer))
        {
            data.RightDistance = hit.distance;
        }
        else
        {
            data.RightDistance = rayDistance;
        }
    }

    void CheckForwardRock(RayData data)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, forwardRayDistance, rockLayer)) //Rayの長さで近さを判定してるから別のものも判定したいなら個別に近さを設定しないといけない
        {
            data.ForwardRockDistance = hit.distance;
        }
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, forwardRayDistance, nextRockLayer))
        {
            data.ForwardNextRockDistance = hit.distance;
        }
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, forwardRayDistance, salmonLayer))
        {
            data.ForwardSalmonDistance = hit.distance;
        }
    }

    void ConsumeStamina()
    {
        float width = leftDistance + rightDistance;

        float t = Mathf.InverseLerp(maxWidth, minWidth, width); //幅の正規化

        /*float offset = Mathf.Abs(rightDistance - leftDistance) / width;
        float centerFactor = 1f - offset;*/

        float drain = baseDrain * (1f + t * 2f); //スタミナ消費量

        //drain *= (1f + centerFactor); 中央に近いほどスタミナ消費増

        if (rockDistance < forwardRayDistance)
        {
            drain *= rockReduceRate; //岩が近いほどスタミナ消費減
        }
        else if (nextRockDistance < forwardRayDistance)
        {
            drain *= rockReduceRate * 0.5f; //次の岩が近いほどスタミナ消費さらに減
        }
        else if (salmonDistance < forwardRayDistance)
        {
            drain *= flowPenalty; //鮭が近いほどスタミナ消費増
        }

        stamina -= drain * Time.deltaTime;

        Debug.Log("Width: " + width + "  Stamina: " + stamina);
    }

    void OnDrawGizmos() //Rayの可視化
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * rayDistance);
        Gizmos.DrawRay(transform.position, -transform.right * rayDistance);
        Debug.DrawRay(transform.position, transform.forward * forwardRayDistance, Color.blue);
    }
}
