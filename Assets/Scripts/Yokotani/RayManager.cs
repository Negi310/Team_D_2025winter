using UnityEngine;

//Rayによる処理、そこからのスタミナの消費を管理してる

public class RayManager : MonoBehaviour
{
    public float rayDistance = 10f;

    public float stamina = 100f;
    public float baseDrain = 1f;

    public float minWidth = 2f;
    public float maxWidth = 10f;

    float leftDistance;
    float rightDistance;

    public float forwardRayDistance = 5f;
    public float rockReduceRate = 0.5f;
    bool isNearRock;
    bool isNearRockNext;
    public float flowPenalty = 1.5f;

    void Update()
    {
        MeasureWidth();
        ConsumeStamina();
        CheckForwardRock();
    }

    void MeasureWidth()
    {
        RaycastHit hit;

        // 左
        if (Physics.Raycast(transform.position, -transform.right, out hit, rayDistance))
        {
            leftDistance = hit.distance;
        }
        else
        {
            leftDistance = rayDistance;
        }

        // 右
        if (Physics.Raycast(transform.position, transform.right, out hit, rayDistance))
        {
            rightDistance = hit.distance;
        }
        else
        {
            rightDistance = rayDistance;
        }
    }

    void CheckForwardRock()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, forwardRayDistance)) //Rayの長さで近さを判定してるから別のものも判定したいなら個別に近さを設定しないといけない
        {
            if (hit.collider.CompareTag("Rock"))
            {
                isNearRock = true;
            }
            else
            {
                isNearRock = false;
            }

            if (hit.collider.CompareTag("RockNext"))
            {
                isNearRockNext = true;
            }
            else
            {
                isNearRockNext = false;
            }
        }
        else
        {
            isNearRock = false;
            isNearRockNext = false;
        }
    }

    void ConsumeStamina()
    {
        float width = leftDistance + rightDistance;

        float t = Mathf.InverseLerp(maxWidth, minWidth, width); //幅の正規化

        float drain = baseDrain * (1f + t * 2f); //スタミナ消費量

        if (isNearRock)
        {
            drain *= rockReduceRate;
        }

        if (isNearRockNext)
    {
        drain *= flowPenalty;
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
