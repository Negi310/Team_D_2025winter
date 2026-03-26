using System.Collections.Generic;
using UnityEngine;

public class SplineMathModel
{
    public bool TryGetXAtY(IReadOnlyList<Vector2> globalSpline, float targetY, out float resultX)
    {
        resultX = 0f;
        if (globalSpline == null || globalSpline.Count < 2) return false;
        
        int left = 0;
        int right = globalSpline.Count - 2; // 線分の「始点」を探すため -2

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            Vector2 p1 = globalSpline[mid];
            Vector2 p2 = globalSpline[mid + 1];

            // ターゲットのY座標が、この線分の区間内にあるか？
            if (targetY >= p1.y && targetY <= p2.y)
            {
                if (Mathf.Abs(p2.y - p1.y) < 0.0001f) { resultX = p1.x; return true; }
                float t = (targetY - p1.y) / (p2.y - p1.y);
                resultX = p1.x + (p2.x - p1.x) * t;
                return true;
            }

            // 区間外なら、半分を切り捨てる
            if (targetY < p1.y)
            {
                right = mid - 1; // もっと手前にある
            }
            else
            {
                left = mid + 1;  // もっと奥にある
            }
        }

        return false; // 範囲外
    }


    private bool Check(Vector2 p1, Vector2 p2, float targetY, out float resultX)
    {
        resultX = 0f;
        if ((p1.y <= targetY && targetY <= p2.y) || (p2.y <= targetY && targetY <= p1.y))
        {
            if (Mathf.Abs(p2.y - p1.y) < 0.0001f) { resultX = p1.x; return true; }
            float t = (targetY - p1.y) / (p2.y - p1.y);
            resultX = p1.x + (p2.x - p1.x) * t;
            return true;
        }
        return false;
    }
}