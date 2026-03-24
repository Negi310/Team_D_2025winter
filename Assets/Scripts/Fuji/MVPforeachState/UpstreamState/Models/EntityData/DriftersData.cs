using UnityEngine;

public class DrifterData
{
    public DrifterType Type { get; }
    public Vector2 Position { get; set; }
    public float Speed { get; }
    public float CurrentLane { get; set; }
    public float TargetLane { get; set; }
    public int LastSplineIndex = 0;

    public DrifterData(DrifterType type, Vector2 startPos, float speed, float initLane)
    {
        Type = type;
        Position = startPos;
        Speed = speed;
        CurrentLane = initLane;
        TargetLane = initLane;
    }
}