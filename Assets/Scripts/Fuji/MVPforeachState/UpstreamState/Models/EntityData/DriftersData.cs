using UnityEngine;

public class DrifterData
{
    public Vector2 Position { get; set; }
    public float Speed { get; }
    public float CurrentLane { get; set; }
    public float TargetLane { get; set; }
    public int LastSplineIndex = 0;

    public DrifterData(Vector2 startPos, float speed)
    {
        Position = startPos;
        Speed = speed;
        CurrentLane = Random.Range(0.1f, 0.9f);
        TargetLane = Random.Range(0.1f, 0.9f);
    }
}