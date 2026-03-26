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
        RayData data = new RayData();
        data = MeasureWidth(data);
        //ConsumeStamina(data);
        data = CheckForwardRock(data);
        //Debug.Log(data.LeftDistance + "  " + data.RightDistance + "  " + data.ForwardRockDistance + "  " + data.ForwardNextRockDistance + "  " + data.ForwardSalmonDistance);
        return data;
    }
    
    RayData MeasureWidth(RayData data)
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -transform.right, rayDistance, wallLayer);
        if (hitLeft.collider != null) data.LeftDistance = hitLeft.distance;
        else data.LeftDistance = rayDistance; //壁に当たらなかった場合は最大距離を記録

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, transform.right, rayDistance, wallLayer);
        if (hitRight.collider != null) data.RightDistance = hitRight.distance;
        else data.RightDistance = rayDistance; //壁に当たらなかった場合は最大距離を記録
        
        return data;
    }

    RayData CheckForwardRock(RayData data)
    {
        Vector2 forwardDir = transform.up; 

        // ★ Physics2D に変更し、ぶつかったらその距離を記録
        RaycastHit2D hitRock = Physics2D.Raycast(transform.position, forwardDir, forwardRayDistance, rockLayer);
        if (hitRock.collider != null) data.ForwardRockDistance = hitRock.distance;
        else data.ForwardRockDistance = forwardRayDistance;

        RaycastHit2D hitNextRock = Physics2D.Raycast(transform.position, forwardDir, forwardRayDistance, nextRockLayer);
        if (hitNextRock.collider != null) data.ForwardNextRockDistance = hitNextRock.distance;
        else data.ForwardNextRockDistance = forwardRayDistance;

        RaycastHit2D hitSalmon = Physics2D.Raycast(transform.position, forwardDir, forwardRayDistance, salmonLayer);
        if (hitSalmon.collider != null) data.ForwardSalmonDistance = hitSalmon.distance;
        else data.ForwardSalmonDistance = forwardRayDistance;
        
        return data;
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
        bool isHitLeft = Physics2D.Raycast(transform.position, -transform.right, rayDistance, wallLayer);
        Gizmos.color = isHitLeft ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, -transform.right * rayDistance);

        // 2. 右側のRay（壁に当たったら「緑」、当たらなければ「赤」）
        bool isHitRight = Physics2D.Raycast(transform.position, transform.right, rayDistance, wallLayer);
        Gizmos.color = isHitRight ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, transform.right * rayDistance);

        // 3. 前方のRay（岩や鮭に当たったら「黄色」、当たらなければ「青」）
        // 前方用のレイヤー（岩、次の岩、鮭）を |（OR演算子）で合体させてまとめてチェックします
        LayerMask forwardMask = rockLayer | nextRockLayer | salmonLayer;
        bool isHitForward = Physics2D.Raycast(transform.position, transform.up, forwardRayDistance, forwardMask);
        
        Gizmos.color = isHitForward ? Color.yellow : Color.blue;
        Gizmos.DrawRay(transform.position, transform.up * forwardRayDistance);
    }
}
