using System.Collections.Generic;
using UnityEngine;

public class SplineMathModel
{
    public bool TryGetXAtY(IReadOnlyList<Vector2> globalSpline, float targetY, ref int lastIndex, out float resultX)
    {
        resultX = 0f;
        if (globalSpline == null || globalSpline.Count < 2) return false;
        if (lastIndex < 0 || lastIndex >= globalSpline.Count - 1) lastIndex = 0;

        for (int i = lastIndex; i < globalSpline.Count - 1; i++)
        {
            if (Check(globalSpline[i], globalSpline[i + 1], targetY, out resultX))
            {
                lastIndex = i; return true;
            }
        }
        for (int i = 0; i < lastIndex; i++)
        {
            if (Check(globalSpline[i], globalSpline[i + 1], targetY, out resultX))
            {
                lastIndex = i; return true;
            }
        }
        return false;
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