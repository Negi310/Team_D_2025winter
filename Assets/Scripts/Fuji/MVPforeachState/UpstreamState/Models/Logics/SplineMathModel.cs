using System.Collections.Generic;
using UnityEngine;

public class SplineMathModel
{
    public bool TryGetXAtY(IReadOnlyList<Vector2> globalSpline, float targetY, out float resultX)
    {
        resultX = 0f;
        if (globalSpline == null || globalSpline.Count < 2) return false;
        
        // ★修正: 二分探索をやめて、より安全な線形探索に変更（インスペクタのY座標のズレによるエラー落ちを防ぐ）
        for (int i = 0; i < globalSpline.Count - 1; i++)
        {
            Vector2 p1 = globalSpline[i];
            Vector2 p2 = globalSpline[i + 1];

            // ターゲットのY座標が、この線分の区間内にあるか？（昇順・降順どちらにも対応）
            if ((targetY >= p1.y && targetY <= p2.y) || (targetY >= p2.y && targetY <= p1.y))
            {
                if (Mathf.Abs(p2.y - p1.y) < 0.0001f) { resultX = p1.x; return true; }
                float t = (targetY - p1.y) / (p2.y - p1.y);
                resultX = p1.x + (p2.x - p1.x) * t;
                return true;
            }
        }

        return false; // 範囲外
    }
}